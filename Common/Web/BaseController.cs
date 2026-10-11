using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipRate.Common.Web
{
    public class BaseController : Controller
    {
        protected readonly UserService _userService;
        public BaseController(UserService userService)
        {
           _userService = userService;
        }
        protected string CurrentUserId()
        {
           return _userService.GetUserId();
        }
        protected string CurrentUserName()
        {
            return _userService.GetUsername();
        }
    }
}
