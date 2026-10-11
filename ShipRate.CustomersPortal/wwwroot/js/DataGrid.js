let pageNumber = 1;
let pageSize = 10;
let sortColumn = "";
let sortDirection = "";
let currentController = "";
let currentAction = "";

function loadGrid(controller, action, extraParams = "") {
    currentController = controller;
    currentAction = action;
    const url = `/${controller}/${action}?Partial=True&pageNumber=${pageNumber}&pageSize=${pageSize}&sortColumn=${sortColumn}&sortDirection=${sortDirection}${extraParams}`;
    //$("#gridContainer").html("Loading...");
    $.get(url, data => $("#gridContainer").html(data));
}

function goPage(controller, action, page) {
    pageNumber = page;
    loadGrid(controller, action);
}

function changePageSize(size) {
    pageSize = size;
    pageNumber = 1;
    loadGrid(currentController, currentAction);
}

function sortGrid(controller, action, column , sortDir) {
    if (sortColumn === column) {
        if (sortDir === "desc") {
            sortDirection = "asc"
        }
        if (sortDir === "asc") {
            sortDirection = "desc"
        }
    }
    else { sortColumn = column; sortDirection = "desc"; }
    loadGrid(controller, action);
}

function gridAction(controller, action, id) {
    window.location.href = `/${controller}/${action}/${id}`;
}