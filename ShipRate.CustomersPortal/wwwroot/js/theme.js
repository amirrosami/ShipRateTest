(function () {
    const themes = ["ocean", "dark", "desert"];

    function setTheme(name) {
        document.documentElement.setAttribute("data-theme", name);
        localStorage.setItem("theme", name);
    }

    document.querySelectorAll(".theme-option").forEach(btn => {
        btn.addEventListener("click", () => {
            setTheme(btn.dataset.theme);
        });
    });

    const cycleBtn = document.getElementById("themeCycleBtn");
    if (cycleBtn) {
        cycleBtn.addEventListener("click", () => {
            const current = localStorage.getItem("theme") || "ocean";
            const next = themes[(themes.indexOf(current) + 1) % themes.length];
            setTheme(next);
        });
    }
})();
