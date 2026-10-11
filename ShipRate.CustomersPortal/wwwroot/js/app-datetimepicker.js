class AppDateTimePicker {

    constructor(wrapper) {

        this.wrapper = wrapper;

        this.display = wrapper.querySelector(".app-dtp-display");
        this.displayEnd = wrapper.querySelector(".app-dtp-display-end");

        this.hidden = wrapper.querySelector("input[type=hidden]");
        this.dropdown = wrapper.querySelector(".app-dtp-dropdown");

        this.trigger = wrapper.querySelector(".app-dtp-trigger");
        this.calendar = wrapper.querySelector(".app-dtp-calendar");

        this.current = new Date();

        this.init();
    }

    init() {

        this.trigger.addEventListener("click", () => {

            this.dropdown.style.display =
                this.dropdown.style.display === "none" ? "block" : "none";

            this.render();

        });

        this.render();
    }

    render() {

        this.calendar.innerHTML = "";

        let year = this.current.getFullYear();
        let month = this.current.getMonth();

        let first = new Date(year, month, 1).getDay();
        let days = new Date(year, month + 1, 0).getDate();

        for (let i = 0; i < first; i++) {

            let empty = document.createElement("div");
            this.calendar.appendChild(empty);

        }

        for (let d = 1; d <= days; d++) {

            let cell = document.createElement("div");

            cell.className = "app-dtp-day";
            cell.innerText = d;

            cell.onclick = () => {

                let date = new Date(year, month, d);

                this.hidden.value = date.toISOString();
                this.display.value = date.toLocaleDateString();

                this.dropdown.style.display = "none";

            };

            this.calendar.appendChild(cell);
        }

    }

}

document.querySelectorAll(".app-datetimepicker-wrapper")
    .forEach(w => new AppDateTimePicker(w));





// wwwroot/js/app-datetimepicker-pro.js

class AppDateTimePickerPro {

    constructor(wrapper) {

        this.wrapper = wrapper
        this.display = wrapper.querySelector(".app-dtp-display")
        this.displayEnd = wrapper.querySelector(".app-dtp-display-end")

        this.hiddenStart = wrapper.querySelector("input[type=hidden]")
        this.hiddenEnd = wrapper.querySelector("input[type=hidden][name$='End']")

        this.dropdown = wrapper.querySelector(".app-dtp-dropdown")
        this.calendar = wrapper.querySelector(".app-dtp-calendar")

        this.btnPrev = wrapper.querySelector(".app-dtp-prev")
        this.btnNext = wrapper.querySelector(".app-dtp-next")

        this.current = new Date()
        this.startDate = null
        this.endDate = null

        this.dragging = false

        this.init()
    }

    init() {

        this.wrapper.querySelector(".app-dtp-trigger")
            .addEventListener("click", () => this.toggle())

        document.addEventListener("click", (e) => {
            if (!this.wrapper.contains(e.target)) {
                this.close()
            }
        })

        if (this.btnPrev) {
            this.btnPrev.onclick = () => this.changeMonth(-1)
        }

        if (this.btnNext) {
            this.btnNext.onclick = () => this.changeMonth(1)
        }

        this.initKeyboard()

        this.render()

    }

    toggle() {

        this.dropdown.style.display =
            this.dropdown.style.display === "block" ? "none" : "block"

        this.autoPosition()

    }

    close() {
        this.dropdown.style.display = "none"
    }

    autoPosition() {

        let rect = this.wrapper.getBoundingClientRect()

        if (window.innerHeight - rect.bottom < 320) {
            this.dropdown.style.top = "auto"
            this.dropdown.style.bottom = "100%"
        }
    }

    changeMonth(step) {

        this.current.setMonth(this.current.getMonth() + step)
        this.render()

    }

    render() {

        this.calendar.innerHTML = ""

        let year = this.current.getFullYear()
        let month = this.current.getMonth()

        let first = new Date(year, month, 1).getDay()
        let days = new Date(year, month + 1, 0).getDate()

        for (let i = 0; i < first; i++) {
            let empty = document.createElement("div")
            this.calendar.appendChild(empty)
        }

        for (let d = 1; d <= days; d++) {

            let date = new Date(year, month, d)

            let cell = document.createElement("div")
            cell.className = "app-dtp-day"
            cell.innerText = d

            cell.dataset.date = date.toISOString()

            cell.onmousedown = () => {

                this.dragging = true
                this.selectDate(date, true)

            }

            cell.onmouseover = () => {

                if (this.dragging) {
                    this.selectDate(date, false)
                }

            }

            cell.onmouseup = () => {

                this.dragging = false

            }

            cell.onclick = () => {

                this.selectDate(date, true)

            }

            this.calendar.appendChild(cell)

        }

        this.highlight()

    }

    selectDate(date, firstClick) {

        if (!this.startDate || firstClick) {

            this.startDate = date
            this.endDate = null

        } else {

            if (date < this.startDate) {
                this.endDate = this.startDate
                this.startDate = date
            } else {
                this.endDate = date
            }

        }

        this.updateInputs()
        this.highlight()

    }

    highlight() {

        let cells = this.calendar.querySelectorAll(".app-dtp-day")

        cells.forEach(c => {

            let d = new Date(c.dataset.date)

            c.classList.remove("selected", "in-range")

            if (this.startDate && d.toDateString() === this.startDate.toDateString()) {
                c.classList.add("selected")
            }

            if (this.endDate && d.toDateString() === this.endDate.toDateString()) {
                c.classList.add("selected")
            }

            if (this.startDate && this.endDate && d > this.startDate && d < this.endDate) {
                c.classList.add("in-range")
            }

        })

    }

    updateInputs() {

        if (this.startDate) {

            this.hiddenStart.value = this.startDate.toISOString()
            this.display.value = this.format(this.startDate)

        }

        if (this.endDate && this.hiddenEnd) {

            this.hiddenEnd.value = this.endDate.toISOString()

            if (this.displayEnd) {
                this.displayEnd.value = this.format(this.endDate)
            }

        }

    }

    format(date) {

        return date.getFullYear() + "/" +
            String(date.getMonth() + 1).padStart(2, "0") + "/" +
            String(date.getDate()).padStart(2, "0")

    }

    initKeyboard() {

        document.addEventListener("keydown", (e) => {

            if (this.dropdown.style.display !== "block") return

            switch (e.key) {

                case "Escape":
                    this.close()
                    break

                case "ArrowLeft":
                    this.moveDay(-1)
                    break

                case "ArrowRight":
                    this.moveDay(1)
                    break

                case "ArrowUp":
                    this.moveDay(-7)
                    break

                case "ArrowDown":
                    this.moveDay(7)
                    break

            }

        })

    }

    moveDay(step) {

        if (!this.startDate) {
            this.startDate = new Date()
        }

        this.startDate.setDate(this.startDate.getDate() + step)

        this.updateInputs()
        this.render()

    }

}

function initDateTimePickers() {

    document.querySelectorAll(".app-datetimepicker-wrapper")
        .forEach(el => {

            if (!el.dataset.initialized) {
                new AppDateTimePickerPro(el)
                el.dataset.initialized = true
            }

        })

}

document.addEventListener("DOMContentLoaded", initDateTimePickers)

document.body.addEventListener("htmx:afterSwap", initDateTimePickers)



