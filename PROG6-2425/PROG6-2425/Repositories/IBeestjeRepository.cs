using PROG6_2425.Models;

namespace PROG6_2425.Repositories;

public interface IBeestjeRepository
{
    Task<IEnumerable<Beestje>> GetAllAsync();
    Task<Beestje?> GetByIdAsync(int id);
    Task CreateAsync(Beestje beestje);
    Task UpdateAsync(Beestje beestje);
    Task DeleteAsync(int id);
}