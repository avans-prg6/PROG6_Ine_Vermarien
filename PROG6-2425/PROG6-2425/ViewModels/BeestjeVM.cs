using System.ComponentModel.DataAnnotations;

namespace PROG6_2425.ViewModels;

public class BeestjeVM
{
    public int BeestjeId { get; set; }
    [Required(ErrorMessage = "Naam is verplicht")]
    public string Naam { get; set; }

    [Required(ErrorMessage = "Type is verplicht")]
    public string Type { get; set; }
    
    [Required(ErrorMessage = "Prijs is verplicht")]
    public decimal Prijs { get; set; }
    
    public string AfbeeldingUrl { get; set; }
}