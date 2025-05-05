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
using LibraryReporter.Models.Reader;
using Library.Data.Models;
using Library.Data.Repositories;
using System.Net;
using DocumentFormat.OpenXml.Bibliography;

namespace LibraryReporter.Controllers
{
    public class BookController : Controller // MainController
    {
        private readonly ILogger<BookController> _logger;
        private AuthService _authService;
        private IBookRepositoryReal _bookRepository;
        private IIssuedBookRepositoryReal _issuedBookRepository;
        private IActionsHistoryRepositoryReal _actionsHistoryRepository;
        private IReaderRepositoryReal _readerRepository;
        private IAuthorRepositoryReal _authorRepository;
        private IPublisherRepositoryReal _publisherRepository;
        private IUserRepositryReal _userRepositryReal;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public BookController(ILogger<BookController> logger,
            IBookRepositoryReal bookRepository,
            IAuthorRepositoryReal authorRepository,
            IActionsHistoryRepositoryReal actionsHistoryRepository,
            IIssuedBookRepositoryReal issuedBookRepository,
            IReaderRepositoryReal readerRepository,
            IPublisherRepositoryReal publisherRepository,
            WebDbContext webDbContext,
            IUserRepositryReal userRepositryReal,
            AuthService authService,
            IWebHostEnvironment webHostEnvironment)
        {
            _actionsHistoryRepository = actionsHistoryRepository;
            _issuedBookRepository = issuedBookRepository;
            _readerRepository = readerRepository;
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

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Create);
            var moderName = _authService.GetName();
            _actionsHistoryRepository
                .CreateBook(Enums.Action.Actions.Create, actionDescription, viewModel.BookName, dataBook.Barcode, moderName);
            
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

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Delete);
            var moderName = _authService.GetName();
            _actionsHistoryRepository.DeleteBook(Enums.Action.Actions.Delete, actionDescription, moderName);

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

        [HttpGet]
        public IActionResult EditBook(int bookId)
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

            var viewModel = new BookEditViewModel()
            {
                Authors = authorsViewModels,
                Publishers = publishersViewModels
            };
            var book = _webDbContext.Books.First(x => x.Id == bookId);
            viewModel.BookName = book.BookName;
            viewModel.Author = book.Author;
            viewModel.Publisher = book.Publisher;
            viewModel.Barcode = book.Barcode;
            viewModel.Status = StatusHelper.GetStatusDescription(book.Status);
            viewModel.Id = bookId;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditBook(BookEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
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

                viewModel.Authors = authorsViewModels;
                viewModel.Publishers = publishersViewModels;

                return View(viewModel);
            }

            var bookId = viewModel.Id;


            var dataBook = new BookData
            {
                BookName = viewModel.BookName,
                Author = viewModel.Author,
                Publisher = viewModel.Publisher,
                Barcode = viewModel.Barcode,
                Status = StatusHelper.GetStatusEnum(viewModel.Status)
            };


