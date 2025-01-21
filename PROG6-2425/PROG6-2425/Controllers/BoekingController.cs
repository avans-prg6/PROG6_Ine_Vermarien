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

    public BoekingController(IBeestjeRepository beestjeRepository, IBoekingRepository boekingRepository, UserManager<Account> userManager, IAccountRepository accountRepository)
    {
        _beestjeRepository = beestjeRepository;
        _boekingRepository = boekingRepository;
        _accountRepository = accountRepository;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> BoekingWizard(int step = 1)
    {
        var model = new BoekingVM { CurrentStep = step};
        Console.WriteLine("steppost: "+ model.CurrentStep);
        if (step == 1)
        {
            TempData["Datum"] = model.Datum;
            // Stap 1: Laat gebruiker een datum selecteren
            return View(model);
        }
        
        if (step == 2 && TempData["Datum"].ToString() != "")
        {
            model.Datum = TempData["Datum"] as DateTime?;
            IEnumerable<Beestje> beschikbareBeestjes = await _boekingRepository.GetBeestjesByDatumAsync(model.Datum.Value);
            model.BeschikbareBeestjes = beschikbareBeestjes.ToList();
        }
        else if (step == 3 && User.Identity.IsAuthenticated)
        {
            // Vul gebruikersgegevens in als de gebruiker is ingelogd
            // var gebruiker = await _userManager.FindByNameAsync(User.Identity.Name);
            var gebruiker = await _userManager.FindByNameAsync(User.Identity.Name);
            if (gebruiker == null)
            {
                Console.WriteLine("geen gebruiker gevonde gekkie");
            }
            model.Naam = gebruiker.Naam;
            model.Adres = gebruiker.Adres;
            model.Email = gebruiker.Email;
            model.Telefoonnummer = gebruiker.TelefoonNummer;
        }
        
        ViewData["Step"] = step;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> BoekingWizard(BoekingVM model)
    {
        
        if (model.CurrentStep == 4 && !ModelState.IsValid)
        {
            ViewData["Step"] = model.CurrentStep;
            return View(model);
        }
        

        if (model.CurrentStep == 1)
        {
            // Sla de geselecteerde datum op in TempData
            TempData["Datum"] = model.Datum;
            model.Datum = TempData["Datum"] as DateTime ?;
            // Ga naar stap 2
            return RedirectToAction("BoekingWizard", new { step = 2 });
        }
        if (model.CurrentStep == 2)
        {
            var datum = TempData["Datum"] as DateTime?;
            TempData.Keep("Datum");
            if (datum.HasValue)
            {
                var beschikbareBeestjes = await _boekingRepository.GetBeestjesByDatumAsync(datum.Value);
                
                model.GekozenBeestjes = beschikbareBeestjes.ToList();
            }
            TempData["Beestjes"] = model.GekozenBeestjes;
            TempData.Keep("Beestjes");

            return RedirectToAction("BoekingWizard", new { step = 3 });

        }

        if (model.CurrentStep == 3)
        {
            // Stap 3: Vul gegevens in
            if (User.Identity.IsAuthenticated)
            {
                var gebruiker = await _accountRepository.GetUserAccountByName(User.Identity.Name);
                model.Naam = gebruiker.Naam;
                model.Adres = gebruiker.Adres;
                model.Email = gebruiker.Email;
                model.Telefoonnummer = gebruiker.TelefoonNummer;
            }
            return RedirectToAction("BoekingWizard", new { step = 4 });
        }

        if (model.CurrentStep == 4)
        {
            // Stap 4: Combineer data en maak boeking
            var boeking = CreateBoekingFromVM(model);

            return RedirectToAction("Bevestigen", boeking);
        }

        return View(model);
    }

    private Boeking CreateBoekingFromVM(BoekingVM model)
    {
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