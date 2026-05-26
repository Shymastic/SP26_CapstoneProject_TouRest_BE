using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyTodayScheduleDTO
    {
        public int TodayScheduleCount { get; set; }
        public int ConfirmScheduleCount { get; set; }
        public int PendingScheduleCount { get; set; }
    }
}
