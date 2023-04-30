using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakerShop.Reporting
{
    [Flags]
    public enum ReportScheduleTypes { Daily = 1, Sun = 2, Mon = 4, Tue = 8, Wed = 16, Thu = 32, Fri = 64, Sat = 128, Hourly = 256, Monthly = 512 };



    
}
