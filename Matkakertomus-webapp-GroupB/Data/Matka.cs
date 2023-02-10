using System.ComponentModel.DataAnnotations;

namespace Matkakertomus_webapp_GroupB.Data
{
    public class Matka
    {
        //Primary key
        public int MatkaId { get; set; }

        //Foreign key
        public int MatkaajaId { get; set; }
        public virtual Matkaaja Matkaaja { get; set; }

        //Items
        [Required]
        public bool Yksityinen { get; set; }

        public DateTime? Alkupvm { get; set; }
        public DateTime? Loppupvm { get; set; }
    }
}
