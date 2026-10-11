using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace ShipRate.WebApp.TagHelpers
{

    [HtmlTargetElement("custom-form")]
    public class CustomFormTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHtmlGenerator _generator;

        public CustomFormTagHelper(IUrlHelperFactory urlHelperFactory,
                                   IHtmlGenerator generator)
        {
            _urlHelperFactory = urlHelperFactory;
            _generator = generator;
        }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("title")]
        public string Title { get; set; } = "";
        public string Method { get; set; } = "post";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            var actionUrl = urlHelper.Action(Action, Controller);
            output.TagName = null;
            output.PreContent.SetHtmlContent("<div class='card'>" +
                    "<div class='card-header'>" +
                       $"<h3 class='card-title'>{Title}</h3>" +
                    "</div>" +
                    "<div class='card-body'> " +
                        $"<form asp-controller='{Controller}' asp-action='{Action}' method='post'> "
                 + "<div class='row g-3'>");

            output.PostContent.SetHtmlContent("</div></form></div></div>");
        }
    }



    [HtmlTargetElement("custom-form-input")]
    public class CustomFormInputTagHelper : TagHelper
    {
        public ModelExpression For { get; set; }
        public string Label { get; set; }
        public string RowOrCol { get; set; } = "col";
        public string Type { get; set; } = "text";
        public int Size { get; set; } = 12;
        public string Placeholder { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var name = For.Name;
            //var value = For.Model?.ToString() ?? "";

            var sb = new StringBuilder();

            if (RowOrCol == "row")
            {
                sb.Append(
                $"<div class=\"row mb-2 g-2\">" +
                $"      <div class='col-sm-{Size}'    " +
                $"           <label for=\"{name}\" class=\"form-label mb-2\">{Label}</label>  " +
                $"           <input name='{name}' type=\"{Type}\" class=\"form-control\" id=\"{name}\">" +
                "      </div>" +
                "</div>"
                );
            }
            else
            {
                sb.Append($"<div class=\"col-sm-{Size}\">" +
                    $"           <label for=\"{name}\" class=\"form-label\">{Label}</label>  " +
                    $"           <input name='{name}' type=\"{Type}\" class=\"form-control\" id=\"{name}\">" +
                    "      </div>");
            }
            //sb.AppendLine($"<div class=\"col-12 col-md-{Col} mb-3\">");
            //sb.AppendLine("<div class='form-group'>");
            //sb.AppendLine($"<label class=\"form-label\">{name}</label>");
            //sb.AppendLine($"<input name=\"{name}\" value=\"{value}\" type=\"{Type}\" class=\"form-control\" placeholder=\"{Placeholder}\" />");
            //sb.AppendLine($"<span asp-validation-for=\"{name}\" class=\"text-danger small\"></span>");
            //sb.AppendLine("</div></div>");

            output.TagName = null;
            output.Content.SetHtmlContent(sb.ToString());
        }
    }


    [HtmlTargetElement("custom-checkbox")]
    public class CustomCheckboxTagHelper : TagHelper
    {
        public ModelExpression For { get; set; }
        public int Col { get; set; } = 12;
        public string Label { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var name = For.Name;
            var isChecked = (For.Model is bool b && b) ? "checked" : "";

            var sb = new StringBuilder();
            sb.Append(
                $"<div class='col-4'>" +
                "            <div class=\"form-check\">     " +
                $"           <label class=\"form-check-label\" for='{name}'>{Label}</label>" +
                $"           <input class=\"form-check-input\" style='height:20px;width:20px' asp-for='{name}' name='{name}' type=\"checkbox\" id='{name}' {isChecked}>" +
                $"              " +
                "            </div>  " +
                "     </div>"
                );

            //sb.AppendLine($"<div class=\"col-12 col-md-{Col} mb-1 form-check\">");
            //sb.AppendLine("<div class='form-group'>");
            //sb.AppendLine($"<input type=\"checkbox\" name=\"{name}\" class=\"custom-control custom-checkbox\" {isChecked} />");
            //sb.AppendLine($"<label class=\"form-check-label\">{name}</label>");
            //sb.AppendLine("</div></div>");

            output.TagName = null;
            output.Content.SetHtmlContent(sb.ToString());
        }
    }



    [HtmlTargetElement("custom-radio", Attributes = "asp-for")]
    public class CustomRadioTagHelper : TagHelper
    {
        public ModelExpression For { get; set; }
        public IEnumerable<string> Options { get; set; }
        public int Col { get; set; } = 12;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var name = For.Name;
            var selected = For.Model?.ToString();

            var sb = new StringBuilder();

            sb.AppendLine($"<div class=\"col-12 col-md-{Col} mb-3\">");
            sb.AppendLine($"<label class=\"form-label\">{name}</label>");

            foreach (var opt in Options)
            {
                var isChecked = opt == selected ? "checked" : "";

                sb.AppendLine("<div class='form-check'>");
                sb.AppendLine($"<input type='radio' name='{name}' value='{opt}' class='form-check-input' {isChecked} />");
                sb.AppendLine($"<label class='form-check-label'>{opt}</label>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");

            output.TagName = null;
            output.Content.SetHtmlContent(sb.ToString());
        }
    }

    [HtmlTargetElement("custom-button")]
    public class CustomButtonTagHelper : TagHelper
    {
        public string Text { get; set; } = "Submit";
        public string Type { get; set; } = "submit";
        public string Color { get; set; } = "primary";
        public int Col { get; set; } = 12;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var sb = new StringBuilder();
            sb.Append($"" +
                $"       <button type=\"{Type}\" class=\"btn btn-{Color}\">{Text}</button>" +
                $"     ");



            //sb.AppendLine($"<div class=\"col-12 col-md-{Col} mb-2\">");
            //sb.AppendLine($"<button type=\"{Type}\" class=\"btn btn-{Color} w-100\">{Text}</button>");
            //sb.AppendLine("</div>");

            output.TagName = null;
            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
