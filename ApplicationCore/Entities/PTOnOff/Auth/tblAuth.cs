using ApplicationCore.Entities.PTOnOff.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.PTOnOff.Auth
{
    public class tblAuth
    {
        public tblAuth()
        {
            tblTaskNavigation = new HashSet<tblTask>();
        }

        public int iIDUser { get; set; }
        public string tUserName { get; set; }
        public string tPasswordHash { get; set; }
        public DateTime dtDateTimeRegister { get; set; }

        public ICollection<tblTask> tblTaskNavigation { get; set; }
    }
}
