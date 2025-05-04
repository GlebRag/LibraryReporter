using Enums.Action;
using LibraryReporter.Data.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces.Models
{
    public interface IActionsHistoryData : IBaseModel
    {
        public string ModerSurname { get; set; }
        public Actions Actions { get; set; }
        public string Description { get; set; }
        public DateOnly ExecutionDate { get; set; }
    }
}
