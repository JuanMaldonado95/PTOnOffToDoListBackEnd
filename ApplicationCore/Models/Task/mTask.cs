using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models.Task
{
    public class mTask
    {
        public int? iIDUser { get; set; }
        public int? iIDTask { get; set; }
        public string? tTitle { get; set; }
        public bool bIsCompleted { get; set; }
        public DateTime dtDateTimeRegister { get; set; }
    }
}
