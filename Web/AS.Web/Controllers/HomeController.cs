using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AS.Web.Models;
using AS.Data;
using AS.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AS.Web.Controllers
{
    public class HomeController : Controller
    {
        private ASDbContext aSDbContext;
        public HomeController(ASDbContext aSDbContext)
        {
            this.aSDbContext = aSDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}
