using PROG6_2425.Models;

namespace PROG6_2425.ViewModels;

public class BoekingVM
{
    
    public int BoekingId { get; set; }
    
    public DateTime? Datum { get; set; }
    
    public string GebruikerId { get; set; }
    public string Naam { get; set; }
    public string Adres { get; set; }
    public string Email { get; set; }
    public string Telefoonnummer { get; set; }
    
    public decimal UiteindelijkePrijs { get; set; }
    
    public decimal KortingPercentage { get; set; }
    
    public List<Beestje> BeschikbareBeestjes { get; set; }
    
    public List<int> GeselecteerdeBeestjesIds { get; set; }
    public List<Beestje> GekozenBeestjes { get; set; }
    
    public int CurrentStep { get; set; } = 1;
    
}