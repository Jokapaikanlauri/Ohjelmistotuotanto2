using System.ComponentModel.DataAnnotations;

namespace Matkakertomus_webapp_GroupB.Data
{
    public class Kuva
    {
        //Primary key
        public int KuvaId { get; set; }

        //Foreign key
        public int TarinaId { get; set; }
        public virtual Tarina Tarina { get; set; }

        //Items
        [Required]
        public string Kuvanimi { get; set; }
    }
}
