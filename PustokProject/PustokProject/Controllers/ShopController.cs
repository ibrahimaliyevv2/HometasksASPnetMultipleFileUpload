using Microsoft.AspNetCore.Mvc;
using PustokProject.DAL;
using PustokProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokProject.Controllers
{
    public class ShopController : Controller
    {
        private AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ShopViewModel shopVM = new ShopViewModel
            {
                Genres = _context.Genres.ToList(),
            };
            return View(shopVM);
        }
    }
}
