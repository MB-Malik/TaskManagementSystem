using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TskMngmntSys.Authentication.Dto
{
    public class AuthenticatorSetupDto
    {
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        public string QrCodeImage { get; set; }
    }
}
