using System.Reflection;
using System.Text.Json;
using AngleSharp;
using AngleSharp.Dom;
using QuickEPUB;
using static System.DateTime;

namespace Peizazhet.Scraper;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var backendDirectory =
            Directory.GetParent(Assembly.GetEntryAssembly()?.Location ?? string.Empty)?.Parent?.Parent?.Parent?.Parent?.Parent?.FullName;
        
        Console.WriteLine($"Working on: '{backendDirectory}'");
        
        var articlesDbJsonFilePath = $@"{backendDirectory}/articles.db.json";
        var allArticlesLink = args.FirstOrDefault() ?? "https://peizazhe.com/arkivi/";
        var invalidFileNameChars = Path.GetInvalidFileNameChars(); // We use this to sanitize the fileNames
        
// Custom CSS to format the author div content at the end of the article
        string authorCss = """
                            /* ============================================
                              AUTHOR BIO SECTION - EPUB Compatible
                              Colors inherit from reader theme
                              ============================================ */
                           
                           /* Main Author Bio Container */
                           .author-bio {
                               padding: 25px !important;
                               margin: 40px 0 !important;
                               background: transparent !important;
                               border: 1px solid currentColor !important;
                               border-radius: 4px !important;
                               display: block !important;
                           }
                           
                           /* Author Info Wrapper */
                           .author-bio .author-info {
                               display: block !important;
                               overflow: hidden !important;
                           }
                           
                           /* Author Avatar Container - Float Left */
                           .author-bio .author-avatar {
                               float: left !important;
                               margin: 0 20px 15px 0 !important;
                           }
                           
                           /* Author Avatar Image */
                           .author-bio .author-avatar img,
                           .author-bio .author-avatar .avatar {
                               width: 140px !important;
                               height: 140px !important;
                               border-radius: 0 !important;
                               display: block !important;
                           }
                           
                           /* Author Details Container (name + social icons) */
                           .author-bio .author-details {
                               display: block !important;
                               margin-bottom: 15px !important;
                           }
                           
                           /* Author Name */
                           .author-bio .author-name,
                           .author-bio h3 {
                               font-size: 2rem !important;
                               font-weight: 700 !important;
                               margin: 0 0 10px 0 !important;
                               line-height: 1.2 !important;
                               display: inline-block !important;
                               margin-right: 15px !important;
                           }
                           
                           .author-bio .author-name a,
                           .author-bio h3 a {
                               text-decoration: none !important;
                           }
                           
                           /* Social Icons */
                           .author-bio .social-icons {
                               display: inline-block !important;
                               vertical-align: middle !important;
                           }
                           
                           .author-bio .social-icons a {
                               display: inline-block !important;
                               margin: 0 8px !important;
                               text-decoration: none !important;
                               vertical-align: middle !important;
                           }
                           
                           .author-bio .social-icons a:first-child {
                               margin-left: 0 !important;
                           }
                           
                           .author-bio .social-icons svg {
                               width: 20px !important;
                               height: 20px !important;
                               fill: currentColor !important;
                           }
                           
                           /* Author Description */
                           .author-bio .author-description,
                           .author-bio p {
                               font-size: 1rem !important;
                               line-height: 1.7 !important;
                               margin: 0 !important;
                               font-weight: 400 !important;
                               clear: none !important;
                           }
                           
                           /* Links in description */
                           .author-bio p a {
                               text-decoration: none !important;
                           }
                           
                           .author-bio p a:hover {
                               text-decoration: underline !important;
                           }
                           
                           /* Responsive Design */
                           @media (max-width: 768px) {
                               .author-bio {
                                   padding: 20px !important;
                               }
                               
                               .author-bio .author-avatar {
                                   margin: 0 15px 10px 0 !important;
                               }
                               
                               .author-bio .author-avatar img {
                                   width: 100px !important;
                                   height: 100px !important;
                               }
                               
                               .author-bio .author-name,
                               .author-bio h3 {
                                   font-size: 1.6rem !important;
                               }
                           }
                           
                           @media (max-width: 480px) {
                               .author-bio {
                                   padding: 15px !important;
                               }
                               
                               .author-bio .author-avatar {
                                   float: none !important;
                                   margin: 0 0 15px 0 !important;
                               }
                               
                               .author-bio .author-name,
                               .author-bio h3 {
                                   font-size: 1.4rem !important;
                                   display: block !important;
                                   margin-bottom: 10px !important;
                               }
                               
                               .author-bio .social-icons {
                                   display: block !important;
                                   margin-top: 10px !important;
                               }
                               
                               .author-bio p {
                                   font-size: 0.95rem !important;
                               }
                           }
                           """;

        if (backendDirectory == null || !Directory.Exists(backendDirectory))
        {
            throw new DirectoryNotFoundException(backendDirectory);
        }
        
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(allArticlesLink);


        var savedArticles = await GetSavedArticlesFromDb(articlesDbJsonFilePath);
        var allArticles = ScrapeAllArticlesFromArchive(document);
        
        var articlesToScrape = allArticles.Where(article =>
            savedArticles != null && !savedArticles.Select(x => x.Link).Contains(article.Link)).ToList();
        

        // Visit each scraped link
        foreach (var element in articlesToScrape)
        {
            Console.WriteLine($"Preparing the document {element.Title}...");
            
            document = await context.OpenAsync(element.Link);

            var article = GetArticleTagContent(document);
            element.PublishDateTime = article.PublishedDate;

            // 2. Remove the tags for share, donate and follow
            foreach (var elementToRemove in article.ArticleTag?.QuerySelectorAll(".wp-block-group, .share-buttons, .box, .more")!)
            {
                elementToRemove.Remove();
            }

            var newDocument = await CreateNewHtmlDocumentForArticle(context, document, authorCss, article);
            
            var cleanTitle = invalidFileNameChars.Aggregate(element.Title, (current, c) => current.Replace(c, '_'));

            // Create an Epub instance
            var doc = new Epub(cleanTitle,element.Author);

            // Adding sections of HTML content
            doc.AddSection(element.Title, newDocument.DocumentElement.OuterHtml);


            await using var fs = new FileStream($@"{backendDirectory}/articles/{cleanTitle}.epub", FileMode.Create);
            doc.Export(fs);
            
            Console.WriteLine($"{cleanTitle}.epub was saved successfully!");
        }

        await UpdateDb(savedArticles, articlesToScrape, articlesDbJsonFilePath);
    }

    private static async Task UpdateDb(List<Article>? savedArticles, List<Article> articlesToScrape, string articlesDbJsonFilePath)
    {
        // In the end update the articles.db.json file
        savedArticles?.AddRange(articlesToScrape);

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        var updatedJson = JsonSerializer.Serialize(savedArticles, options);
        await File.WriteAllTextAsync(articlesDbJsonFilePath, updatedJson);
    }

    private static async Task<IDocument> CreateNewHtmlDocumentForArticle(IBrowsingContext context, IDocument document,
        string authorCss, (IElement? ArticleTag, DateTime? PublishedDate) article)
    {
        // 1. Create a new clean HTML document
        var newDocument = await context.OpenNewAsync();
            
        // 2. Create the <style> element
        var styleElement = document.CreateElement("style");
        styleElement.TextContent = authorCss;

        // 3. Append it to the <head> of the document
        newDocument.Head?.AppendChild(styleElement);
        newDocument.Body?.AppendChild(article.ArticleTag);
        return newDocument;
    }

    private static (IElement? ArticleTag, DateTime? PublishedDate) GetArticleTagContent(IDocument document)
    {
        // 1. Get all the content of the <article>
        var article = document.QuerySelector("article");
        var publishedDateTime = document.QuerySelector("time.entry-date.published")?.GetAttribute("datetime");

        TryParse(publishedDateTime, out var dateTime);
        return (article, dateTime);
    }

    private static async Task<List<Article>?> GetSavedArticlesFromDb(string articlesDbJsonFilePath)
    {
        string savedArticlesAsJsonString = await File.ReadAllTextAsync(articlesDbJsonFilePath);

        var savedArticles = new List<Article>();

        if (savedArticlesAsJsonString.Length > 0)
            savedArticles = JsonSerializer.Deserialize<List<Article>>(savedArticlesAsJsonString);
        return savedArticles;
    }

    private static IEnumerable<Article> ScrapeAllArticlesFromArchive(IDocument document)
    {
        var articlesDiv = document.QuerySelectorAll(".archives-by-cat>p");

        // Get all the article links
        var allArticles = articlesDiv.Select(item =>
        {

            // Find the anchor <a> tag within the item
            var linkElement = item.QuerySelector("a");

            // Find the author span
            var authorElement = item.QuerySelector("span.author");

            return new Article
            {
                Link = linkElement.GetAttribute("href"),
                Title = linkElement.TextContent,
                Author = authorElement.TextContent
            };
        });
        return allArticles;
    }
}