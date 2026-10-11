using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Text;

namespace ShipRate.WebApp.TagHelpers
{

    [HtmlTargetElement("search-panel")]
    public class SearchPanelTagHelper : TagHelper
    {
        public string AspAction { get; set; }
        public string AspController { get; set; }
        public object Model { get; set; }

        public override void Process(TagHelperContext ctx, TagHelperOutput output)
        {
            output.TagName = null;
           // output.Attributes.SetAttribute("method", "get");
           // output.Attributes.SetAttribute("action", AspAction);
           // output.Attributes.SetAttribute("controller", AspController);
            output.PreContent.SetHtmlContent("" +
                "<div class='card'>" +
                    "<div class='card-header'>" +
                        "<h3 class='card-title'>Search</h3>" +
                    "</div>" +
                    "<div class='card-body'> " +
                        $"<form action='/{AspController}/{AspAction}' asp-controller='{AspController}' method='get'> " +
                            "<div class='row'>");
            output.PostContent.SetHtmlContent(
                                "<div class='col-md-2'> " +
                                "   <div class='form-group'> " +
                                "       <label>&nbsp;</label>" +
                                "       <div> " +
                                "           <button type='submit' class='btn btn-primary'>Search</button>" +
                               // "           <a asp-action='Index' class='btn btn-secondary'>Clear</a> " +
                                "       </div>" +
                                "   </div>" +
                            "   </div>" +
                            " </div>  " +
                        "</form>" +
                     "</div>" +
                "</div>");
        }
    }

    [HtmlTargetElement("search-input")]
    public class SearchInputTagHelper : TagHelper
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public int Size { get; set; }
        public override void Process(TagHelperContext ctx, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", $"col-md-{Size}");

            output.Content.SetHtmlContent($@"
            <div class='form-group'>
                <label>{Label}</label>
                <input name='{Name}' asp-for='{Name}' class='form-control'/>
            </div>
        ");
        }
    }







    [HtmlTargetElement("grid-action", ParentTag = "grid-actions")]
    public class GridActionTagHelper : TagHelper
    {
        private const string ContextName = "GridContext";

        public string AspAction { get; set; }
        public string AspController { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context.Items.TryGetValue(ContextName, out var ctxObj)
                && ctxObj is GridContext gridContext)
            {
                gridContext.Actions.Add(new GridActionInfo
                {
                    AspAction = AspAction,
                    Icon = Icon,
                    Class = Class,
                    AspController = AspController
                });
            }

