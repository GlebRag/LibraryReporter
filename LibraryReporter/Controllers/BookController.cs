using LibraryReporter.Data.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Models;
using LibraryReporter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LibraryReporter.Models.Book;
using LibraryReporter.Data.Models;
using LibraryReporter.Data.Interfaces.Repositories;
using LibraryReporter.Controllers.AuthAttributes;

namespace LibraryReporter.Controllers
{
    public class BookController : Controller // MainController
    {
        private readonly ILogger<BookController> _logger;
        private AuthService _authService;
        private IBookRepositoryReal _bookRepository;
        private IAuthorRepositoryReal _authorRepository;
        private IPublisherRepositoryReal _publisherRepository;
        private IUserRepositryReal _userRepositryReal;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public BookController(ILogger<BookController> logger,
            IBookRepositoryReal bookRepository,
            IAuthorRepositoryReal authorRepository,
            IPublisherRepositoryReal publisherRepository,
            WebDbContext webDbContext,
            IUserRepositryReal userRepositryReal,
            AuthService authService,
            IWebHostEnvironment webHostEnvironment)
        {
            _publisherRepository = publisherRepository;
            _authorRepository = authorRepository;
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
                        Id = dbBook.Id,
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

        [HttpGet]
        public IActionResult CreateBook()
        {
            var authorsViewModels = _authorRepository
                .GetAll()
                .Select(x => new AuthorSurnameAndIdViewModel 
                {
                    Id = x.Id,
                    Surname = x.Surname
                })
                .ToList();

            var publishersViewModels = _publisherRepository
                .GetAll()
                .Select(x => new PublisherNameAndIdViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();

            var viewModel = new BookCreationViewModel()
            {
                Authors = authorsViewModels,
                Publishers = publishersViewModels
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateBook(BookCreationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dataBook = new BookData
            {
                BookName = viewModel.BookName,
                Author = viewModel.Author,
                Publisher = viewModel.Publisher,
                Barcode = GenerateUniqueBarcode(), // Генерация уникального штрихкода
                Status = Enums.Status.Status.InLibrary
            };

            _bookRepository.Create(dataBook);

            return RedirectToAction("Index");
        }

        private string GenerateUniqueBarcode()
        {
            Random random = new Random();
            string barcode;

            do
            {
                barcode = random.Next(10000000, 99999999).ToString(); // Генерация случайного 8-значного числа
            } while (IsBarcodeExists(barcode)); // Проверка на существование в репозитории

            return barcode;
        }
        private bool IsBarcodeExists(string barcode)
        {
            // Пример проверки через репозиторий
            return _bookRepository.GetAll().Any(book => book.Barcode == barcode);
        }

        [IsAuthenticated]
        public IActionResult DeleteBook(int bookId)
        {
            _bookRepository.Delete(bookId);

            return RedirectToAction("Index");
        }

        public IActionResult SearchBooks(string name, string author, string publisher, string barcode)
        {

            var viewModels = _bookRepository
                .SearchBook(name, author, publisher, barcode)
                .Select(dbBook => new BookViewModel
                {

                    Id = dbBook.Id,
                    BookName = dbBook.BookName,
                    Author = dbBook.Author,
                    Publisher = dbBook.Publisher,
                    Barcode = dbBook.Barcode,
                    Status = StatusHelper.GetStatusDescription(dbBook.Status)

                })
                .ToList();

            return View("Index", viewModels);
        }
    }
}
