using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TskMngmntSys.Authentication.Dto
{
    public class Verify2FaDto
    {
        public long UserId { get; set; }
        public string Code { get; set; }
        public string TenancyName { get; set; }
    }

}
