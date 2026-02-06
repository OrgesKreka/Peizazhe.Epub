let book, rendition;
let allBookPaths = [];
let currentIndex = 0;
const ITEMS_PER_PAGE = 20;
const scrollContainer = document.getElementById("scroll-container");
// 1. Fetch JSON and Init Sidebar
async function init() {
  try {
    const response = await fetch("backend/articles.db.json");
    allBookPaths = await response.json();
    allBookPaths.sort((a, b) => {
      return new Date(b.PublishDateTime) - new Date(a.PublishDateTime); // descending
    });

    // Initial load
    loadNextBatch();

    // Set up Intersection Observer for infinite scroll in sidebar
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && currentIndex < allBookPaths.length) {
          loadNextBatch();
        }
      },
      { root: document.getElementById("sidebar-scroll"), threshold: 1.0 },
    );

    observer.observe(document.getElementById("scroll-anchor"));
  } catch (e) {
    console.error("Failed to load JSON library.", e);
  }
}

function loadNextBatch() {
  const list = document.getElementById("book-list");
  const nextBatch = allBookPaths.slice(
    currentIndex,
    currentIndex + ITEMS_PER_PAGE,
  );

  nextBatch.forEach((element) => {
    const li = document.createElement("li");
    li.className = "book-item-wrapper";

    const path = "backend/articles/" + element.FileName;

    // Main Link Container
    const a = document.createElement("a");
    a.className = "book-link-container";

    // 1. Download Icon (Left)
    const dl = document.createElement("span");
    dl.className = "download-action";
    dl.innerHTML = `<svg viewBox="0 0 24 24" width="18" height="18" stroke="currentColor" stroke-width="2" fill="none"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v4"></path><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>`;

    dl.onclick = (e) => {
      e.stopPropagation(); // Prevents loading the book when clicking download
      const link = document.createElement("a");
      link.href = path;
      link.download = path.split("/").pop();
      link.click();
    };

    // 2. Text Content (Right)
    const textContainer = document.createElement("div");
    textContainer.className = "book-info";

 
    textContainer.innerHTML = `
            <span class="book-title">${element.Title}</span>
            <span class="book-author">• ${element.Author}</span>
        `;

    a.onclick = () => {
      document
        .querySelectorAll(".menu-list a")
        .forEach((el) => el.classList.remove("is-active"));
      a.classList.add("is-active");
      loadBook(path);
    };

    a.appendChild(dl);
    a.appendChild(textContainer);
    li.appendChild(a);
    list.appendChild(li);
  });

  currentIndex += nextBatch.length;
}

// 3. ePub Logic
async function loadBook(url) {
  const viewerDiv = document.getElementById("viewer");
  viewerDiv.innerHTML =
    "<div id='loading-status' class='p-5 has-text-centered'>Opening book...</div>";

  try {
    // Fetch file as binary data to bypass MIME and Policy errors
    const response = await fetch(url);
    const buffer = await response.arrayBuffer();

    if (book) book.destroy();

    book = ePub(buffer);

    rendition = book.renderTo("viewer", {
      flow: "scrolled",
      width: "100%",
      height: "auto", // Necessary for continuous vertical flow
      allowScriptedContent: true,
    });

    await rendition.display();

    console.log("Book displayed successfully!");

    // Clean up: Only remove loading status if display succeeded
    const loadingStatus = document.getElementById("loading-status");
    console.log(loadingStatus);
    if (loadingStatus) loadingStatus.remove();

    setTheme(currentTheme); // Apply current UI theme to book
  } catch (err) {
    viewerDiv.innerHTML = `<div class="notification is-danger">Failed to load: ${err.message}</div>`;
  }
}
// 3. Theme Logic
let currentTheme = "dark";
const themes = {
  dark: { bg: "#1a1d21", text: "#d1d5db", sidebar: "#121417" },
  sepia: { bg: "#f4ecd8", text: "#5f4b32", sidebar: "#eadecd" },
  light: { bg: "#ffffff", text: "#333333", sidebar: "#f5f5f5" },
};

function setTheme(t) {
  currentTheme = t;
  const config = themes[t];

  // Update UI
  document.body.style.backgroundColor = config.bg;
  document.getElementById("scroll-container").style.backgroundColor = config.bg;
  document.querySelector(".sidebar").style.backgroundColor = config.sidebar;

  // Update Book Content
  if (rendition) {
    rendition.themes.default({
      body: {
        "background-color": `${config.bg} !important`,
        color: `${config.text} !important`,
        "font-family": "'Merriweather', serif !important",
        "font-size": "20px !important",
        "line-height": "1.8 !important",
        padding: "0 10% !important",
      },
    });
  }
}

// 4. Progress Tracking
scrollContainer.onscroll = () => {
  const totalHeight =
    scrollContainer.scrollHeight - scrollContainer.clientHeight;
  const progress = (scrollContainer.scrollTop / totalHeight) * 100;
  document.getElementById("progress-bar").style.width = progress + "%";
};

init();
