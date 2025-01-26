namespace PROG6_2425.Models;

public class BeestjeBoeking
{
    public int BeestjeBoekingId { get; set; }

    // Verwijzing naar het Beestje
    public int BeestjeId { get; set; }
    public Beestje Beestje { get; set; }

    // Verwijzing naar de Boeking
    public int BoekingId { get; set; }
    public Boeking Boeking { get; set; }

    // Specifieke eigenschappen voor het beestje in de boeking
    public string Naam { get; set; }
    public decimal Prijs { get; set; }
}