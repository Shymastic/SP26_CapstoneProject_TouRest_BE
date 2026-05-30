using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyWithUsersDTO
    {
        public Guid AgencyId { get; set; }
        public string AgencyName { get; set; } = null!;
        public IEnumerable<AgencyUserListItemDTO> Users { get; set; } = [];
    }
}
