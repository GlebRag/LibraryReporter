using LibraryReporter.Data.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Models;
using LibraryReporter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LibraryReporter.Models.Book;
using Library.Data.Models;
using LibraryReporter.Controllers.AuthAttributes;
using LibraryReporter.Data.Interfaces.Repositories;
using LibraryReporter.Models.Reader;
using LibraryReporter.Models.Author;

namespace LibraryReporter.Controllers
{
    public class AuthorController : Controller // MainController
    {
        private readonly ILogger<AuthorController> _logger;
        private AuthService _authService;
        private IActionsHistoryRepositoryReal _actionsHistoryRepository;
        private IAuthorRepositoryReal _authorRepository;
        private IUserRepositryReal _userRepositryReal;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public AuthorController(ILogger<AuthorController> logger,
            IAuthorRepositoryReal authorRepository,
            IActionsHistoryRepositoryReal actionsHistoryRepository,
            WebDbContext webDbContext,
            IUserRepositryReal userRepositryReal,
            AuthService authService,
            IWebHostEnvironment webHostEnvironment)
        {
            _actionsHistoryRepository = actionsHistoryRepository;
            _authorRepository = authorRepository;
            _webDbContext = webDbContext;
            _userRepositryReal = userRepositryReal;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var authorsFromDb = _authorRepository.GetAll();
            var authorsViewModels = authorsFromDb
                .Select(dbAuthor =>
                    new AuthorViewModel
                    {
                        Id = dbAuthor.Id,
                        Name = dbAuthor.Name,
                        Surname = dbAuthor.Surname,
                        PhoneNumber = dbAuthor.PhoneNumber,
                        AddedDate = dbAuthor.AddedDate,
                    }
                )
                .ToList();

            return View(authorsViewModels);
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
        public IActionResult CreateAuthor()
        {

            var viewModel = new AuthorCreationViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateAuthor(AuthorCreationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dataAuhor = new AuthorData
            {
                Name = viewModel.Name,
                Surname = viewModel.Surname,
                PhoneNumber = viewModel.PhoneNumber,
                AddedDate = DateOnly.FromDateTime(DateTime.Now)

            };

            _authorRepository.Create(dataAuhor);
            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Create);
            var moderName = _authService.GetName();
            _actionsHistoryRepository
                .CreateAuthor(Enums.Action.Actions.Create, actionDescription, viewModel.Surname, viewModel.Name, moderName);

            viewModel.Success = true;

            return View("AuthorCreated", viewModel);
        }

        [IsAuthenticated]
        public IActionResult DeleteAuthor(int authorId)
        {
            _authorRepository.Delete(authorId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Delete);
            var moderName = _authService.GetName();
            _actionsHistoryRepository.DeleteAuthor(Enums.Action.Actions.Delete, actionDescription, moderName);

            return RedirectToAction("Index");
        }

        public IActionResult SearchAuthors(string name, string surname, string phonenumber, DateOnly birthday)
        {

            var viewModels = _authorRepository
                .SearchAuthor(name, surname, phonenumber)
                .Select(dbReader => new AuthorViewModel
                {
                    Id = dbReader.Id,
                    Name = dbReader.Name,
                    Surname = dbReader.Surname,
                    PhoneNumber = dbReader.PhoneNumber,
                    AddedDate = dbReader.AddedDate

                })
                .ToList();

            return View("Index", viewModels);
        }

        [HttpGet]
        public IActionResult EditAuthor(int authorId)
        {
            var viewModel = new AuthorViewModel();
            var reader = _webDbContext.Authors.First(x => x.Id == authorId);
            viewModel.Name = reader.Name;
            viewModel.Surname = reader.Surname;
            viewModel.PhoneNumber = reader.PhoneNumber;
            viewModel.AddedDate = reader.AddedDate;
            viewModel.Id = authorId;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditAuthor(AuthorViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                return View(viewModel);
            }

            var authorId = viewModel.Id;


            var dataAuthor = new AuthorData
            {
                Name = viewModel.Name,
                Surname = viewModel.Surname,
                PhoneNumber = viewModel.PhoneNumber,
                AddedDate = DateOnly.FromDateTime(DateTime.Now)
            };


            _authorRepository.Update(dataAuthor, authorId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Edit);
            var moderName = _authService.GetName();
            _actionsHistoryRepository
                .EditAuthor(Enums.Action.Actions.Edit, actionDescription, viewModel.Surname, viewModel.Name, moderName);


            return RedirectToAction("Index");
        }
    }
}
