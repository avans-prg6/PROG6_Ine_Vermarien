using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROG6_2425.Models;
using PROG6_2425.Repositories;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAccountRepository _accountRepository;
    private readonly SignInManager<IdentityUser> _signInManager;
    

    public AccountController(UserManager<IdentityUser> userManager, IAccountRepository accountRepository, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _accountRepository = accountRepository;
        _signInManager = signInManager;
    }

    public IActionResult Register()
    {
        return View();
    }
    public async Task<IActionResult> Register(RegisterVM input)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = input.Email, Email = input.Email };
            var result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect("/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View();
    }
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAccount()
    {
        var viewModel = new AccountBeheerVM
        {
            KlantenKaartTypes = await _accountRepository.GetKlantenKaartTypesAsync()
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateAccount(AccountBeheerVM model)
    {
        // Make account and klantenkaart
        string errorMessage = string.Empty;
        var wachtwoord = GenerateRandomPassword();
        var success = await _accountRepository.CreateAccountAsync(model, wachtwoord);

        if (success)
        {
            TempData["Wachtwoord"] = wachtwoord;
            return RedirectToAction("CreateAccount");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "er is een fout opgetreden tijdens het aanmaken van een account");
            return View(model);
        }
    }
    
    private string GenerateRandomPassword()
    {
        // Password generation logic
        var passwordLength = 12;
        var random = new Random();
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        return new string(Enumerable.Range(1, passwordLength)
            .Select(_ => validChars[random.Next(validChars.Length)])
            .ToArray());
    }
    
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM Input)
    {
        if (ModelState.IsValid)
        {
            var result =
                await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return LocalRedirect("/");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
        }

        return View();
    }

    public async Task<IActionResult> Logout()
    {
        _signInManager.SignOutAsync();
        return LocalRedirect("/Account/Login");
    }

    public async Task<Account?> GetUserAccountById(string id)
    {
        return await _accountRepository.GetUserAccountByName(id);
    }

}