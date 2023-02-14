using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace MatkakertomusGroupB.Server.Models
{
	public class Destination
	{
		//PK
		public int DestinationId { get; set; }

		//Local Items
		[Required]
		public string Name { get; set; }
		[Required]
		public string Country { get; set; }
		[Required]
		public string Municipality { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public string Image { get; set; }


		//If this class is FK in other table create ICollection to enable
		//searching/listing the entries that have this class as FK
		public virtual ICollection<Story> Stories { get; set; }
	}
}
