namespace PROG6_2425.ViewModels;

public class BoekingOverzichtVM
{
    public int BoekingId { get; set; }
    public DateTime Datum { get; set; }
    public List<BeestjeVM> Beestjes { get; set; } = new List<BeestjeVM>();
    public decimal TotaalPrijs { get; set; }
}
