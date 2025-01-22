using PROG6_2425.Models;

namespace PROG6_2425.Repositories;

public interface IBeestjeRepository
{
    public IEnumerable<Beestje> GetAll();
    Beestje GetById(int id);
    IEnumerable<Beestje> GetBeestjesByIds(List<int> beestjeIds);
    void Create(Beestje beestje);
    void Update(Beestje beestje);
    void Delete(int id);
}