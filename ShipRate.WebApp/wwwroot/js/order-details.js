// wwwroot/js/order-details.js

$(document).ready(function () {

    // Get current highest index
    function getNextIndex() {
        let maxIndex = -1;
        $('#detailsBody tr.detail-row').each(function () {
            const index = parseInt($(this).data('index')) || 0;
            if (index > maxIndex) maxIndex = index;
        });
        return maxIndex + 1;
    }

    // Calculate line total for a row
    function calculateLineTotal($row) {
        const quantity = parseFloat($row.find('.quantity').val()) || 0;
        const unitPrice = parseFloat($row.find('.unit-price').val()) || 0;
        const discount = parseFloat($row.find('.discount').val()) || 0;

        const lineTotal = quantity * unitPrice * (1 - discount / 100);
        $row.find('.line-total').val(formatCurrency(lineTotal));

        return lineTotal;
    }

    // Calculate grand total
    function calculateGrandTotal() {
        let total = 0;
        $('#detailsBody tr.detail-row:visible').each(function () {
            total += calculateLineTotal($(this));
        });
        $('#grandTotal').text(formatCurrency(total));
    }

    // Format as currency
    function formatCurrency(amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    }

    // Add new row
    $('#btnAddRow').on('click', function () {
        const template = document.getElementById('rowTemplate');
        const newIndex = getNextIndex();

        let newRow = template.innerHTML.replace(/__INDEX__/g, newIndex);
        $('#detailsBody').append(newRow);

        // Focus on the product dropdown of new row
        $(`#detailsBody tr[data-index="${newIndex}"] .product-select`).focus();

        calculateGrandTotal();
    });

    // Remove row (soft delete for existing, hard delete for new)
    $(document).on('click', '.btn-remove-row', function () {
        const $row = $(this).closest('tr');
        const id = $row.find('input[name$=".Id"]').val();

        if (id && parseInt(id) > 0) {
            // Existing record - soft delete
            $row.find('.is-deleted').val('true');
            $row.addClass('d-none');
        } else {
            // New record - hard delete
            $row.remove();
        }

        calculateGrandTotal();
    });

    // Product selection change - fetch price
    $(document).on('change', '.product-select', function () {
        const $row = $(this).closest('tr');
        const productId = $(this).val();
        const productName = $(this).find('option:selected').text();

        // Update hidden product name field
        $row.find('.product-name').val(productName);

        if (productId) {
            // Fetch product details via AJAX
            $.ajax({
                url: '/Orders/GetProductDetails',
                type: 'GET',
                data: { productId: productId },
                success: function (data) {
                    $row.find('.unit-price').val(data.price);
                    $row.find('.product-name').val(data.name);
                    calculateLineTotal($row);
                    calculateGrandTotal();
                },
                error: function () {
                    console.error('Failed to fetch product details');
                }
            });
        }
    });

    // Recalculate on input change
    $(document).on('input', '.quantity, .unit-price, .discount', function () {
        const $row = $(this).closest('tr');
        calculateLineTotal($row);
        calculateGrandTotal();
    });

    // Form validation before submit
    $('#orderForm').on('submit', function (e) {
        const visibleRows = $('#detailsBody tr.detail-row:visible');

        if (visibleRows.length === 0) {
            e.preventDefault();
            alert('Please add at least one order item.');
            return false;
        }

        // Validate each visible row
        let isValid = true;
        visibleRows.each(function () {
            const $row = $(this);
            const productId = $row.find('.product-select').val();
            const quantity = $row.find('.quantity').val();
            const unitPrice = $row.find('.unit-price').val();

            if (!productId || !quantity || !unitPrice) {
                isValid = false;
                $row.addClass('table-danger');
            } else {
                $row.removeClass('table-danger');
            }
        });

        if (!isValid) {
            e.preventDefault();
            alert('Please fill in all required fields for each order item.');
            return false;
        }

        return true;
    });

    // Initial calculation
    calculateGrandTotal();
});
