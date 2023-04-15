using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace MatkakertomusGroupB.Shared.Models
{
    public class DestinationDTO
    {
        public int? DestinationId { get; set; }

        //Local Items
        public string Name { get; set; }
        public string Country { get; set; }
        public string? Municipality { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
