using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskMngmntSys.Entities.Task;

namespace TskMngmntSys.Tasks.Dtos
{
    public class ChangeTaskStatusDto
    {
        public int TaskId { get; set; }
        public TaskState Status { get; set; }
    }
}
