using System.ComponentModel.DataAnnotations;

namespace ET21KM_Ohjelmistotuotanto2_GroupB.Server.Models
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
