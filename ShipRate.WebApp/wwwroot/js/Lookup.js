function initLookup(cfg) {
    var input = document.getElementById(cfg.inputId);
    var hidden = document.getElementById(cfg.hiddenId);
    var results = document.getElementById(cfg.resultsId);
    var timer = null;
    var activeIndex = -1;
    var items = [];

    // type in input
    input.addEventListener('input', function () {
        var term = input.value.trim();
        hidden.value = '';
        clearTimeout(timer);

        if (term.length < cfg.minChars) {
            close();
            return;
        }

        timer = setTimeout(function () {
            search(term);
        }, 300);
    });

    // keyboard
    input.addEventListener('keydown', function (e) {
        if (e.key === 'ArrowDown') {
            e.preventDefault();
            move(1);
        } else if (e.key === 'ArrowUp') {
            e.preventDefault();
            move(-1);
        } else if (e.key === 'Enter') {
            e.preventDefault();
            select(activeIndex);
        } else if (e.key === 'Escape') {
            close();
        }
    });

    // close when click outside
    document.addEventListener('click', function (e) {
        if (!document.getElementById(cfg.wrapperId).contains(e.target)) {
            close();
        }
    });

    function search(term) {
        fetch(cfg.url + '?term=' + encodeURIComponent(term))
            .then(function (r) { return r.json(); })
            .then(function (data) {
                items = Array.isArray(data) ? data : [];
                render();
            });
    }

    function render() {
        activeIndex = -1;
        results.innerHTML = '';

        if (items.length === 0) {
            results.innerHTML = '<div class="lookup-no-results">No results</div>';
            results.style.display = 'block';
            return;
        }

        for (var i = 0; i < items.length; i++) {
            var div = document.createElement('div');
            div.className = 'lookup-item';
            div.textContent = items[i][cfg.textField];
            div.setAttribute('data-index', i);
            div.addEventListener('mousedown', function (e) {
                e.preventDefault();
                select(parseInt(this.getAttribute('data-index')));
            });
            results.appendChild(div);
        }

        results.style.display = 'block';
    }

    function move(dir) {
        var divs = results.querySelectorAll('.lookup-item');
        if (divs.length === 0) return;

        activeIndex += dir;
        if (activeIndex < 0) activeIndex = 0;
        if (activeIndex >= divs.length) activeIndex = divs.length - 1;

        divs.forEach(function (d) { d.classList.remove('active'); });
        divs[activeIndex].classList.add('active');
    }

    function select(index) {
        if (index < 0 || index >= items.length) return;
        input.value = items[index][cfg.textField];
        hidden.value = items[index][cfg.valueField];
        close();
    }

    function close() {
        results.style.display = 'none';
        results.innerHTML = '';
        activeIndex = -1;
    }
}
