using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyUserListItemDTO
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
