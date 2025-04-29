using LibraryReporter.Data.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Models;
using LibraryReporter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LibraryReporter.Models.Book;

namespace LibraryReporter.Controllers
{
    public class AuthorController : Controller // MainController
    {
        private readonly ILogger<AuthorController> _logger;
        private AuthService _authService;
        private IAuthorRepositoryReal _bookRepository;
        private IUserRepositryReal _userRepositryReal;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public AuthorController(ILogger<AuthorController> logger,
            IBookRepositoryReal bookRepository,
            WebDbContext webDbContext,
            IUserRepositryReal userRepositryReal,
            AuthService authService,
            IWebHostEnvironment webHostEnvironment)
        {
            _bookRepository = bookRepository;
            _webDbContext = webDbContext;
            _userRepositryReal = userRepositryReal;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var booksFromDb = _bookRepository.GetAll();
            var booksViewModels = booksFromDb
                .Select(dbBook =>
                    new BookViewModel
                    {
                        BookName = dbBook.BookName,
                        Author = dbBook.Author,
                        Publisher = dbBook.Publisher,
                        Barcode = dbBook.Barcode,
                        Status = StatusHelper.GetStatusDescription(dbBook.Status)
                    }
                )
                .ToList();

            return View(booksViewModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
