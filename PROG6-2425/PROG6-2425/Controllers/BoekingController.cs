using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROG6_2425.Models;
using PROG6_2425.Repositories;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Controllers;

public class BoekingController : Controller
{
    private readonly IBeestjeRepository _beestjeRepository;
    private readonly IBoekingRepository _boekingRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly UserManager<Account> _userManager;

    public BoekingController(IBeestjeRepository beestjeRepository, IBoekingRepository boekingRepository,
        UserManager<Account> userManager, IAccountRepository accountRepository)
    {
        _beestjeRepository = beestjeRepository;
        _boekingRepository = boekingRepository;
        _accountRepository = accountRepository;
        _userManager = userManager;
    }

    public async Task<IActionResult> BoekingWizard(int step = 1)
    {
        var model = new BoekingVM { CurrentStep = step };
        Console.WriteLine("steppost: " + model.CurrentStep);

        switch (step)
        {
            case 1:
                return await AddDateAsync(model);
            case 2:
                return await AddBeestjesAsync(model);
            case 3:
                return await AddAccountInfoAsync(model);
            case 4:
                return await ConfirmBoekingAsync(model);
            default:
                return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> BoekingWizard(BoekingVM model)
    {
        if (model.CurrentStep == 4 && !ModelState.IsValid)
        {
            ViewData["Step"] = model.CurrentStep;
            return View(model);
        }

        switch (model.CurrentStep)
        {
            case 1:
                return await ProcessStep1Async(model);
            case 2:
                return await ProcessStep2Async(model);
            case 3:
                return await ProcessStep3Async(model);
            case 4:
                return await ProcessStep4Async(model);
            default:
                return View(model);
        }
    }

    [HttpGet("BoekingWizard/Step1")]
    private async Task<IActionResult> AddDateAsync(BoekingVM model)
    {
        TempData["Datum"] = model.Datum;
        TempData.Keep("Datum");
        return View(model);
    }
    
    [HttpPost("BoekingWizard/Step1")]
    private async Task<IActionResult> ProcessStep1Async(BoekingVM model)
    {
        TempData["Datum"] = model.Datum;
        return RedirectToAction("BoekingWizard", new { step = 2 });
    }

    [HttpGet("BoekingWizard/Step2")]
    private async Task<IActionResult> AddBeestjesAsync(BoekingVM model)
    {
        if (TempData["Datum"] != null && DateTime.TryParse(TempData["Datum"].ToString(), out DateTime datum))
        {
            model.Datum = datum;
            var beschikbareBeestjes = await _boekingRepository.GetBeestjesByDatumAsync(datum);
            model.BeschikbareBeestjes = beschikbareBeestjes.ToList();
        }
        else
        {
            Console.WriteLine("Geen datum gevonden in TempData");
        }
        ViewData["Step"] = 2;
        return View(model);
    }
    
    [HttpPost("BoekingWizard/Step2")]
    private async Task<IActionResult> ProcessStep2Async(BoekingVM model)
    {
        if (TempData["Datum"] != null && DateTime.TryParse(TempData["Datum"].ToString(), out DateTime datum))
        {
            TempData.Keep("Datum");
            var beschikbareBeestjes = await _boekingRepository.GetBeestjesByDatumAsync(datum);
            model.GekozenBeestjes = beschikbareBeestjes.ToList();
            TempData["Beestjes"] = model.GekozenBeestjes;
            TempData.Keep("Beestjes");
        }

        return RedirectToAction("BoekingWizard", new { step = 3 });
    }

    [HttpGet("BoekingWizard/Step3")]
    private async Task<IActionResult> AddAccountInfoAsync(BoekingVM model)
    {
        if (User.Identity.IsAuthenticated)
        {
            TempData.Keep("Datum");
            TempData.Keep("Beestjes");
            var gebruiker = await _userManager.FindByNameAsync(User.Identity.Name);
            if (gebruiker != null)
            {
                model.Naam = gebruiker.Naam;
                model.Adres = gebruiker.Adres;
                model.Email = gebruiker.Email;
                model.Telefoonnummer = gebruiker.TelefoonNummer;
            }
            else
            {
                Console.WriteLine("Gebruiker niet gevonden");
            }
        }

        ViewData["Step"] = 3;
        return View(model);
    }
    
    [HttpPost("BoekingWizard/Step3")]
    private async Task<IActionResult> ProcessStep3Async(BoekingVM model)
    {
        if (User.Identity.IsAuthenticated)
        {
            TempData.Keep("Datum");
            TempData.Keep("Beestjes");
            var gebruiker = await _accountRepository.GetUserAccountByName(User.Identity.Name);
            if (gebruiker != null)
            {
                model.Naam = gebruiker.Naam;
                model.Adres = gebruiker.Adres;
                model.Email = gebruiker.Email;
                model.Telefoonnummer = gebruiker.TelefoonNummer;
            }
        }

        return RedirectToAction("BoekingWizard", new { step = 4 });
    }

    [HttpGet("BoekingWizard/Step4")]
    private async Task<IActionResult> ConfirmBoekingAsync(BoekingVM model)
    {
        TempData.Keep("Datum");
        TempData.Keep("Beestjes");
        ViewData["Step"] = 4;
        return View(model);
    }

    [HttpPost("BoekingWizard/Step4")]

    private async Task<IActionResult> ProcessStep4Async(BoekingVM model)
    {
        TempData.Keep("Datum");
        TempData.Keep("Beestjes");
        var boeking = CreateBoekingFromVM(model);
        return RedirectToAction("Bevestigen", boeking);
    }

    private Boeking CreateBoekingFromVM(BoekingVM model)
    {
        TempData.Keep("Beestjes");
        var geselecteerdeBeestjes = model.GekozenBeestjes;

        return new Boeking
        {
            Datum = model.Datum.Value,
            Naam = model.Naam,
            Adres = model.Adres,
            Email = model.Email,
            Telefoonnummer = model.Telefoonnummer,
            UiteindelijkePrijs = geselecteerdeBeestjes.Sum(b => b.Prijs),
            Beestjes = geselecteerdeBeestjes.Select(b => new BeestjeBoeking
            {
                BeestjeId = b.BeestjeId,
                Naam = b.Naam,
                Prijs = b.Prijs
            }).ToList()
        };
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Bevestigen(Boeking newBoeking)
    {
        await _boekingRepository.CreateBoekingAsync(newBoeking);
        return RedirectToAction("UserBoekingen");
    }

    public async Task<IActionResult> UserBoekingen()
    {
        var user = await _userManager.GetUserAsync(User);
        var boekingen = await _boekingRepository.GetBoekingenByUserId(user.Id);
        return View();
    }

    // public async Task<IActionResult> Index(DateTime datum)
    // {
    //     IEnumerable<Beestje> bezetteBeestjes = await _boekingRepository.GetBeestjesByDatumAsync(datum);
    //     IEnumerable<Beestje> beschikbareBeestjes = await _beestjeRepository.GetAllAsync();
    //
    //     var beschikbareBeestjesFiltered = beschikbareBeestjes
    //         .Where(b => !bezetteBeestjes.Any(bz => bz.BeestjeId == b.BeestjeId))
    //         .ToList();
    //     
    //     var model = new BoekingVM()
    //     {
    //         BeschikbareBeestjes = beschikbareBeestjes,
    //         Datum = datum
    //     };
    //     
    //     //als gebruiker al is ingelogd gegevens gelijk overnemen
    //     if (User.Identity.IsAuthenticated)
    //     {
    //         Account gebruiker = await _userManager.GetUserAsync(User);
    //         model.GebruikerId = gebruiker.Id;
    //         model.Naam = gebruiker.Naam;
    //         model.Adres = gebruiker.Adres;
    //         model.Email = gebruiker.Email;
    //         model.Telefoonnummer = gebruiker.TelefoonNummer;
    //     }
    //     
    //     return View(model);
    // }

    // [HttpPost]
    // public IActionResult KiesBeestjes(DateTime datum, List<int> beestjeIds)
    // {
    //     // Hier komt de logica om de gekozen beestjes op te slaan
    //     return RedirectToAction("GegevensInvoeren", new { datum = datum, beestjeIds = string.Join(",", beestjeIds) });
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> GegevensInvoeren(BoekingVM model, DateTime datum, string beestjeIds)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         var beestjeIdsList = beestjeIds.Split(',').Select(int.Parse).ToList();
    //         var geselecteerdeBeestjes = await _beestjeRepository.GetBeestjesByIdsAsync(beestjeIdsList);
    //
    //         var boeking = new Boeking
    //         {
    //             Datum = datum,
    //             Naam = model.Naam,
    //             Email = model.Email,
    //             Adres = model.Adres,
    //             Telefoonnummer = model.Telefoonnummer,
    //             UiteindelijkePrijs = geselecteerdeBeestjes.Sum(b => b.Prijs),
    //             Beestjes = geselecteerdeBeestjes.Select(b => new BeestjeBoeking
    //             {
    //                 BeestjeId = b.BeestjeId,
    //                 Naam = b.Naam,
    //                 Prijs = b.Prijs
    //             }).ToList()
    //         };
    //
    //         await _boekingRepository.CreateBoekingAsync(boeking);
    //         return RedirectToAction("Overzicht", new { boekingId = boeking.BoekingId });
    //     }
    //
    //     return View(model);
    // }
    //
    // // Stap 4: Overzicht en bevestigen
    // public async Task<IActionResult> Overzicht(int boekingId)
    // {
    //     var boeking = await _boekingRepository.GetBoekingByIdAsync(boekingId);
    //     return View(boeking);
    // }
    //
}