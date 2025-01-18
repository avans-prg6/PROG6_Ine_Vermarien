using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using PROG6_2425.Models;

namespace PROG6_2425.ViewModels;

public class AccountBeheerVM
{
    [Required(ErrorMessage = "Naam VM")]
    public string Naam { get; set; }
    [Required(ErrorMessage = "Adres VM")]
    public string Adres { get; set; }
    [Required(ErrorMessage = "email VM")]
    public string Email { get; set; }
    [Required(ErrorMessage = "tellie VM")]

    public string TelefoonNummer { get; set; }
    
    public string Wachtwoord { get; set; }
    [Required(ErrorMessage = "klantenkaart VM")]
    public int KlantenKaartTypeId { get; set; }
    public List<KlantenKaartType> KlantenKaartTypes { get; set; }

}