            output.SuppressOutput();
        }
    }



    [HtmlTargetElement("grid-actions", ParentTag = "data-grid")]
    public class GridActionsTagHelper : TagHelper
    {
        private const string ContextName = "GridContext";
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // get parent context
            if (context.Items.TryGetValue(ContextName, out var ctx))
            {
                // forward context to children
                context.Items[ContextName] = ctx;
            }

            // VERY IMPORTANT → execute nested grid-action
            await output.GetChildContentAsync();

            output.SuppressOutput();
        }
    }


    public class GridActionInfo
    {
        public string AspAction { get; set; }
        public string AspController { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
    }

    public class PaginationInfo
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int[] PageSizeOptions { get; set; }
    }
    public class GridContext
    {
        public List<GridColumnInfo> Columns { get; } = new();
        public List<GridActionInfo> Actions { get; } = new();
        public PaginationInfo PaginationInfo { get; set; }
        public string IdProperty { get; set; } = "Id";
    }



    [HtmlTargetElement("data-grid")]
    public class DataGridTagHelper : TagHelper
    {
        private const string ContextName = "GridContext";

        public IEnumerable Items { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string IdProperty { get; set; } = "Id";
        public string SortColumn { get; set; }
        public string SortDir { get; set; } = "Desc";
        public override void Init(TagHelperContext context)
        {
            context.Items[ContextName] = new GridContext
            {
                IdProperty = IdProperty
            };
        }

        public StringBuilder AddPagination(StringBuilder html, PaginationInfo pagination)
        {
            int current = pagination.PageNumber;
            int total = pagination.TotalPages;

            int start = Math.Max(1, current - 2);
            int end = Math.Min(total, current + 2);

            html.Append("<tfoot><tr><td colspan='100%'>");

            html.Append("<nav><ul class='pagination justify-content-center'>");

            // First
            //html.Append($"<li class='page-item {(current == 1 ? "disabled" : "")}'>" +
            //    $"<a class='page-link' href='?page=1'>«</a></li>");

            html.Append($"<li class='page-item {(current == 1 ? "disabled" : "")}'>" +
                   $"<button class='page-link' onclick='goPage(\"{Controller}\",\"{Action}\",{1})'>«</button></li>");

            // Prev
            //html.Append($"<li class='page-item {(current == 1 ? "disabled" : "")}'>" +
            //    $"<a class='page-link' href='?PageNumber={current - 1}'>‹</a></li>");

            html.Append($"<li class='page-item {(current == 1 ? "disabled" : "")}'>" +
                $"<button class='page-link' onclick='goPage(\"{Controller}\",\"{Action}\",{current - 1})'>‹</button></li>");


            // Ellipsis before
            if (start > 1)
                html.Append("<li class='page-item disabled'><span class='page-link'>...</span></li>");

            // Pages
            for (int i = start; i <= end; i++)
            {
                var active = i == current ? "active" : "";
                //html.Append($"<li class='page-item {active}'>" +
                //    $"<a class='page-link' href='?PageNumber={i}'>{i}</a></li>");

                html.Append($"<li class='page-item {active}'>" +
                    $"<button class='page-link' onclick='goPage(\"{Controller}\",\"{Action}\",{i})'>{i}</button></li>");
            }

            // Ellipsis after
            if (end < total)
                html.Append("<li class='page-item disabled'><span class='page-link'>...</span></li>");

            // Next
            //html.Append($"<li class='page-item {(current == total ? "disabled" : "")}'>" +
            //    $"<a class='page-link' href='?page={current + 1}'>›</a></li>");

            html.Append($"<li class='page-item {(current == total ? "disabled" : "")}'>" +
                $"<button class='page-link' onclick='goPage(\"{Controller}\",\"{Action}\",{current + 1})'>›</button></li>");



            // Last
            //html.Append($"<li class='page-item {(current == total ? "disabled" : "")}'>" +
            //  $"<a class='page-link' href='?page={total}'>»</a></li>");

            html.Append($"<li class='page-item {(current == total ? "disabled" : "")}'>" +
              $"<button class='page-link' onclick='goPage(\"{Controller}\",\"{Action}\",{total})'>»</button></li>");


            html.Append("<li>");
            string pagesizeDef = AddPageSize(pagination);
            html.Append(pagesizeDef);
            html.Append("</li>");

            html.Append("</ul>");



            html.Append("</nav></td></tr></tfoot>");

            return html;
        }

        private string AddPageSize(PaginationInfo pagination)
        {
            var html = new StringBuilder();
            html.Append("<div class='d-flex align-items-center'>");
            html.Append("<label class='me-2'>Page Size:</label>");

            html.Append("<select class='form-select form-select-sm' ");
            html.Append("style='width:90px' ");
            html.Append("onchange='changePageSize(this.value)'>");

            foreach (var s in pagination.PageSizeOptions)
            {
                var selected = s == pagination.PageSize ? "selected" : "";
                html.Append($"<option value='{s}' {selected}>{s}</option>");
            }

            html.Append("</select></div>");
            return html.ToString();
        }

        private string BuildSortUrl(string colName)
        {
            string sortDir = (SortColumn == colName && SortDir == "Desc") ? "Asc" : "Desc";
            string url = $"/{Controller}/{Action}?SortColumn={colName}&&SortDirection={sortDir}";
            return url;
        }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await output.GetChildContentAsync();

            var gridContext = (GridContext)context.Items[ContextName];
            var columns = gridContext.Columns;
            var actions = gridContext.Actions;
            var pagination = gridContext.PaginationInfo;

            output.TagName = "div";
            output.Attributes.SetAttribute("id", "gridContainer");
            //output.Attributes.SetAttribute("style", "table-layout:auto");
            int startNumber = 0;
            if (pagination != null)
            {
                startNumber = (pagination.PageNumber - 1) * pagination.PageSize + 1;
            }
            var html = new StringBuilder();

            html.Append("<table class='table table-hover'");
            // ================= HEADER =================
            html.Append("<thead class='table-info'><tr>");

            foreach (var col in columns)
            {
                if (col.Sortable)
                {

                    //html.Append($"<th> <a class='text-decoration-none text-dark' href={BuildSortUrl(col.Property)}> {col.Title}");
                    html.Append($"<th> <a class='text-decoration-none text-dark' href = 'javascript:void(0)' onclick = 'sortGrid(\"{Controller}\",\"{Action}\",\"{col.Property}\" ,\"{SortDir}\")'>");

                    if (SortColumn == col.Property)
                    {
                        if (SortDir == "desc")
                        {
                            html.Append("▼ ");
                        }
                        else
                        {
                            html.Append("▲ ");
                        }
                    }
                    html.Append(col.Title);
                    html.Append("</a></td>");
                }
                else
                {
                    html.Append($"<th>{col.Title}</th>");
                }
            }
            if (actions.Any())
                html.Append("<th>Operations</th>");

            html.Append("</tr></thead>");

            // ================= BODY =================
            html.Append("<tbody>");

            if (Items == null || !Items.Cast<object>().Any())
            {
                html.Append($"<tr><td colspan='{columns.Count + (actions.Any() ? 1 : 0)}' class='text-center'>No Records found!</td></tr>");
            }
            else
            {
                int rowIndex = 0;
                foreach (var item in Items)
                {
                    html.Append("<tr>");

                    foreach (var col in columns)
                    {
                        var value = GetNestedPropertyValue(item, col.Property);

                        if (col.IsRowNumber)
                        {
                            html.Append($"<td>{startNumber + rowIndex}</td>");
                        }
                        else if (col.Type == "bool")
                        {
                            var badge = (value is bool b && b)
                                ? $"<span class='badge bg-success'>{col.TrueText}</span>"
                                : $"<span class='badge bg-danger'>{col.FalseText}</span>";

                            html.Append($"<td>{badge}</td>");
                        }
                        else
                        {
                            html.Append($"<td>{value}</td>");
                        }
                    }

                    // ===== ACTIONS =====
                    if (actions.Any())
                    {
                        var id = GetNestedPropertyValue(item, gridContext.IdProperty);

                        html.Append("<td>");

                        foreach (var act in actions)
                        {
                            var url = $"/{act.AspController}/{act.AspAction}/{id}";
                            html.Append($@"
                            <a href='{url}' class='action-btn'>
                                <i class='bi bi-{act.Icon}-fill {act.Class}'></i>
                            </a> ");
                        }

                        html.Append("</td>");
                    }

                    html.Append("</tr>");
                    rowIndex++;
                }
            }

            html.Append("</tbody>");
            if (pagination != null)
            {

                html = AddPagination(html, pagination);
            }

            html.Append("</table>");
            output.Content.SetHtmlContent(html.ToString());
        }

        private object GetNestedPropertyValue(object obj, string path)
        {
            foreach (var part in path.Split('.'))
            {
                if (obj == null) return "";
                var prop = obj.GetType().GetProperty(part);
                obj = prop?.GetValue(obj);
            }
            return obj ?? "";
        }
    }


    public class GridColumnInfo
    {
        public string Property { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public bool IsRowNumber { get; set; } = false;
        public string TrueText { get; set; }
        public string FalseText { get; set; }
        public bool Sortable { get; set; }

    }


    [HtmlTargetElement("grid-column", ParentTag = "data-grid")]
    public class GridColumnTagHelper : TagHelper
    {
        private const string ContextName = "GridContext";

        public string Property { get; set; } = "";
        public string Title { get; set; }
        public string Type { get; set; }
        public bool IsRowNumber { get; set; } = false;
        public string TrueText { get; set; } = "True";
        public string FalseText { get; set; } = "False";
        public bool Sortable { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context.Items.TryGetValue(ContextName, out var ctxObj)
                && ctxObj is GridContext gridContext)
            {
                gridContext.Columns.Add(new GridColumnInfo
                {
                    Property = Property,
                    Title = Title,
                    Type = Type,
                    IsRowNumber = IsRowNumber,
                    TrueText = TrueText,
                    FalseText = FalseText,
                    Sortable = Sortable
                });
            }

            output.SuppressOutput();
        }
    }


    [HtmlTargetElement("submit-button")]
    public class SubmitButtonTagHelper : TagHelper
    {
        public string Value { get; set; }
        public string Class { get; set; }

        public override void Process(TagHelperContext ctx, TagHelperOutput output)
        {
            //< div class="col-md-4">
            //                <div class="form-group">
            //                    <label>&nbsp;</label>
            //                    <div>
            //                        <button type = "submit" class="btn btn-primary">
            //                            <i class="fas fa-search"></i> جستجو
            //                        </button>
            //                        <a href = "@Url.Action("Index")" class="btn btn-secondary">
            //                            <i class="fas fa-times"></i> پاک کردن
            //                        </a>
            //                        <a href = "@Url.Action("Create")" class="btn btn-success">
            //                            <i class="fas fa-plus"></i> ثبت کالای جدید
            //                        </a>
            //                    </div>
            //                </div>
            //            </div>
            output.TagName = "button";
            output.Attributes.SetAttribute("class", $"{Class}");
            output.Content.SetContent($"{Value}");
        }
    }


    [HtmlTargetElement("action-button")]
    public class ActionButtonTagHelper : TagHelper
    {
        public string AspAction { get; set; }
        public string Value { get; set; }
        public string Class { get; set; }

        public override void Process(TagHelperContext ctx, TagHelperOutput output)
        {
            //< div class="col-md-4">
            //                <div class="form-group">
            //                    <label>&nbsp;</label>
            //                    <div>
            //                        <button type = "submit" class="btn btn-primary">
            //                            <i class="fas fa-search"></i> جستجو
            //                        </button>
            //                        <a href = "@Url.Action("Index")" class="btn btn-secondary">
            //                            <i class="fas fa-times"></i> پاک کردن
            //                        </a>
            //                        <a href = "@Url.Action("Create")" class="btn btn-success">
            //                            <i class="fas fa-plus"></i> ثبت کالای جدید
            //                        </a>
            //                    </div>
            //                </div>
            //            </div>
            output.TagName = "a";
            output.Attributes.SetAttribute("class", $"{Class}");
            output.Content.SetContent($"{Value}");
        }
    }
    [HtmlTargetElement("alert-messages")]
    public class AlertMessagesTagHelper : TagHelper
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public override void Process(TagHelperContext ctx, TagHelperOutput output)
        {
            output.TagName = "div";
            if (!string.IsNullOrEmpty(Value))
            {
                output.Content.SetHtmlContent($"<div class='alert alert-{Type}'>{Value}</div>");
            }
            else
            {
                output.Content.SetHtmlContent("");
            }
        }
    }


    [HtmlTargetElement("grid-pagination", ParentTag = "data-grid")]
    public class GridPaginationTagHelper : TagHelper
    {
        public int PageNumber { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int[] PageSizeOptions { get; set; } = { 10, 15, 20, 30, 50 };
        public int TotalPages { get; set; }
        private const string ContextName = "GridContext";
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            if (context.Items.TryGetValue(ContextName, out var ctxObj)
                && ctxObj is GridContext gridContext)
            {
                gridContext.PaginationInfo = new PaginationInfo
                {
                    PageNumber = PageNumber,
                    TotalPages = TotalPages,
                    TotalCount = TotalCount,
                    PageSize = PageSize,
                    PageSizeOptions = PageSizeOptions
                };
            }

            output.SuppressOutput();
        }
    }



}
