using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Windows.Forms;

namespace ShipRate.WebApp.TagHelpers
{
    [HtmlTargetElement("form-lookup")]
    public class FormLookupTagHelper : TagHelper
    {
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        public string Label { get; set; }
        public string SearchUrl { get; set; }
        public string Placeholder { get; set; } = "Type to search...";
        public string ValueField { get; set; } = "localName";
        public string TextField { get; set; } = "display";
        public string DisplayValue { get; set; }
        public int MinChars { get; set; } = 2;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var name = For?.Name ?? "lookup";
            var id = name.Replace(".", "_");
            var currentValue = For?.Model?.ToString() ?? "";
            var currentDisplay = DisplayValue ?? "";

            output.TagName = null;
            output.Content.SetHtmlContent($@"
                    <div class=""mb-3 lookup-wrapper"" id=""wrap_{id}"">
                        {(string.IsNullOrEmpty(Label) ? "" : $"<label class=\"form-label\">{Label}</label>")}
                        <input type=""text"" class=""form-control lookup-input"" id=""txt_{id}"" placeholder=""{Placeholder}"" autocomplete=""off"" value=""{currentDisplay}"" />
                        <input type=""hidden"" name=""{name}"" id=""val_{id}"" value=""{currentValue}"" />
                        <div class=""lookup-results"" id=""res_{id}""></div>
                    </div>" + @"
<style>
.lookup-wrapper {position: relative;
}

.lookup-results {display: none;
    position: absolute;
    left: 0;
    right: 0;
    z-index: 1000;
    max-height: 250px;
    overflow-y: auto;
    background: #fff;
    border: 1px solid #ccc;
    border-top: none;
    border-radius: 0 0 6px 6px;
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.lookup-item {padding: 8px 12px;
    cursor: pointer;
}

.lookup-item:hover {
        background-color: silver;
    }

.lookup-item.active {background - color: silver;
        color: #fff;
    }

.lookup-no-results {padding: 8px 12px;
    color: #999;
    text-align: center;
}

</style>
"+
$@"
<script>
function initLookup(cfg) {{
    var input = document.getElementById(cfg.inputId);
    var hidden = document.getElementById(cfg.hiddenId);
    var results = document.getElementById(cfg.resultsId);
    var timer = null;
    var activeIndex = -1;
    var items = [];

    // type in input
    input.addEventListener('input', function () {{
        var term = input.value.trim();
        hidden.value = '';
        clearTimeout(timer);

        if (term.length < cfg.minChars) {{
            close();
            return;
        }}

        timer = setTimeout(function () {{
            search(term);
        }}, 300);
    }});

    // keyboard
    input.addEventListener('keydown', function (e) {{
        if (e.key === 'ArrowDown') {{
            e.preventDefault();
            move(1);
        }} else if (e.key === 'ArrowUp') {{
            e.preventDefault();
            move(-1);
        }} else if (e.key === 'Enter') {{
            e.preventDefault();
            select(activeIndex);
        }} else if (e.key === 'Escape') {{
            close();
        }}
    }});

    // close when click outside
    document.addEventListener('click', function (e) {{
        if (!document.getElementById(cfg.wrapperId).contains(e.target)) {{
            close();
        }}
    }});

    function search(term) {{
        fetch(cfg.url + '?term=' + encodeURIComponent(term))
            .then(function (r) {{ return r.json(); }})
            .then(function (data) {{
                items = Array.isArray(data) ? data : [];
                render();
            }});
    }}

    function render() {{
        activeIndex = -1;
        results.innerHTML = '';

        if (items.length === 0) {{
            results.innerHTML = '<div class=""lookup-no-results"">No results</div>';
            results.style.display = 'block';
            return;
        }}

        for (var i = 0; i < items.length; i++) {{
            var div = document.createElement('div');
            div.className = 'lookup-item';
            div.textContent = items[i][cfg.textField];
            div.setAttribute('data-index', i);
            div.addEventListener('mousedown', function (e) {{
                e.preventDefault();
                select(parseInt(this.getAttribute('data-index')));
            }});
            results.appendChild(div);
        }}

        results.style.display = 'block';
    }}

    function move(dir) {{
        var divs = results.querySelectorAll('.lookup-item');
        if (divs.length === 0) return;

        activeIndex += dir;
        if (activeIndex < 0) activeIndex = 0;
        if (activeIndex >= divs.length) activeIndex = divs.length - 1;

        divs.forEach(function (d) {{ d.classList.remove('active'); }});
        divs[activeIndex].classList.add('active');
    }}

    function select(index) {{
        if (index < 0 || index >= items.length) return;
        input.value = items[index][cfg.textField];
        hidden.value = items[index][cfg.valueField];
        close();
    }}

    function close() {{
        results.style.display = 'none';
        results.innerHTML = '';
        activeIndex = -1;
    }}
}}


    initLookup({{
        wrapperId: 'wrap_{id}',
        inputId: 'txt_{id}',
        hiddenId: 'val_{id}',
        resultsId: 'res_{id}',
        url: '{SearchUrl}',
        valueField: '{ValueField}',
        textField: '{TextField}',
        minChars: {MinChars}
    }});
</script>");
        }
    }
}