using Microsoft.AspNetCore.Identity;

namespace ET21KM_Ohjelmistotuotanto2_GroupB.Server.Models
{
	public class Traveller : IdentityUser
	{
		//Local Items
		public string Forename { get; set; }
		public string Surname { get; set; }
		public string Nickname { get; set; }
		public string Municipality { get; set; }
		public string Description { get; set; }
		public string Image { get; set; }

		//If this class is FK in other table create ICollection to enable
		//searching/listing the entries that have this class as FK
		public virtual ICollection<Trip> Trips { get; set; }

	}
}