using System.ComponentModel.DataAnnotations;

namespace Matkakertomus_webapp_GroupB.Data
{
    public class Tarina
    {
        //Public key
        public int TarinaId { get; set; }

        //Foreign key
        public int MatkakohdeId { get; set; }
        public virtual Matkakohde Matkakohde { get; set; }
        public int MatkaId { get; set; }
        public virtual Matka Matka { get; set; }

        //Items
        [Required]
        public DateTime Pvm { get; set; }
        [Required]
        public string Teksti { get; set; }
    }
}
