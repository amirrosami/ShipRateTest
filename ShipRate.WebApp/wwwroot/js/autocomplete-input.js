(function () {
    let debounceTimer;

    document.addEventListener('input', function (e) {
        if (!e.target.matches('[data-autocomplete]')) return;

        const input = e.target;
        const container = input.closest('.autocomplete-container');
        const results = container.querySelector('.autocomplete-results');
        const hidden = container.querySelector('.autocomplete-hidden');
        hidden.value = '';
        // 1. Get base URL and additional fields config
        const baseUrl = input.dataset.url;
        const additionalFields = input.dataset.additionalFields;
        const term = input.value.trim();

        if (term.length < 2) {
            results.style.display = 'none';
            return;
        }

        // 2. Build the query parameters
        const queryParams = new URLSearchParams({ term: term });

        // If there are dependent fields, find them in the current form and add to URL
        if (additionalFields) {
            const form = input.closest('form');
            if (form) {
                additionalFields.split(',').forEach(fieldName => {
                    const field = form.querySelector(`[name="${fieldName.trim()}"]`);
                    if (field) {
                        queryParams.append(fieldName.trim(), field.value);
                    }
                });
            }
        }

        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => {
            fetch(`${baseUrl}?${queryParams.toString()}`)
                .then(r => r.json())
                .then(data => {
                    results.innerHTML = '';
                    if (data.length === 0) {
                        results.style.display = 'none';
                        return;
                    }
                    data.forEach(item => {
                        const a = document.createElement('a');
                        a.href = '#';
                        a.className = 'list-group-item list-group-item-action';
                        a.textContent = item.locationName || item.name;

                        a.addEventListener('click', function (ev) {
                            ev.preventDefault();
                            input.value = item.locationName || item.name;
                            hidden.value = item.id;
                            results.style.display = 'none';
                        });
                        results.appendChild(a);
                    });
                    results.style.display = 'block';
                });
        }, 300);
    });

    document.addEventListener('click', function (e) {
        if (!e.target.closest('.autocomplete-container')) {
            document.querySelectorAll('.autocomplete-results').forEach(el => el.style.display = 'none');
        }
    });
})();



//// wwwroot/js/autocomplete-global.js
//(function () {
//    let debounceTimer;

//    document.addEventListener('input', function (e) {
//        if (!e.target.matches('[data-autocomplete]')) return;
        
//        const input = e.target;
//        // Find the parent container to scope our lookup
//        const container = input.closest('.autocomplete-container');
//        const results = container.querySelector('.autocomplete-results');
//        const hidden = container.querySelector('.autocomplete-hidden');
//        hidden.value = '';
//        const apiUrl = input.dataset.url;
//        const term = input.value.trim();

//        if (term.length < 2) {
//            results.style.display = 'none';
//            return;
//        }

//        clearTimeout(debounceTimer);
//        debounceTimer = setTimeout(() => {
//            fetch(`${apiUrl}?term=${encodeURIComponent(term)}`)
//                .then(r => r.json())
//                .then(data => {
//                    results.innerHTML = '';
//                    if (data.length === 0) {
//                        results.style.display = 'none';
//                        return;
//                    }
//                    data.forEach(item => {
//                        const a = document.createElement('a');
//                        a.href = '#';
//                        a.className = 'list-group-item list-group-item-action';
//                        // Assuming your API returns 'locationName' and 'id'
//                        // If fields differ, you might need a data-value-key and data-text-key attribute
//                        a.textContent = item.locationName || item.name;

//                        a.addEventListener('click', function (ev) {
//                            ev.preventDefault();
//                            input.value = item.locationName || item.name;
//                            hidden.value = item.id;
//                            results.style.display = 'none';
//                        });
//                        results.appendChild(a);
//                    });
//                    results.style.display = 'block';
//                });
//        }, 300);
//    });

//    // Close results when clicking away
//    document.addEventListener('click', function (e) {
//        if (!e.target.closest('.autocomplete-container')) {
//            document.querySelectorAll('.autocomplete-results').forEach(el => el.style.display = 'none');
//        }
//    });
//})();
