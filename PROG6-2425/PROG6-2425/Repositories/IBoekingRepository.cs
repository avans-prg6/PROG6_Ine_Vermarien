using PROG6_2425.Models;

namespace PROG6_2425.Repositories;

public interface IBoekingRepository
{
    Task<IEnumerable<Beestje>> GetBeestjesByDatumAsync(DateTime datum);

    Task<Boeking> CreateBoekingAsync(Boeking boeking);

    Task<IEnumerable<Boeking>> GetBoekingenByUserId(string id);
    Task<Boeking?> GetBoekingById(int id);
    
    

}