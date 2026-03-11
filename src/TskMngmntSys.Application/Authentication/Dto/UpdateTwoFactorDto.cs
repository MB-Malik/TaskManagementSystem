using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TskMngmntSys.Authentication.Dto
{
    public class UpdateTwoFactorDto
    {
        //public long UserId { get; set; }
        //public int? TenantId { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
    }

}
