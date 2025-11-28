using ApplicationCore.Entities.PTOnOff.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.PTOnOff.Task
{
    public class tblTask
    {
        public int iIDTask { get; set; }
        public int iIDUser { get; set; }
        public string tTitle { get; set; }
        public bool bIsCompleted { get; set; }
        public DateTime dtDateTimeRegister { get; set; }

        public virtual  tblAuth tblAuthNavigation { get; set; }
    }
}
