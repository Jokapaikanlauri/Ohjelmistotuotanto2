using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatkakertomusGroupB.Shared.Models
{
    public class PictureDTO
    {
        public int? PictureId { get; set; }
        public int StoryId { get; set; }
        public string Image { get; set; }
    }
}
