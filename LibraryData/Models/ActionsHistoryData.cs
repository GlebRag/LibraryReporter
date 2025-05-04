using Enums.Action;
using Library.Data.Interfaces.Models;
using LibraryReporter.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Models
{
    public class ActionsHistoryData : BaseModel, IActionsHistoryData
    {
        public string ModerSurname { get; set; }
        public Actions Actions { get; set; }
        public string Description { get; set; }
        public DateOnly ExecutionDate { get; set; }
    }
}
