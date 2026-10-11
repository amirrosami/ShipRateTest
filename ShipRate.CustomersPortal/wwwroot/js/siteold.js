// Sidebar Toggle
$(document).ready(function () {
    $('#sidebarToggle').on('click', function () {
        $('#sidebar').toggleClass('active');
        $('.main-content').toggleClass('active');
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Active menu item
    var path = window.location.pathname;
    $('.sidebar .nav-link').each(function () {
        var href = $(this).attr('href');
        if (path.indexOf(href) !== -1) {
            $(this).addClass('active');
        }
    });

    // Confirm delete
    $('.delete-confirm').on('click', function (e) {
        if (!confirm('آیا از حذف این مورد اطمینان دارید؟')) {
            e.preventDefault();
        }
    });

    // Number formatting
    $('.number-format').each(function () {
        var num = $(this).text();
        if (num) {
            $(this).text(Number(num).toLocaleString('fa-IR'));
        }
    });
});

// Sort table function
function sortTable(column) {
    var urlParams = new URLSearchParams(window.location.search);
    var currentSort = urlParams.get('sortColumn');
    var currentDirection = urlParams.get('sortDirection');

    var newDirection = 'asc';
    if (currentSort === column && currentDirection === 'asc') {
        newDirection = 'desc';
    }

    urlParams.set('sortColumn', column);
    urlParams.set('sortDirection', newDirection);
    urlParams.set('pageNumber', '1'); // Reset to first page

    window.location.search = urlParams.toString();
}

// Search with debounce
let searchTimeout;
function performSearch() {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function () {
        $('#searchForm').submit();
    }, 500);
}

// Clear search
function clearSearch() {
    $('#searchTerm').val('');
    $('#countryFilter').val('');
    $('#searchForm').submit();
}

// Format numbers as Persian
function formatNumber(num) {
    if (!num) return '0';
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
        .replace(/0/g, '۰')
        .replace(/1/g, '۱')
        .replace(/2/g, '۲')
        .replace(/3/g, '۳')
        .replace(/4/g, '۴')
        .replace(/5/g, '۵')
        .replace(/6/g, '۶')
        .replace(/7/g, '۷')
        .replace(/8/g, '۸')
        .replace(/9/g, '۹');
}

// Show loading spinner
function showLoading() {
    $('body').append('<div class="loading-overlay"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
}

function hideLoading() {
    $('.loading-overlay').remove();
}

// Form validation helper
function validateForm(formId) {
    var form = document.getElementById(formId);
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return false;
    }
    return true;
}

// AJAX helper for getting cities by country
function loadCitiesByCountry(countryId, targetSelectId) {
    if (!countryId) {
        $(targetSelectId).html('<option value="">ابتدا کشور را انتخاب کنید</option>');
        return;
    }

    $.ajax({
        url: '/City/GetCitiesByCountry',
        type: 'GET',
        data: { countryId: countryId },
        success: function (data) {
            var options = '<option value="">انتخاب کنید</option>';
            $.each(data, function (index, item) {
                options += '<option value="' + item.id + '">' + item.name + '</option>';
            });
            $(targetSelectId).html(options);
        },
        error: function () {
            alert('خطا در بارگذاری شهرها');
        }
    });
}
