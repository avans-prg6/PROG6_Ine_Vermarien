using Microsoft.AspNetCore.Identity;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Repositories;

public interface IAccountRepository
{
    Task<List<KlantenKaartType>> GetKlantenKaartTypesAsync();
    Task<bool> CreateAccountAsync(AccountBeheerVM model, string wachtwoord);
    Task<IdentityUser?> GetUserByEmailAsync(string email);
    Task<IdentityUser?> GetUserByNameAsync(string name);
    Task<Account?> GetUserAccountByName(string name);

}