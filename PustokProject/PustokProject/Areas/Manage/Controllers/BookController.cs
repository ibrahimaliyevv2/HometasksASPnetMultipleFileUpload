using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokProject.DAL;
using PustokProject.Helpers;
using PustokProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PustokProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            var model = _context.Books.Include(x => x.Genre).Include(x => x.Author).ToList();
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }


            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author notfound");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();

                return View();
            }

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre notfound");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();

                return View();
            }

            if (book.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "PosterFile is required");
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }
            else
            {
                if (book.PosterFile.ContentType != "image/png" && book.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterFile", "File format must be image/png or image/jpeg");
                }

                if (book.PosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterFile", "File size must be less than 2MB");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.Genres = _context.Genres.ToList();
                    return View();
                }

                BookImage bookImage = new BookImage
                {
                    Name = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterFile),
                    PosterStatus = true
                };

                book.BookImages.Add(bookImage);
            }



            if (book.ImageFiles != null)
            {
                foreach (var file in book.ImageFiles)
                {
                    if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", "File format must be image/png or image/jpeg");
                    }

                    if (file.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "File size must be less than 2MB");
                    }

                    if (!ModelState.IsValid)
                    {
                        ViewBag.Authors = _context.Authors.ToList();
                        ViewBag.Genres = _context.Genres.ToList();
                        return View();
                    }
                }

                foreach (var file in book.ImageFiles)
                {
                    BookImage bookImage = new BookImage
                    {
                        Name = FileManager.Save(_env.WebRootPath, "uploads/books", file),
                        PosterStatus = false
                    };

                    book.BookImages.Add(bookImage);
                }
            }



            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);

            if (book == null)
                return RedirectToAction("error", "dashboard");

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();


            return View(book);
        }
    }
}

