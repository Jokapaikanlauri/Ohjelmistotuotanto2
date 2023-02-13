using System.Security.Cryptography;

namespace ET21KM_Ohjelmistotuotanto2_GroupB.Server.Models
{
	public class Story
	{
		//PK
		public int StoryId { get; set; }

		//FK
		public int TripId { get; set; }
		public virtual Trip Trip { get; set; }
		
		//FK
		public int DestinationId { get; set; }
		public virtual Destination Destination { get; set; }


		//Local Items
		public string Description { get; set; }

		public DateTime Datum { get; set; }


		//If this class is FK in other table create ICollection to enable
		//searching/listing the entries that have this class as FK
		public virtual ICollection<Picture> Pictures { get; set; }
	}
}
