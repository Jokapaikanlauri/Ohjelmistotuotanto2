using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace MatkakertomusGroupB.Shared.Models
{
	public class TripDTO
	{
		//PK
		public int? TripId { get; set; }

		//FK
        public string TravellerId { get; set; }

		//Local Items
		public DateTime? DatumStart { get; set; }
		public DateTime? DatumEnd { get; set; }
		public Boolean Private { get; set; }
	}
}
