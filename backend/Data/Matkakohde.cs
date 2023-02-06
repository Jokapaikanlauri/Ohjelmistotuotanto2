using Microsoft.Build.Framework;

namespace backend.Data
{
    public class Matkakohde
    {
        //Primary key
        public int MatkakohdeId { get; set; }

        //Items
        [Required]
        public string Nimi { get; set; }
        [Required]
        public string Maa { get; set; }


        public string? Paikkakunta { get; set; }
        public string? Kuvaus { get; set; }
        public string? Kuva { get; set; }
    }
}
