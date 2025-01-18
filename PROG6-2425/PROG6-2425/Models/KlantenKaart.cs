namespace PROG6_2425.Models;

public class KlantenKaart
{
    public int Id { get; set; }
    public int KlantenKaartTypeId { get; set; }
    public KlantenKaartType KlantenKaartType { get; set; }
    
    // Relatie met account
    public string AccountId { get; set; }
    public Account Account { get; set; }
}