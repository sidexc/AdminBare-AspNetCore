﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SideXC.WebUI.Controllers
{
    [AllowAnonymous]
    public class AspNetCoreController : BaseController
    {
        [Authorization]
        public IActionResult Welcome() => View();                
        public IActionResult Interactive() => View();
        public IActionResult Editions() => View();
        public IActionResult Faq() => View();
    }
}
