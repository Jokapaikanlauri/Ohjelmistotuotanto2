using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace ET21KM_Ohjelmistotuotanto2_GroupB.Server.Models
{
	public class Destination
	{
		//PK
		public int DestinationId { get; set; }

		//Local Items

		public string Name { get; set; }
		public string Country { get; set; }
		public string Municipality { get; set; }
		public string Description { get; set; }
		public string Image { get; set; }


		//If this class is FK in other table create ICollection to enable
		//searching/listing the entries that have this class as FK
		public virtual ICollection<Story> Stories { get; set; }
	}
}
