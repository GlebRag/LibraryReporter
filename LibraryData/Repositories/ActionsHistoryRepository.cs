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
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        void IssueBook(Actions action, string actionDescription, string ReaderName, string ReaderSurname, string BookName, string Barcode, string Author, string Publisher, DateOnly IssueDate, DateOnly AcceptanceLastDate, string moder);
        void AcceptanceBook(Actions action, string actionDescription, string ReaderName, string ReaderSurname, string BookName, string Barcode, string Author, string Publisher,string moder);

        IEnumerable<ActionsHistoryData> SearchAction(Actions action, DateOnly dateFrom, DateOnly dateTo);
        
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



        public void IssueBook(Actions action, string actionDescription, string ReaderName, string ReaderSurname, string BookName, string Barcode, string Author, string Publisher, DateOnly IssueDate, DateOnly AcceptanceLastDate, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} книгу читателю: {ReaderName} {ReaderSurname}. Название: {BookName} Штрихкод: {Barcode} Автор: {Author} Издатель: {Publisher}. Книга выдана с {IssueDate} по {AcceptanceLastDate}",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }

        public void AcceptanceBook(Actions action, string actionDescription, string ReaderName, string ReaderSurname, string BookName, string Barcode, string Author, string Publisher, string moder)
        {
            var dataHistory = new ActionsHistoryData
            {
                ModerSurname = moder,
                Actions = action,
                Description = $"{actionDescription} книгу у читателя: {ReaderName} {ReaderSurname}. Название: {BookName} Штрихкод: {Barcode} Автор: {Author} Издатель: {Publisher}.",
                ExecutionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            Add(dataHistory);
        }



        public IEnumerable<ActionsHistoryData> SearchAction(Actions action, DateOnly dateFrom, DateOnly dateTo)
        {
            var parameters = new List<SqlParameter>();
            var sql = new StringBuilder("SELECT * FROM dbo.ActionHistory WHERE 1=1"); //Это условие всгеда истинно

            sql.Append(" AND Actions = @Actions");
            parameters.Add(new SqlParameter("@Actions", action));
         
            if (dateFrom != DateOnly.MinValue && dateTo != DateOnly.MinValue)
            {
                // Если есть оба значения: диапазон между DateFrom и DateTo
                sql.Append(" AND CAST(ExecutionDate AS DATE) BETWEEN @DateFrom AND @DateTo");
                parameters.Add(new SqlParameter("@DateFrom", dateFrom));
                parameters.Add(new SqlParameter("@DateTo", dateTo));
            }
            else if (dateFrom != DateOnly.MinValue)
            {
                // Если только DateFrom: данные от DateFrom до сегодняшнего дня
                sql.Append(" AND CAST(ExecutionDate AS DATE) >= @DateFrom");
                parameters.Add(new SqlParameter("@DateFrom", dateFrom));
            }
            else if (dateTo != DateOnly.MinValue)
            {
                // Если только DateTo: данные с начала до DateTo
                sql.Append(" AND CAST(ExecutionDate AS DATE) <= @DateTo");
                parameters.Add(new SqlParameter("@DateTo", dateTo));
            }
            else
            {
                // Если нет значений, вы можете либо ничего не делать, либо использовать данные до сегодняшнего дня
                sql.Append(" AND CAST(ExecutionDate AS DATE) <= @CurrentDate");
                parameters.Add(new SqlParameter("@CurrentDate", DateTime.Now.Date));
            }

            var result = _webDbContext
                .Database
                .SqlQueryRaw<ActionsHistoryData>(sql.ToString(), parameters.ToArray())
                .ToList();

            return result;
        }
    }
}
