using Abp.Application.Services.Dto;

namespace TskMngmntSys.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

