using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models.Auth
{
    public class mLogin
    {
        [Required]
        public string tUserName { get; set; }

        [Required]
        public string tPassword { get; set; }
    }
}
