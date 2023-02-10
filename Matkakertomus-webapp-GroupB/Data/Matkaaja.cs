using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Matkakertomus_webapp_GroupB.Data
{
    public class Matkaaja : IdentityUser
    {
        public string? Etunimi { get; set; }
        public string? Sukunimi { get; set; }
        public string? Paikkakunta { get; set; }
        public string? Esittely { get; set; }
        public string? Kuva { get; set; }
    }
}
