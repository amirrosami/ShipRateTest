using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ShipRate.WebApp.TagHelpers
{
    [HtmlTargetElement("auth-policy", Attributes = "policy")]
    public class AuthPolicyTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthPolicyTagHelper(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HtmlAttributeName("policy")]
        public string Policy { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = ViewContext.HttpContext.User;

            var result = await _authorizationService.AuthorizeAsync(user, Policy);

            if (!result.Succeeded)
            {
                output.SuppressOutput();
            }
        }
    }
}