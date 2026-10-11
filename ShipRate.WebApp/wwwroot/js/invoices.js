(function () {
    window.openInvoiceModal = function () {
        const modalEl = document.getElementById('invoiceModal');
        if (!modalEl) return;
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();
    };

    window.openPartyLookupModal = function () {
        const modalEl = document.getElementById('partyLookupModal');
        if (!modalEl) return;
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();
    };

    //window.selectParty = function (id, name) {
    //    const partyIdInput = document.querySelector("input[name='PartyId']");
    //    const partyNameInput = document.querySelector("input[name='PartyName']");



    //    if (partyIdInput) partyIdInput.value = id;
    //    if (partyNameInput) partyNameInput.value = name;

    //    const modalEl = document.getElementById('partyLookupModal');
    //    if (modalEl) {
    //        const modal = bootstrap.Modal.getInstance(modalEl);
    //        if (modal) modal.hide();
    //    }
    //};

    window.selectParty = function (id, name) {
        const partyIdInput = document.querySelector("input[name='PartyId']");
        const partyDisplay = document.getElementById("PartyNameDisplay");

        if (partyIdInput) partyIdInput.value = id;
        if (partyDisplay) partyDisplay.textContent = name;

        const modalEl = document.getElementById('partyLookupModal');
        if (modalEl) {
            const modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) modal.hide();
        }
    };


    window.clearSelectedParty = function () {
        const partyIdInput = document.querySelector("input[name='PartyId']");
        const partyNameInput = document.querySelector("input[name='PartyName']");

        if (partyIdInput) partyIdInput.value = "";
        if (partyNameInput) partyNameInput.value = "";
    };

    function parseDecimal(value) {
        if (!value) return 0;
        value = value.toString().replace(/,/g, '');
        const parsed = parseFloat(value);
        return isNaN(parsed) ? 0 : parsed;
    }

    function formatNumber(value) {
        return value.toLocaleString(undefined, {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    function calculateItemPreview() {
        const qty = parseDecimal(document.querySelector("input[name='QTY']")?.value);
        const unitPrice = parseDecimal(document.querySelector("input[name='UnitPrice']")?.value);
        const discount = parseDecimal(document.querySelector("input[name='Discount']")?.value);
        const taxPercent = parseDecimal(document.querySelector("input[name='TaxPercent']")?.value);

        let beforeDiscount = qty * unitPrice;
        let afterDiscount = beforeDiscount - discount;
        if (afterDiscount < 0) afterDiscount = 0;
        let taxAmount = afterDiscount * taxPercent / 100;
        let totalAmount = afterDiscount + taxAmount;

        const beforeEl = document.getElementById("amountBeforeDiscountPreview");
        const afterEl = document.getElementById("amountAfterDiscountPreview");
        const taxEl = document.getElementById("taxAmountPreview");
        const totalEl = document.getElementById("totalAmountPreview");

        if (beforeEl) beforeEl.value = formatNumber(beforeDiscount);
        if (afterEl) afterEl.value = formatNumber(afterDiscount);
        if (taxEl) taxEl.value = formatNumber(taxAmount);
        if (totalEl) totalEl.value = formatNumber(totalAmount);
    }

    async function reloadChargeOptions(container) {
        const form = container.closest("form");
        if (!form) return;

        const chargeType = form.querySelector("select[name='ChargeTypeId']");
        const currency = form.querySelector("select[name='CurrencyId']");
        const charge = form.querySelector("select[name='ChargeId']");
        const financeDirectionInput = document.querySelector("select[name='FinanceDirectionId']");
        const invoiceFinanceDirection = financeDirectionInput ? financeDirectionInput.value : "";

        if (!charge) return;

        const selectedChargeId = charge.value || charge.getAttribute("data-selected-charge-id") || "";
        const url = new URL('/Invoices/ChargesByFilters', window.location.origin);
        if (currency && currency.value) url.searchParams.append('currencyId', currency.value);
        if (chargeType && chargeType.value) url.searchParams.append('chargeTypeId', chargeType.value);
        if (invoiceFinanceDirection) url.searchParams.append('financeDirectionId', invoiceFinanceDirection);
        if (selectedChargeId) url.searchParams.append('selectedId', selectedChargeId);

        const response = await fetch(url.toString(), {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        if (!response.ok) return;

        const html = await response.text();
        charge.innerHTML = html;
    }

    document.body.addEventListener("input", function (e) {
        if (e.target.classList.contains("item-calc")) {
            calculateItemPreview();
        }
    });

    document.body.addEventListener("change", function (e) {
        if (e.target.classList.contains("item-charge-filter")) {
            reloadChargeOptions(e.target);
        }
    });

    document.body.addEventListener("htmx:afterSwap", function (e) {
        if (e.target && e.target.id === "invoice-item-form-container") {
            calculateItemPreview();
        }
    });


    document.body.addEventListener("click", function (e) {
        const btn = e.target.closest(".party-select-btn");
        if (!btn) return;

        const id = btn.getAttribute("data-party-id") || "";
        const name = btn.getAttribute("data-party-name") || "";

        window.selectParty(id, name);
    });


//    document.body.addEventListener("invoice-list-refresh", function () {
//        htmx.ajax('GET', '/Invoices/List', {
//            target: '#invoice-list-container',
//            swap: 'innerHTML'
//        });
    //    });



})();
