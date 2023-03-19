using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatkakertomusGroupB.Shared.Models
{
    public class UserDTO
    {
        public UserDTO()
        {
            
        }

		public UserDTO(string userName)
		{
            this.Username = userName;
		}

        //It's a GUID, thus string
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Nickname { get; set; }

    }
}
