/* Author: Andrii Kudenko
   Description: javascript file for dynamic querying and displaying of players' information
*/

let debounceTimer;

document.addEventListener("DOMContentLoaded", function () {
    // Select the search input field by its ID
    const searchInput = document.getElementById("searchInput");

    // Clear the value of the search input
    if (searchInput) {
        searchInput.value = "";
    }
});
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
    const resultsContainer = document.getElementById("result-section");
    const resultsDiv = document.getElementById("users-result-list");

    resultsDiv.innerHTML = '';

    if (results.length === 0) {
        resultsDiv.innerHTML = `<li class="users-result-list-item">
                <p>No results found.</p>
            </li>`;
    } else {
        results.forEach(result => {
            const highlightedName = highlightMatch(result.name, searchQuery);
            const email = result.email.split("@");
            const highlightedEmail = highlightMatch(email[0], searchQuery);
            /*const highlightedEmail = highlightMatch(result.email, searchQuery);*/
            /*const card = `
                <div class="col-md-4">
                    <div class="card mb-4 shadow-sm">
                        <div class="card-body">
                            <h5 class="card-title unhighlighted">${highlightedName}</h5>
                            <p class="unhighlighted">${highlightedEmail}@${email[1]}</p>                            
                            <a href="/User/Profile/${result.id}" class="btn btn-primary">View Profile</a>
                        </div>
                    </div>
                </div>
            `;*/
            const card = `
            <a href="/User/Profile/${result.id}" class="users-result-link">
            <li class="users-result-list-item">
                <h5 class="card-title unhighlighted mb-8">${highlightedName}</h5>
                <p class="unhighlighted mb-6">${highlightedEmail}@${email[1]}</p>
                <svg class="view-user-arrow" xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#e8eaed"><path d="m242-200 200-280-200-280h98l200 280-200 280h-98Zm238 0 200-280-200-280h98l200 280-200 280h-98Z"/></svg>
            </li>
            </a>
            `;
            
            resultsDiv.innerHTML += card;
        });
    }

    resultsContainer.style.display = 'block';
}

function clearResults() {
    const resultsContainer = document.getElementById("result-section");
    const resultsDiv = document.getElementById("users-result-list");

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