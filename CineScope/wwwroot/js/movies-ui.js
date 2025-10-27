// movies-ui.js
// Handles: dark/light theme toggle, load-more button, optional infinite scroll

(function () {
    // ---- Theme toggle (persist in localStorage) ----
    const themeToggle = document.getElementById('theme-toggle');
    const root = document.documentElement;
    const THEME_KEY = 'cinescope-theme';

    function applyTheme(theme) {
        if (theme === 'dark') {
            root.classList.add('dark-mode');
            root.classList.remove('light-mode');
            if (themeToggle) themeToggle.textContent = 'Light';
        } else {
            root.classList.add('light-mode');
            root.classList.remove('dark-mode');
            if (themeToggle) themeToggle.textContent = 'Dark';
        }
    }

    const saved = localStorage.getItem(THEME_KEY) || 'dark';
    applyTheme(saved);

    if (themeToggle) {
        themeToggle.addEventListener('click', () => {
            const cur = localStorage.getItem(THEME_KEY) || 'dark';
            const next = (cur === 'dark') ? 'light' : 'dark';
            localStorage.setItem(THEME_KEY, next);
            applyTheme(next);
        });
    }

    // ---- Load more / Infinite scroll ----
    const loadMoreBtn = document.getElementById('load-more');
    const moviesContainer = document.getElementById('movies-container');
    const loadingIndicator = document.getElementById('loading-indicator');

    async function fetchPage(page) {
        loadingIndicator.style.display = 'block';
        try {
            const url = `/Movies/Trending?page=${page}`;
            const resp = await fetch(url, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            if (!resp.ok) throw new Error('Network error');
            const html = await resp.text();
            // Append HTML partial to container
            const temp = document.createElement('div');
            temp.innerHTML = html;
            // The partial returns markup with .movie-cards row. Append child nodes
            const nodes = temp.querySelectorAll('.movie-card-wrap, .movie-card-wrap *');
            // Simpler: append innerHTML
            moviesContainer.insertAdjacentHTML('beforeend', html);
            return true;
        } catch (e) {
            console.error(e);
            return false;
        } finally {
            loadingIndicator.style.display = 'none';
        }
    }

    async function onLoadMoreClicked() {
        const current = parseInt(moviesContainer.dataset.page || '1', 10);
        const next = current + 1;
        const ok = await fetchPage(next);
        if (ok) {
            moviesContainer.dataset.page = String(next);
        } else {
            // disable button on failure
            loadMoreBtn.disabled = true;
            loadMoreBtn.textContent = 'No more results';
        }
    }

    if (loadMoreBtn) {
        loadMoreBtn.addEventListener('click', onLoadMoreClicked);
    }

    // Optional: infinite scroll. Uncomment to enable automatic loading when near bottom.
    let infiniteEnabled = false; // set to true to enable automatically
    if (infiniteEnabled) {
        window.addEventListener('scroll', async () => {
            if ((window.innerHeight + window.scrollY) >= (document.body.offsetHeight - 600)) {
                // near bottom
                if (!loadMoreBtn.disabled && !loadingIndicator.style.display) {
                    await onLoadMoreClicked();
                }
            }
        });
    }

    // Accessibility: keyboard enter on load more
    if (loadMoreBtn) {
        loadMoreBtn.addEventListener('keyup', (e) => {
            if (e.key === 'Enter') onLoadMoreClicked();
        });
    }
})();
