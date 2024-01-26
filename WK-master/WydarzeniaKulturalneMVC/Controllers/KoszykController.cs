using Microsoft.AspNetCore.Mvc;
using WydarzeniaKulturalneMVC.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;
namespace WydarzeniaKulturalneMVC.Controllers
{
    public class KoszykController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
