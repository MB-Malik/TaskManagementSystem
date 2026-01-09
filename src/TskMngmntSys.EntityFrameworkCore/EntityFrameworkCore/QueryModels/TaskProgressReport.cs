using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TskMngmntSys.EntityFrameworkCore.QueryModels
{
    public class TaskProgressReport
    {
        public long UserId { get; set; }
        public string UserName { get; set; }

        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
    }
}
