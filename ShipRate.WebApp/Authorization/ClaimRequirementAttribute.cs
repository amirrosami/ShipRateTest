using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShipRate.Web.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ClaimRequirementAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _claimType;
        private readonly string _claimValue;

        /// <summary>
        /// سازنده Attribute
        /// </summary>
        /// <param name="claimType">نوع Claim (مثلاً "Permission")</param>
        /// <param name="claimValue">مقدار Claim (مثلاً "Country.Create")</param>
        public ClaimRequirementAttribute(string claimType, string claimValue)
        {
            _claimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
            _claimValue = claimValue ?? throw new ArgumentNullException(nameof(claimValue));
        }

        /// <summary>
        /// بررسی دسترسی کاربر
        /// </summary>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // ✅ بررسی احراز هویت
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // ✅ بررسی وجود Claim
            var hasClaim = context.HttpContext.User.HasClaim(_claimType, _claimValue);

            if (!hasClaim)
            {
                // ✅ برای درخواست‌های AJAX → 403 Forbidden
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new JsonResult(new
                    {
                        success = false,
                        message = "شما دسترسی لازم برای انجام این عملیات را ندارید"
                    })
                    {
                        StatusCode = 403
                    };
                }
                else
                {
                    // ✅ برای درخواست‌های عادی → Redirect به صفحه AccessDenied
                    context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                }
            }
        }

        /// <summary>
        /// بررسی اینکه آیا درخواست AJAX است یا نه
        /// </summary>
        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   request.Headers["Accept"].ToString().Contains("application/json");
        }
    }
}

