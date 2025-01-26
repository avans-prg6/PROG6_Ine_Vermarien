using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PROG6_2425.Models;

public class Account : IdentityUser
{
    public string Naam { get; set; }
    public string Adres { get; set; }
    [Phone]
    public string TelefoonNummer { get; set; }
    public KlantenKaart KlantenKaart { get; set; }

}