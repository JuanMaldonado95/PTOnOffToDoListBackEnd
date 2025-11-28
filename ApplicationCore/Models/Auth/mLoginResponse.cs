using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models.Auth
{
    public class mLoginResponse
    {
        public string Token { get; set; }
        public int iIDUser { get; set; }
        public string tUserName { get; set; }
    }
}
