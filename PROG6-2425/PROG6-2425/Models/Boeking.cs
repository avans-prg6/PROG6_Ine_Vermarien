namespace PROG6_2425.Models;

public class Boeking
{
    public int BoekingId { get; set; }
    public DateTime Datum { get; set; }
    public decimal UiteindelijkePrijs { get; set; }
    public decimal KortingPercentage { get; set; }
    // Andere eventuele eigenschappen en navigatie-eigenschappen
    
    // Relatie met beestjes
    public int BeestjeId { get; set; }
    public Beestje Beestje { get; set; }
    
    // Relatie met accounts (gebruikers)
    public string AccountId { get; set; }
    public Account Account { get; set; }
}