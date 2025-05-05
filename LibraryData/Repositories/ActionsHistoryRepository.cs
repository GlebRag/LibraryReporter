using LibraryReporter.Data.Models;
using LibraryReporter.Data;
using Enums.Users;
using Microsoft.EntityFrameworkCore;
using LibraryReporter.Data.Interfaces.Repositories;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using LibraryReporter.Data.Interfaces.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using System.Text;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Net;
using Library.Data.Models;
using Enums.Action;

namespace LibraryReporter.Data.Repositories
{
    public interface IActionsHistoryRepositoryReal : IActionsHistoryRepository<ActionsHistoryData>
    {
        // TO DO: Combine and create universal methods
        void CreateBook(Enums.Action.Actions action, string actionDescription, string bookName, string barcode, string moder);
        void CreateReader(Enums.Action.Actions action, string actionDescription, string ReaderName, string ReaderSurname, string moder);
        void CreateAuthor(Enums.Action.Actions action, string actionDescription, string AuthorName, string AuthorSurname, string moder);
        void CreatePublisher(Enums.Action.Actions action, string actionDescription, string PublisherName, string Email, string moder);

        void EditBook(Enums.Action.Actions action, string actionDescription, string bookName, string barcode, string moder);
        void EditReader(Enums.Action.Actions action, string actionDescription, string ReaderName, string ReaderSurname, string moder);
        void EditAuthor(Enums.Action.Actions action, string actionDescription, string AuthorName, string AuthorSurname, string moder);
        void EditPublisher(Enums.Action.Actions action, string actionDescription, string PublisherName, string Email, string moder);

        void DeleteBook(Enums.Action.Actions action, string actionDescription, string moder);
        void DeleteReader(Enums.Action.Actions action, string actionDescription, string moder);
        void DeleteAuthor(Enums.Action.Actions action, string actionDescription, string moder);
        void DeletePublisher(Enums.Action.Actions action, string actionDescription, string moder);
    }

    public class ActionsHistoryRepository : BaseRepository<ActionsHistoryData>, IActionsHistoryRepositoryReal
    {
        public ActionsHistoryRepository(WebDbContext webDbContext) : base(webDbContext)
        {
        }

        public void CreateAuthor(Actions action, string actionDescription, string AuthorName, string AuthorSurname, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} автора. Фамилия: {AuthorSurname} Имя: {AuthorName}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void CreateBook(Actions action, string actionDescription, string bookName, string barcode, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} книгу. Название: {bookName} Штрихкод: {barcode}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void CreatePublisher(Actions action, string actionDescription, string PublisherName, string Email, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} издателя. Название: {PublisherName} Эл. Почта: {Email}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void CreateReader(Actions action, string actionDescription, string ReaderName, string ReaderSurname, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} читателя. Фамилия: {ReaderSurname} Имя: {ReaderName}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }





        public void DeleteAuthor(Actions action, string actionDescription, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} автора.",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void DeleteBook(Actions action, string actionDescription, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} книгу.",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void DeletePublisher(Actions action, string actionDescription, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} издателя.",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void DeleteReader(Actions action, string actionDescription, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} читателя.",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }





        public void EditAuthor(Actions action, string actionDescription, string AuthorName, string AuthorSurname, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} автора. Фамилия: {AuthorSurname} Имя: {AuthorName}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void EditBook(Actions action, string actionDescription, string bookName, string barcode, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} книгу. Название: {bookName} Штрихкод: {barcode}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void EditPublisher(Actions action, string actionDescription, string PublisherName, string Email, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} издателя. Название: {PublisherName} Эл. Почта: {Email}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void EditReader(Actions action, string actionDescription, string ReaderName, string ReaderSurname, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} читателя. Фамилия: {ReaderSurname} Имя: {ReaderName}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }
    }
}
