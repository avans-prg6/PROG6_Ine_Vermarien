using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6_2425.Models;

public class Boeking
{
    public int BoekingId { get; set; }
    public DateTime Datum { get; set; }
    
    public decimal UiteindelijkePrijs { get; set; }
    public decimal KortingPercentage { get; set; }
    
    public string Naam { get; set; }
    public string Adres { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [Phone]
    public string Telefoonnummer { get; set; }
    
    // Relatie met BeestjeBoeking (many to many)
    public List<BeestjeBoeking> Beestjes { get; set; }

    
    // Relatie met accounts (gebruikers)
    [ForeignKey("Account")]
    public string? AccountId { get; set; }
    public Account? Account { get; set; }
}