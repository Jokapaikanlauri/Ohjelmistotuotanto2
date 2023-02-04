using System.ComponentModel.DataAnnotations;

namespace backend.Data
{
    public class Matkaaja
    {
        //Primary key
        public int MatkaajaId { get; set; }

        //Items
        [Required]
        public string Nimimerkki { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Paikkakunta { get; set; }
        public string Esittely { get; set; }
        public string Kuva { get; set; }
    }
}
