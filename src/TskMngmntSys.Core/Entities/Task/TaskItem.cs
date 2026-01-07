using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TskMngmntSys.Entities.Task
{
    public class TaskItem : FullAuditedEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskState Status { get; set; }
        public DateTime? DueDate { get; set; }

        public long? AssignedUserId { get; set; }
    }
}
