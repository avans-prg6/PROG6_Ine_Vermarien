using PROG6_2425.Models;

namespace PROG6_2425.Repositories;

public interface IBoekingRepository
{
    public IEnumerable<Beestje> GetBeestjesByDatum(DateTime datum);
    public void CreateBoeking(Boeking boeking);
    public List<Boeking> GetBoekingenByUserId(string id);
    public Boeking GetBoekingById(int id);
    public void Delete(int boekingId);




}