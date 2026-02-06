let book;
let rendition;

// Wait for the window to finish loading everything
window.addEventListener('DOMContentLoaded', (event) => {
    console.log('DOM fully loaded and parsed');
    // You can initialize a default book here if you like
});

function loadBook(url) {
    // Check if ePub exists before running
    if (typeof ePub === 'undefined') {
        console.error("ePub.js library is not loaded yet.");
        return;
    }

    const viewerElement = document.getElementById("viewer");
    viewerElement.innerHTML = "";

    book = ePub(url);
    rendition = book.renderTo("viewer", {
        width: "100%",
        height: "100%",
        flow: "paginated",
        manager: "default"
    });

    rendition.display().catch(err => console.error("Error displaying book:", err));
}

let allBookPaths = [];
let currentIndex = 0;
const BATCH_SIZE = 15; // Load 15 titles at a time

async function init() {
    try {
        const response = await fetch('backend/articles.db.json');
        allBookPaths = await response.json();
        
        // Start the observer
        setupInfiniteScroll();
    } catch (err) {
        console.error("Fetch error. Remember to run a local server for dev!", err);
    }
}

function renderTitles() {
    const list = document.getElementById('book-list');
    const fragment = document.createDocumentFragment();
    
    // Get the next slice of data
    const nextBatch = allBookPaths.slice(currentIndex, currentIndex + BATCH_SIZE);
    
    nextBatch.forEach(path => {
        const li = document.createElement('li');
        const a = document.createElement('a');
        
        // Clean the filename (e.g., "epubs/Book_Name.epub" -> "Book Name")
        const fileName = path.split('/').pop().replace('.epub', '').replace(/_/g, ' ');
        a.textContent = fileName;
        
        a.onclick = () => {
            document.querySelectorAll('#book-list a').forEach(el => el.classList.remove('is-active'));
            a.classList.add('is-active');
            loadBook(path); // Your existing epub.js function
        };
        
        li.appendChild(a);
        fragment.appendChild(li);
    });

    list.appendChild(fragment);
    currentIndex += BATCH_SIZE;
}

function setupInfiniteScroll() {
    const anchor = document.getElementById('scroll-anchor');
    
    const observer = new IntersectionObserver((entries) => {
        // If the anchor is visible and we have more books to show...
        if (entries[0].isIntersecting && currentIndex < allBookPaths.length) {
            renderTitles();
        }
    }, {
        root: document.querySelector('.sidebar'), // Watch scrolling inside sidebar
        threshold: 0.1
    });

    observer.observe(anchor);
}

init();