using Library.Data.Repositories;
using LibraryReporter.Data.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Services;
using Microsoft.AspNetCore.Mvc;
using LibraryReporter.Models.Book;
using LibraryReporter.Models.LibraryReport;
using LibraryReporter.Models.Reader;

namespace LibraryReporter.Controllers
{
    public class LibraryReportController : Controller
    {
        private readonly ILogger<LibraryReportController> _logger;
        private AuthService _authService;
        private IBookRepositoryReal _bookRepository;
        private IIssuedBookRepositoryReal _issuedBookRepository;
        private IReaderRepositoryReal _readerRepository;
        private IAuthorRepositoryReal _authorRepository;
        private IPublisherRepositoryReal _publisherRepository;
        private IUserRepositryReal _userRepository;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public LibraryReportController(ILogger<LibraryReportController> logger,
            IBookRepositoryReal bookRepository,
            IAuthorRepositoryReal authorRepository,
            IIssuedBookRepositoryReal issuedBookRepository,
            IReaderRepositoryReal readerRepository,
            IPublisherRepositoryReal publisherRepository,
            WebDbContext webDbContext,
            IUserRepositryReal userRepository,
            AuthService authService,
            IWebHostEnvironment webHostEnvironment)
        {
            _issuedBookRepository = issuedBookRepository;
            _readerRepository = readerRepository;
            _publisherRepository = publisherRepository;
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _webDbContext = webDbContext;
            _userRepository = userRepository;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public IActionResult IssuedBooks()
        {
            var books = _bookRepository.GetAll()
                .Where(book => book.Status == Enums.Status.Status.InArmas) 
                .ToList();

            var issuedDatas = _issuedBookRepository.GetAll().ToList();

            // Совмещаем данные по совпадению идентификатора книги:
            // для каждой записи о выдаче находим соответствующую книгу.
            var issuedBooksViewModels = (from issued in issuedDatas
                                         join book in books on issued.BookId equals book.Id
                                         select new IssuedBookReportViewModel
                                         {
                                             ModerSurname = _userRepository.Get(issued.ModerId)?.Login,
                                             ReaderSurname = _readerRepository.Get(issued.ReaderId)?.Surname,
                                             BookName = book.BookName,
                                             Author = book.Author,
                                             Publisher = book.Publisher,
                                             Barcode = book.Barcode,
                                             IssueDate = issued.IssueDate,
                                             AcceptanceLastDate = issued.AcceptanceLastDate
                                         }).ToList();

            return View(issuedBooksViewModels);
        }

    }
}
