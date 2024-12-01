let debounceTimer;

/*alert("Hello")*/
document.getElementById("searchInput").addEventListener("input", function () {
    const searchQuery = this.value.trim();

    clearTimeout(debounceTimer);

    debounceTimer = setTimeout(() => {
        if (searchQuery) {
            /*fetch(`/Search/UserResults?query=${encodeURIComponent(searchQuery)}`)*/
            fetch(`/Search/UserResults?query=${encodeURIComponent(searchQuery)}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }   
                    console.log(response);
                    return response.json()
                })
                .then(data => {
                    console.log(data.result);
                    displayResults(data.result, searchQuery);
                })
                .catch(error => console.error('Error fetching search results:', error));
        } else {
            clearResults();
        }
    }, 300);
});

function displayResults(results, searchQuery) {
    const resultsContainer = document.getElementById("resultsContainer");
    const resultsDiv = document.getElementById("results");

    resultsDiv.innerHTML = '';

    if (results.length === 0) {
        resultsDiv.innerHTML = `<p>No results found.</p>`;
    } else {
        results.forEach(result => {
            const highlightedName = highlightMatch(result.name, searchQuery);
            const email = result.email.split("@");
            const highlightedEmail = highlightMatch(email[0], searchQuery);
            /*const highlightedEmail = highlightMatch(result.email, searchQuery);*/
            const card = `
                <div class="col-md-4">
                    <div class="card mb-4 shadow-sm">
                        <div class="card-body">
                            <h5 class="card-title unhighlighted">${highlightedName}</h5>
                            <p class="unhighlighted">${highlightedEmail}@${email[1]}</p>                            
                            <a href="/User/Profile/${result.id}" class="btn btn-primary">View Profile</a>
                        </div>
                    </div>
                </div>
            `;
            resultsDiv.innerHTML += card;
        });
    }

    resultsContainer.style.display = 'block';
}

function clearResults() {
    const resultsContainer = document.getElementById("resultsContainer");
    const resultsDiv = document.getElementById("results");

    resultsDiv.innerHTML = '';
    resultsContainer.style.display = 'none';
}

function highlightMatch(text, query) {
    if (!query) return text; // No query, return the text as is

    // Create a regex pattern to match the query (case-insensitive)
    const regex = new RegExp(`(${query})`, 'gi');
    // Replace the matched query with a highlighted version
    return text.replace(regex, '<span class="highlighted">$1</span>');
}