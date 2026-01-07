using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TskMngmntSys.Tasks.Dtos
{
    public class GetTasksByUserInput : PagedAndSortedResultRequestDto
    {
        public long UserId { get; set; }
    }
}
