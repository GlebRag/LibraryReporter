﻿using LibraryReporter.Data.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Models;
using LibraryReporter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LibraryReporter.Models.Book;
using Library.Data.Models;
using LibraryReporter.Controllers.AuthAttributes;
using LibraryReporter.Data.Interfaces.Repositories;
using LibraryReporter.Models.Author;
using LibraryReporter.Models.Publisher;
using LibraryReporter.Models.Reader;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace LibraryReporter.Controllers
{
    public class PublisherController : Controller // MainController
    {
        private readonly ILogger<PublisherController> _logger;
        private AuthService _authService;
        private IActionsHistoryRepositoryReal _actionsHistoryRepository;
        private IPublisherRepositoryReal _publisherRepository;
        private IUserRepositryReal _userRepositryReal;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public PublisherController(ILogger<PublisherController> logger,
            IPublisherRepositoryReal publisherRepository,
            IActionsHistoryRepositoryReal actionsHistoryRepository,
            WebDbContext webDbContext,
            IUserRepositryReal userRepositryReal,
            AuthService authService,
            IWebHostEnvironment webHostEnvironment)
        {
            _actionsHistoryRepository = actionsHistoryRepository;
            _publisherRepository = publisherRepository;
            _webDbContext = webDbContext;
            _userRepositryReal = userRepositryReal;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var publishersFromDb = _publisherRepository.GetAll();
            var publishersViewModels = publishersFromDb
                .Select(dbPublisher =>
                    new PublisherViewModel
                    {
                        Id = dbPublisher.Id,
                        Name = dbPublisher.Name,
                        City = dbPublisher.City,
                        Email = dbPublisher.Email,
                        AddedDate = dbPublisher.AddedDate
                    }
                )
                .ToList();

            return View(publishersViewModels);
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
        public IActionResult CreatePublisher()
        {

            var viewModel = new PublisherCreationViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreatePublisher(PublisherCreationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dataPublisher = new PublisherData
            {
                Name = viewModel.Name,
                City = viewModel.City,
                Email = viewModel.Email,
                AddedDate = DateOnly.FromDateTime(DateTime.Now)

            };

            _publisherRepository.Create(dataPublisher);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Create);
            var moderName = _authService.GetName();
            _actionsHistoryRepository
                .CreatePublisher(Enums.Action.Actions.Create, actionDescription, viewModel.Name, viewModel.Email, moderName);

            viewModel.Success = true;

            return View("PublisherCreated", viewModel);
        }

        [IsAuthenticated]
        public IActionResult DeletePublisher(int publisherId)
        {
            _publisherRepository.Delete(publisherId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Delete);
            var moderName = _authService.GetName();
            _actionsHistoryRepository.DeletePublisher(Enums.Action.Actions.Delete, actionDescription, moderName);

            return RedirectToAction("Index");
        }

        public IActionResult SearchPublishers(string name, string city, string email)
        {

            var viewModels = _publisherRepository
                .SearchPublisher(name, city, email)
                .Select(dbReader => new PublisherViewModel
                {
                    Id = dbReader.Id,
                    Name = dbReader.Name,
                    City = dbReader.City,
                    Email = dbReader.Email,
                    AddedDate = dbReader.AddedDate

                })
                .ToList();

            return View("Index", viewModels);
        }

        [HttpGet]
        public IActionResult EditPublisher(int publisherId)
        {
            var viewModel = new PublisherViewModel();
            var publisher = _webDbContext.Publishers.First(x => x.Id == publisherId);
            viewModel.Name = publisher.Name;
            viewModel.City = publisher.City;
            viewModel.Email = publisher.Email;
            viewModel.AddedDate = publisher.AddedDate;
            viewModel.Id = publisherId;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditPublisher(PublisherViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                return View(viewModel);
            }

            var publisherId = viewModel.Id;


            var dataPublisher = new PublisherData
            {
                Name = viewModel.Name,
                City = viewModel.City,
                Email = viewModel.Email,
                AddedDate = DateOnly.FromDateTime(DateTime.Now)
            };


            _publisherRepository.Update(dataPublisher, publisherId);

            var actionDescription = ActionsHelper.GetActionDescription(Enums.Action.Actions.Edit);
            var moderName = _authService.GetName();
            _actionsHistoryRepository
                .EditPublisher(Enums.Action.Actions.Edit, actionDescription, viewModel.Name, viewModel.Email, moderName);

            return RedirectToAction("Index");
        }
    }
}
