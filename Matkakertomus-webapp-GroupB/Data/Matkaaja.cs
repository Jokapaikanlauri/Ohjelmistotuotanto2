using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Matkakertomus_webapp_GroupB.Data
{
    public class Matkaaja : IdentityUser
    {
        [PersonalData]
        public string? Etunimi { get; set; }
        [PersonalData]
        public string? Sukunimi { get; set; }
        [PersonalData]
        public string? Paikkakunta { get; set; }
        [PersonalData]
        public string? Esittely { get; set; }
        [PersonalData]
        public string? Kuva { get; set; }
    }
}
