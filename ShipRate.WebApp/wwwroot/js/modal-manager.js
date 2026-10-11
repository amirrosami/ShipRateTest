window.AppModal = (() => {

    let modalElement = document.getElementById('app-modal');

    let modal = new bootstrap.Modal(modalElement);

    function open() {
        modal.show();
    }

    function close() {
        modal.hide();
    }

    function clear() {
        document.getElementById('app-modal-content').innerHTML = '';
    }

    modalElement.addEventListener('hidden.bs.modal', () => {
        clear();
    });

    return {
        open,
        close,
        clear
    };

})();



window.AppModalSecond = (() => {

    let modalElement = document.getElementById('app-modal-second');

    let modal = new bootstrap.Modal(modalElement);

    function open() {
        modal.show();
    }

    function close() {
        modal.hide();
    }

    function clear() {
        document.getElementById('app-modal-content-second').innerHTML = '';
    }

    modalElement.addEventListener('hidden.bs.modal', () => {
        clear();
    });

    return {
        open,
        close,
        clear
    };

})();






document.body.addEventListener('htmx:afterSwap', function (e) {

    if (e.detail.target.id === 'app-modal-content') {
        AppModal.open();
    }
    if (e.detail.target.id === 'app-modal-content-second') {
        AppModalSecond.open();
    }

});



document.body.addEventListener('GridChanged', function () {
    AppModal.close();
});


document.body.addEventListener('GridChangedOnAppModal', function () {
    AppModalSecond.close();
});


document.body.addEventListener('toastSuccess', (e) => {

    createToast(e.detail.value,"success");

});

document.body.addEventListener('toastFail', (e) => {

    createToast(e.detail.value, "danger");

});




function handleCheckboxChange(checkbox) {
    const val = checkbox.value;
    const isChecked = checkbox.checked;

    console.log("value: " + val + "  checked = " + isChecked);

    if (isChecked) {
        checkbox.value = "true"
    } else {
        checkbox.value = "false"
    }
}

function getValue(form, field) {
    const el = form.querySelector(`[name="${field}"]`);
    if (!el) return null;
    //if (el.type === 'checkbox') {
    //    return el.checked; // Returns true if checked, false if not
    //}
    return el.value;
}

function evalSimple(expr, form) {
    expr = expr.trim();

    let m;

    // in(...)
    if ((m = expr.match(/^(\w+)\s+in\((.+)\)$/))) {
        const val = getValue(form, m[1]);
        const list = m[2].split(",").map(v => v.trim());
        return list.includes(val);
    }

    // !in(...)
    if ((m = expr.match(/^(\w+)\s+!in\((.+)\)$/))) {
        const val = getValue(form, m[1]);
        const list = m[2].split(",").map(v => v.trim());
        return !list.includes(val);
    }

    // comparisons
    if ((m = expr.match(/^(\w+)\s*(=|!=|>|<|>=|<=)\s*(.+)$/))) {
        let value = getValue(form, m[1]);
        let expected = m[3].trim();

        if (!isNaN(value) && !isNaN(expected)) {
            value = Number(value);
            expected = Number(expected);
        }

        switch (m[2]) {
            case "=": return value == expected;
            case "!=": return value != expected;
            case ">": return value > expected;
            case "<": return value < expected;
            case ">=": return value >= expected;
            case "<=": return value <= expected;
        }
    }

    return false;
}

function evaluateCondition(condition, form) {

    // OR first
    const orParts = condition.split("||");

    for (let part of orParts) {

        // AND inside each OR block
        const andParts = part.split("&&");

        let andResult = true;

        for (let expr of andParts) {
            if (!evalSimple(expr, form)) {
                andResult = false;
                break;
            }
        }

        if (andResult) return true;
    }

    return false;
}





function updateConditional(form) {

    form.querySelectorAll("[data-show-if]").forEach(el => {

        const rule = el.dataset.showIf;
        const show = evaluateCondition(rule, form);

        el.style.display = show ? "" : "none";

        const inputs = el.querySelectorAll("input, select, textarea");

        inputs.forEach(input => {

            input.disabled = !show;

            if (!show) {

                if (input.type === "checkbox" || input.type === "radio") {
                    input.checked = false;
                }
                else if (input.tagName === "SELECT" || input.tagName === "select") {
                    input.selectedIndex = -1; // or 0 if you prefer first option
                }
                else {
                    input.value = "";
                }

            }

        });

    });

}


document.addEventListener("change", e => {
    const form = e.target.closest("form");
    console.log("change triggered");
    if (form) updateConditional(form);
});

document.addEventListener("DOMContentLoaded", () => {
    console.log("DOMContentLoaded triggered");
    document.querySelectorAll("form").forEach(updateConditional);
});

document.body.addEventListener("htmx:load", function (e) {
    const container = e.target;

    const forms = container.querySelectorAll("form");
    console.log("htmx-load triggered");
    console.log(forms);
    forms.forEach(form => {
        updateConditional(form);
    });

});


document.body.addEventListener("htmx:afterSwap", function (e) {

    if (e.target.id === "app-modal-content") {

        const form = e.target.querySelector("form");

        if (form) {
            updateConditional(form);
        }
    }
});

document.body.addEventListener("htmx:responseError", function (evt) {
    const msg = evt.detail.xhr.responseText;
    console.log("error!!!!!     " + msg)
    createToast(msg, "danger");
});
