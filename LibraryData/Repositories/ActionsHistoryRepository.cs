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
        void CreateBook(Enums.Action.Actions action, string actionDescription, string bookName, string barcode, string moder);
    }

    public class ActionsHistoryRepository : BaseRepository<ActionsHistoryData>, IActionsHistoryRepositoryReal
    {
        public ActionsHistoryRepository(WebDbContext webDbContext) : base(webDbContext)
        {
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
    }
}
