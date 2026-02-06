namespace Peizazhet.Scraper;

public class Article
{
    public required string Link { get; set; }
    public required string Author { get; set; }
    public required string Title { get; set; }
    public DateTime? PublishDateTime { get; set; }
}