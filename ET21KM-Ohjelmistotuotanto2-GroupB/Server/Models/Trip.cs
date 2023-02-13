using System.Security.Cryptography;

namespace ET21KM_Ohjelmistotuotanto2_GroupB.Server.Models
{
	public class Trip
	{
		//PK
		public int TripId { get; set; }

		//FK
		public int TravellerId { get; set; }
		public virtual Traveller Traveller { get; set; }


		//Local Items

		public DateTime DatumStart { get; set; }

		public DateTime DatumEnd { get; set; }

		public Boolean Private { get; set; }


		//If this class is FK in other table create ICollection to enable
		//searching/listing the entries that have this class as FK
		public virtual ICollection<Story> Stories { get; set; }
	}
}
