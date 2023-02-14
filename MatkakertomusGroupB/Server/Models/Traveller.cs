using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MatkakertomusGroupB.Server.Models
{
	//Replaces ApplicationUser Class
	//Must extend IdentityUser
	//Remeber to replace ApplicationUser with Traveller in all relevant files
	public class Traveller : IdentityUser
	{
		//Local Items
		[Required]
		[PersonalData]
		public string Forename { get; set; }

		[Required]
		[PersonalData]
		public string Surname { get; set; }

		[PersonalData]
		public string? Municipality { get; set; }

		[PersonalData]
		public string? Description { get; set; }

		[PersonalData]
		public string? Image { get; set; }

		//If this class is FK in other table create ICollection to enable
		//searching/listing the entries that have this class as FK
		public virtual ICollection<Trip> Trips { get; set; }

	}
}