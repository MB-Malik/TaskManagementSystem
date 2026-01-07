using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskMngmntSys.Entities.Task;

namespace TskMngmntSys.Tasks.Dtos
{
    public class UpdateTaskDto
    {
        public int Id { get; set; }          
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskState Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
