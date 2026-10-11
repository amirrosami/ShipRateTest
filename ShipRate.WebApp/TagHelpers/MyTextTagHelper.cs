using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ShipRate.WebApp.TagHelpers
{
    [HtmlTargetElement("mytext")]
    public class MyTextTagHelper:TagHelper
    {
      
            [HtmlAttributeName("text")]
            public string? Text { get; set; }

            [HtmlAttributeName("color")]
            public string? Color { get; set; } = "steelblue";

            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                output.TagName = "div"; // renders as <div>
                output.Attributes.SetAttribute("style", $"color:{Color};font-weight:bold;");
                output.Content.SetContent(Text ?? "Hello from TestTagHelper!");
            }
    }
}



