using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
    
     private BoekingVM GetBoekingVMFromSession()
    {
        var modelJson = HttpContext.Session.GetString("BoekingVM");
        if (string.IsNullOrEmpty(modelJson))
        {
            return new BoekingVM();
        }

        return JsonConvert.DeserializeObject<BoekingVM>(modelJson);
    }

    private void SaveBoekingVMToSession(BoekingVM model)
    {
        var modelJson = JsonConvert.SerializeObject(model);
        HttpContext.Session.SetString("BoekingVM", modelJson);
    }

    public IActionResult BoekingWizard(int step = 1)
    {
        var model = GetBoekingVMFromSession();
        model.CurrentStep = step;

        switch (step)
        {
            case 1:
                return AddDate(model);
            case 2:
                return AddBeestjes(model);
            case 3:
                return AddAccountInfo(model);
            case 4:
                return ConfirmBoeking(model);
            default:
                return View(model);
        }
    }

    [HttpPost]
    public IActionResult BoekingWizard(BoekingVM model)
    {
        if (model.CurrentStep == 4 && !ModelState.IsValid)
        {
            return ProcessStep4(model);
        }

        SaveBoekingVMToSession(model);

        switch (model.CurrentStep)
        {
            case 1:
                return ProcessStep1(model);
            case 2:
                return ProcessStep2(model);
            case 3:
                return ProcessStep3(model);
            case 4:
                return ProcessStep4(model);
            default:
                return View(model);
        }
    }

    [HttpGet("BoekingWizard/Step1")]
    private IActionResult AddDate(BoekingVM model)
    {
        return View(model);
    }

    [HttpPost("BoekingWizard/Step1")]
    private IActionResult ProcessStep1(BoekingVM model)
    {
        HttpContext.Session.SetString("Datum", model.Datum.ToString());
        SaveBoekingVMToSession(model);
        return RedirectToAction("BoekingWizard", new { step = 2 });
    }

    [HttpGet("BoekingWizard/Step2")]
    private IActionResult AddBeestjes(BoekingVM model)
    {
        var datumString = HttpContext.Session.GetString("Datum");
        if (DateTime.TryParse(datumString, out DateTime datum))
        {
            model.Datum = datum;
            var beschikbareBeestjes = _boekingRepository.GetBeestjesByDatum(datum);
            model.BeschikbareBeestjes = beschikbareBeestjes.ToList();
        }
        return View(model);
    }

    [HttpPost("BoekingWizard/Step2")]
    private IActionResult ProcessStep2(BoekingVM model)
    {
        var datumString = HttpContext.Session.GetString("Datum");
        if (DateTime.TryParse(datumString, out DateTime datum))
        {
            // Controleer of er geselecteerde beestjes zijn
            if (model.GeselecteerdeBeestjesIds != null && model.GeselecteerdeBeestjesIds.Any())
            {
                model.GekozenBeestjes = _beestjeRepository.GetBeestjesByIds(model.GeselecteerdeBeestjesIds).ToList();
                model.UiteindelijkePrijs = model.GekozenBeestjes.Sum(b => b.Prijs); 
                
                // Opslaan in sessie
                HttpContext.Session.SetString("UiteindelijkePrijs", model.UiteindelijkePrijs.ToString("F2"));
                var beestjesJson = JsonConvert.SerializeObject(model.GekozenBeestjes);
                HttpContext.Session.SetString("Beestjes", beestjesJson);
            }
        }

        SaveBoekingVMToSession(model);
        // Ga naar de volgende stap
        return RedirectToAction("BoekingWizard", new { step = 3 });
    }

    [HttpGet("BoekingWizard/Step3")]
    private IActionResult AddAccountInfo(BoekingVM model)
    {
        var beestjesJson = HttpContext.Session.GetString("Beestjes");
        if (!string.IsNullOrEmpty(beestjesJson))
        {
            model.GekozenBeestjes = JsonConvert.DeserializeObject<List<Beestje>>(beestjesJson);
        }

        string datumString = HttpContext.Session.GetString("Datum");

        if (!datumString.IsNullOrEmpty() && DateTime.TryParse(datumString, out var datum))
        {
            model.Datum = datum;
        }
        if (User.Identity.IsAuthenticated)
        {
            var gebruiker = _userManager.FindByNameAsync(User.Identity.Name).Result;

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

        return View(model);
    }

    [HttpPost("BoekingWizard/Step3")]
    private IActionResult ProcessStep3(BoekingVM model)
    {
        var beestjesJson = HttpContext.Session.GetString("Beestjes");
        if (User.Identity.IsAuthenticated)
        {
            if (beestjesJson != null)
            {
                model.GekozenBeestjes = JsonConvert.DeserializeObject<List<Beestje>>(beestjesJson);
            }
            Account gebruiker = _accountRepository.GetUserAccountByName(User.Identity.Name).Result;

            if (gebruiker != null)
            {
                model.Naam = gebruiker.Naam;
                model.Adres = gebruiker.Adres;
                model.Email = gebruiker.Email;
                model.Telefoonnummer = gebruiker.TelefoonNummer;
            }
        }
        SaveBoekingVMToSession(model);
        return RedirectToAction("BoekingWizard", new { step = 4 });
    }

    [HttpGet("BoekingWizard/Step4")]
    private IActionResult ConfirmBoeking(BoekingVM model)
    {
        var beestjesJson = HttpContext.Session.GetString("Beestjes");
        var datumString = HttpContext.Session.GetString("Datum");
        var prijsString = HttpContext.Session.GetString("UiteindelijkePrijs");
        model.GekozenBeestjes = JsonConvert.DeserializeObject<List<Beestje>>(beestjesJson);
        if (DateTime.TryParse(datumString, out DateTime datum))
        {
            model.Datum = datum;
        }
        if (decimal.TryParse(prijsString, out decimal uiteindelijkePrijs))
        {
            model.UiteindelijkePrijs = uiteindelijkePrijs;
        }
        return View(model);
    }

    [HttpPost("BoekingWizard/Step4")]
    private IActionResult ProcessStep4(BoekingVM model)
    {
        var beestjesJson = HttpContext.Session.GetString("Beestjes");
        var datumString = HttpContext.Session.GetString("Datum");
        var prijsString = HttpContext.Session.GetString("UiteindelijkePrijs");

        model = GetBoekingVMFromSession();

        model.GekozenBeestjes = JsonConvert.DeserializeObject<List<Beestje>>(beestjesJson);
        if (DateTime.TryParse(datumString, out DateTime datum))
        {
            model.Datum = datum;
        }
        
        if (decimal.TryParse(prijsString, out decimal uiteindelijkePrijs))
        {
            model.UiteindelijkePrijs = uiteindelijkePrijs;
        }

        var boeking = CreateBoekingFromVM(model);
        Bevestigen(boeking);

        return RedirectToAction("UserBoekingen");
    }
    
    private Boeking CreateBoekingFromVM(BoekingVM model)
    {
        var geselecteerdeBeestjes = model.GekozenBeestjes;
        string userId = _userManager.GetUserId(User);

        if (User.Identity.IsAuthenticated)
        {
            return new Boeking
            {
                AccountId = userId,
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

    public void Bevestigen(Boeking newBoeking)
    {
        _boekingRepository.CreateBoeking(newBoeking);
        HttpContext.Session.Clear();
    }
    
    [HttpGet]
    public IActionResult Details(int id)
    {
        Boeking boeking = _boekingRepository.GetBoekingById(id);
        BoekingVM boekingVm = BoekingToBoekingVm(boeking);
        
        return View(boekingVm);
    }
    
    [Authorize]
    public IActionResult UserBoekingen()
    {
        var user = _userManager.GetUserAsync(User).Result;
        IEnumerable<Boeking> boekingen = _boekingRepository.GetBoekingenByUserId(user.Id);
        List<BoekingVM> boekingVms = new List<BoekingVM>();
        foreach (Boeking b in boekingen)
        {
            BoekingVM boekingVm = BoekingToBoekingVm(b);
            boekingVms.Add(boekingVm);
        }
        
        return View("BoekingIndex", boekingVms);
    }
    
    [Authorize(Roles = "Admin")] 
    [HttpGet]
    public IActionResult GetAllBoekingen()
    {
        var boekingen = _boekingRepository.GetAllBoekingen(); 

        List<BoekingVM> boekingVms = new List<BoekingVM>();
        foreach (Boeking b in boekingen)
        {
            BoekingVM boekingVm = BoekingToBoekingVm(b);
            boekingVms.Add(boekingVm);
        }

        return View("BoekingIndex", boekingVms);
    }
    
    public BoekingVM BoekingToBoekingVm(Boeking boeking)
    {
        BoekingVM boekingVm = new BoekingVM
        {
            BoekingId = boeking.BoekingId,
            Datum = boeking.Datum,
            Naam = boeking.Naam,
            Adres = boeking.Adres,
            Email = boeking.Email,
            Telefoonnummer = boeking.Telefoonnummer,
            UiteindelijkePrijs = boeking.UiteindelijkePrijs,
            KortingPercentage = boeking.KortingPercentage,
            GekozenBeestjes = boeking.Beestjes.Select(bb => bb.Beestje).ToList()
        };
        List<int> beestjeIds = new List<int>();
        foreach (BeestjeBoeking b in boeking.Beestjes)
        {
            beestjeIds.Add(b.BeestjeId);
        }
        
        List<Beestje> gekozenBeestjes = _beestjeRepository.GetBeestjesByIds(beestjeIds).ToList();
        boekingVm.GekozenBeestjes = gekozenBeestjes;
        return boekingVm;
    }
    
    [HttpPost]
    public IActionResult Delete(int id)
    {
        _boekingRepository.Delete(id);
        return RedirectToAction("UserBoekingen");
    }
}
    