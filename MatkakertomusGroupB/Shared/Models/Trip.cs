﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace MatkakertomusGroupB.Shared.Models
{
	public class Trip
	{
		//PK
		[Key]
		public int TripId { get; set; }

		//FK
		[ForeignKey("Traveller")]
        public string TravellerId { get; set; }
		public virtual Traveller Traveller { get; set; }

		//Local Items

		[Display(Name = "Starting date")]
		[DataType(DataType.Date)]
		public DateTime? DatumStart { get; set; }

		[Display(Name = "Ending date")]
		[DataType(DataType.Date)]
		public DateTime? DatumEnd { get; set; }

		[Required]
		public Boolean Private { get; set; }


		//If this class is FK in other table create ICollection to enable
		//searching/listing the entries that have this class as FK
		public virtual ICollection<Story> Stories { get; set; }
	}
}
