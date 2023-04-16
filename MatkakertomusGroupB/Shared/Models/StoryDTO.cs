using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MatkakertomusGroupB.Shared.Models
{
    public class StoryDTO
    {
        //PK
        public int? StoryId { get; set; }

        //FK
        public int TripId { get; set; }

        //FK
        public int DestinationId { get; set; }


        //Local Items
        public string Description { get; set; }
        public DateTime Datum { get; set; }

        public override string? ToString()
        {
            var sb = new StringBuilder();
            sb.Append(StoryId.ToString());
            sb.Append(',');
            sb.Append(TripId.ToString());
            sb.Append(',');
            sb.Append(DestinationId.ToString());
            sb.Append(',');
            sb.Append(Description);
            sb.Append(',');
            sb.Append(Datum.ToString());
            return sb.ToString();
        }
    }
}
