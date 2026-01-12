using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskMngmntSys.Entities.Task;

namespace TskMngmntSys.Tasks.Dtos
{
    public class TaskOutputDto : EntityDto<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public long? AssignedUserId { get; set; }
    }
}
