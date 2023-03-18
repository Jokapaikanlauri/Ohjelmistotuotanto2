using System.ComponentModel.DataAnnotations;

namespace MatkakertomusGroupB.Shared.Models
{
	public class Picture
	{
		//PK
		public int PictureId { get; set; }

		//FK
		public int StoryId { get; set; }
		public virtual Story Story { get; set; }


		//Local Items
		[Required]
		public string Image { get; set; }

	}
}