            _bookRepository.Update(dataBook, bookId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Edit);
            var moderName = _authService.GetName();
            _actionsHistoryRepository
                .EditBook(Enums.Action.Actions.Edit, actionDescription, viewModel.BookName, viewModel.Barcode, moderName);


            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult IssueBook(int bookId)
        {
            var readersViewModels = _readerRepository
                .GetAll()
                .Select(x => new ReaderSurnameAndIdViewModel
                {
                    Id = x.Id,
                    Surname = x.Surname
                })
                .ToList();

            var viewModel = new IssueBookViewModel()
            {
                Readers = readersViewModels
            };

            var book = _webDbContext.Books.First(x => x.Id == bookId);
            viewModel.BookName = book.BookName;
            viewModel.Author = book.Author;
            viewModel.Publisher = book.Publisher;
            viewModel.IssueDate = DateOnly.FromDateTime(DateTime.Now);
            viewModel.AcceptanceLastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14));
            viewModel.Id = bookId;

            return View(viewModel);
        }

        [IsAuthenticated]
        [HttpPost]
        public IActionResult IssueBook(IssueBookViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                var readersViewModels = _readerRepository
                .GetAll()
                .Select(x => new ReaderSurnameAndIdViewModel
                {
                    Id = x.Id,
                    Surname = x.Surname
                })
                .ToList();

                viewModel.IssueDate = DateOnly.FromDateTime(DateTime.Now);
                viewModel.AcceptanceLastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14));

                viewModel.Readers = readersViewModels;
                

                return View(viewModel);
            }

            var bookId = viewModel.Id;

            var moderId = _authService.GetUserId();

            var dataIssueBook = new IssuedBookData
            {
                ModerId = (int)moderId!,
                BookId = bookId,
                ReaderId = viewModel.Reader,
                IssueDate = viewModel.IssueDate,
                AcceptanceLastDate = viewModel.AcceptanceLastDate
            };


            _issuedBookRepository.IssueBook(dataIssueBook, bookId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Issue);
            var moderName = _authService.GetName();
            var readerName = _readerRepository.Get(viewModel.Reader)?.Name;
            var readerSurname = _readerRepository.Get(viewModel.Reader)?.Surname;
            var barcode = _bookRepository.Get(bookId)?.Barcode;
            var author = _bookRepository.Get(bookId)?.Author;
            var publisher = _bookRepository.Get(bookId)?.Publisher;
            _actionsHistoryRepository
                .IssueBook(Enums.Action.Actions.Issue, actionDescription, readerName, readerSurname, viewModel.BookName, barcode, author, publisher, viewModel.IssueDate, viewModel.AcceptanceLastDate, moderName);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AcceptanceBook(int bookId)
        {
            var issueBook = _webDbContext.IssuedBooks.First(x => x.BookId == bookId);
            var readerId = issueBook.ReaderId;
            var readerSurname = _webDbContext.Readers.First(x => x.Id == readerId).Surname;

            var viewModel = new AcceptanceBookViewModel();

            var book = _webDbContext.Books.First(x => x.Id == bookId);
            viewModel.BookName = book.BookName;
            viewModel.Author = book.Author;
            viewModel.Publisher = book.Publisher;
            viewModel.AcceptanceDate = DateOnly.FromDateTime(DateTime.Now);
            viewModel.Reader = readerSurname;
            viewModel.Id = bookId;

            return View(viewModel);
        }

        [IsAuthenticated]
        [HttpPost]
        public IActionResult AcceptanceBook(AcceptanceBookViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                return View(viewModel);
            }

            var bookId = viewModel.Id;

            var issuedBookReaderId = _webDbContext.IssuedBooks.First(x => x.BookId == bookId).ReaderId;

            _issuedBookRepository.AcceptanceBook(bookId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Acceptance);
            var moderName = _authService.GetName();
            var readerName = _readerRepository.Get(issuedBookReaderId)?.Name;
            var readerSurname = viewModel.Reader;
            var barcode = _bookRepository.Get(bookId)?.Barcode;
            var author = _bookRepository.Get(bookId)?.Author;
            var publisher = _bookRepository.Get(bookId)?.Publisher;
            _actionsHistoryRepository
                .AcceptanceBook(Enums.Action.Actions.Acceptance, actionDescription, readerName, readerSurname, viewModel.BookName, barcode, author, publisher, moderName);


            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ValidateBarcode([FromBody] BarcodeCheckModel model)
        {
            if (model == null)
            {
                return Json(new { match = false, message = "Получены пустые данные!" });
            }

            // Ищем книгу по идентификатору
            var book = _webDbContext.Books.FirstOrDefault(x => x.Id == model.BookId);
            if (book == null)
            {
                return Json(new { match = false, message = "Книга не найдена!" });
            }

            // Сравниваем штрихкоды с учетом удаления начальных и конечных пробелов
            bool isMatch = book.Barcode.Trim() == model.BookBarcode.Trim();

            return Json(new
            {
                match = isMatch,
                message = isMatch ? "Штрихкод подтвержден!" : "Ошибка: штрихкоды не совпадают!",
                BookId = book.Id,
                BookBarcode = book.Barcode.Trim() // возвращаем штрихкод без лишних пробелов
            });
        }



        public class BarcodeCheckModel
        {
            public string BookBarcode { get; set; }
            public int BookId { get; set; }
        }



    }
}
