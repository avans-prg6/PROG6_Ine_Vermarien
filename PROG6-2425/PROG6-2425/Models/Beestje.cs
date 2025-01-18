using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6_2425.Models;

public class Beestje
{
        public int BeestjeId { get; set; }
        public string Naam { get; set; }
        public string Type { get; set; }
        public decimal Prijs { get; set; }
        public string AfbeeldingUrl { get; set; }
        
        public ICollection<Boeking> Boekingen { get; set; }
}