using LibraryReporter.Data.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Models;
using LibraryReporter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LibraryReporter.Models.Book;
using LibraryReporter.Controllers.AuthAttributes;
using LibraryReporter.Data.Interfaces.Repositories;
using LibraryReporter.Data.Models;
using LibraryReporter.Models.Reader;
using Library.Data.Models;

namespace LibraryReporter.Controllers
{
    public class ReaderController : Controller // MainController
    {
        private readonly ILogger<ReaderController> _logger;
        private AuthService _authService;
        private IReaderRepositoryReal _readerRepository;
        private IActionsHistoryRepositoryReal _actionsHistoryRepository;
        private IUserRepositryReal _userRepositryReal;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public ReaderController(ILogger<ReaderController> logger,
            IReaderRepositoryReal readerRepository,
            IActionsHistoryRepositoryReal actionsHistoryRepository,
            WebDbContext webDbContext,
            IUserRepositryReal userRepositryReal,
            AuthService authService,
            IWebHostEnvironment webHostEnvironment)
        {
            _actionsHistoryRepository = actionsHistoryRepository;
            _readerRepository = readerRepository;
            _webDbContext = webDbContext;
            _userRepositryReal = userRepositryReal;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var readersFromDb = _readerRepository.GetAll();
            var readersViewModels = readersFromDb
                .Select(dbReader =>
                    new ReaderViewModel
                    {
                        Id = dbReader.Id,
                        Name = dbReader.Name,
                        Surname = dbReader.Surname,
                        PhoneNumber = dbReader.PhoneNumber,
                        Birtday = dbReader.Birtday,
                        AddedDate = dbReader.AddedDate,
                    }
                )
                .ToList();

            return View(readersViewModels);
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
        public IActionResult CreateReader()
        {

            var viewModel = new ReaderCreationViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateReader(ReaderCreationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dataReader = new ReaderData
            {
                Name = viewModel.Name,
                Surname = viewModel.Surname,
                PhoneNumber = viewModel.PhoneNumber,
                Birtday = viewModel.Birtday,
                AddedDate = DateOnly.FromDateTime(DateTime.Now)

            };

            _readerRepository.Create(dataReader);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Create);
            var moderName = _authService.GetName();
            _actionsHistoryRepository.CreateReader(Enums.Action.Actions.Create, actionDescription, viewModel.Name, viewModel.Surname, moderName);
            viewModel.Success = true;

            return View("ReaderCreated", viewModel);
        }

        [IsAuthenticated]
        public IActionResult DeleteReader(int readerId)
        {
            _readerRepository.Delete(readerId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Delete);
            var moderName = _authService.GetName();
            _actionsHistoryRepository.DeleteReader(Enums.Action.Actions.Delete, actionDescription, moderName);

            return RedirectToAction("Index");
        }

        public IActionResult SearchReaders(string name, string surname, string phonenumber, DateOnly birthday)
        {

            var viewModels = _readerRepository
                .SearchReader(name, surname, phonenumber, birthday)
                .Select(dbReader => new ReaderViewModel
                {
                    Id = dbReader.Id,
                    Name = dbReader.Name,
                    Surname = dbReader.Surname,
                    PhoneNumber = dbReader.PhoneNumber,
                    Birtday= dbReader.Birtday,
                    AddedDate = dbReader.AddedDate

                })
                .ToList();

            return View("Index", viewModels);
        }

        [HttpGet]
        public IActionResult EditReader(int readerId)
        {
            var viewModel = new ReaderViewModel();
            var reader = _webDbContext.Readers.First(x => x.Id == readerId);
            viewModel.Name = reader.Name;
            viewModel.Surname = reader.Surname;
            viewModel.PhoneNumber = reader.PhoneNumber;
            viewModel.Birtday = reader.Birtday;
            viewModel.AddedDate = reader.AddedDate;
            viewModel.Id = readerId;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditReader(ReaderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                return View(viewModel);
            }

            var readerId = viewModel.Id;


            var dataReader = new ReaderData
            {
                Name = viewModel.Name,
                Surname = viewModel.Surname,
                PhoneNumber = viewModel.PhoneNumber,
                Birtday = viewModel.Birtday,
                AddedDate = DateOnly.FromDateTime(DateTime.Now)
            };


            _readerRepository.Update(dataReader, readerId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Edit);
            var moderName = _authService.GetName();
            _actionsHistoryRepository
                .EditReader(Enums.Action.Actions.Edit, actionDescription, viewModel.Name, viewModel.Surname, moderName);

            return RedirectToAction("Index");
        }
    }
}
