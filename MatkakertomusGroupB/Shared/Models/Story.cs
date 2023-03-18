using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace MatkakertomusGroupB.Shared.Models
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
		[Required]
		public string Description { get; set; }
		[Required]
		[Display(Name = "Datum")]
		[DataType(DataType.Date)]
		public DateTime Datum { get; set; }


		//If this class is FK in other table create ICollection to enable
		//searching/listing the entries that have this class as FK
		public virtual ICollection<Picture> Pictures { get; set; }
	}
}
