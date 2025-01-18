using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG6_2425.Data;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BeestFeestDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountRepository(BeestFeestDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<List<KlantenKaartType>> GetKlantenKaartTypesAsync()
    {
        return await _dbContext.KlantenKaartTypes.ToListAsync();
    }
    
    public async Task<bool> CreateAccountAsync(AccountBeheerVM model, string wachtwoord)
    {
        var user = new Account
        {
            Naam = model.Naam,
            Adres = model.Adres,
            Email = model.Email,
            TelefoonNummer = model.TelefoonNummer,
            UserName = model.Email
        };

        var result = await _userManager.CreateAsync(user, wachtwoord);

        // Maak een klantenkaart aan en koppel deze aan het account
        var klantenKaart = new KlantenKaart
        {
            KlantenKaartTypeId = model.KlantenKaartTypeId,
            AccountId = user.Id
        };

        _dbContext.KlantenKaarten.Add(klantenKaart);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<IdentityUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}
