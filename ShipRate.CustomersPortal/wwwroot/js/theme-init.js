(function () {
    const saved = localStorage.getItem("theme") || "ocean";
    document.documentElement.setAttribute("data-theme", saved);
})();
