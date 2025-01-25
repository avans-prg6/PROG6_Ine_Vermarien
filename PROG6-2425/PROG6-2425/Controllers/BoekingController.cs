using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PROG6_2425.Models;
using PROG6_2425.Repositories;
using PROG6_2425.Services;
using PROG6_2425.Validators;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Controllers;

public class BoekingController : Controller
{
    private readonly IBeestjeRepository _beestjeRepository;
    private readonly IBoekingRepository _boekingRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly DiscountService _discountService;

    private readonly UserManager<Account> _userManager;

    private const string SessionKey = "BoekingVM";

    public BoekingController(IBeestjeRepository beestjeRepository, IBoekingRepository boekingRepository,
        UserManager<Account> userManager, IAccountRepository accountRepository, DiscountService discountService)
    {
        _beestjeRepository = beestjeRepository;
        _boekingRepository = boekingRepository;
        _accountRepository = accountRepository;
        _discountService = discountService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult BoekingWizard()
    {
        var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey) ?? new BoekingVM();
        return RedirectToAction("_BoekingWizardStep1");
    }

    public IActionResult _BoekingWizardSidebar()
    {
        var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey) ?? new BoekingVM();
        return PartialView(sessionModel);
    }

    [HttpGet]
    public IActionResult _BoekingWizardStep1()
    {
        var model = new Step1VM();
        return PartialView(model);
    }

    [HttpPost]
    public IActionResult _BoekingWizardStep1(Step1VM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView(model);
        }

        var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey) ?? new BoekingVM();
        sessionModel.Datum = model.Datum.Value;
        HttpContext.Session.Set(SessionKey, sessionModel);

        return RedirectToAction("_BoekingWizardStep2");
    }

    [HttpGet]
    public IActionResult _BoekingWizardStep2()
    {
        var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey);

        if (sessionModel.Datum == null)
        {
            return RedirectToAction("_BoekingWizardStep1");
        }

        var step2 = new Step2VM
        {
            BeschikbareBeestjes = _boekingRepository.GetBeestjesByDatum(sessionModel.Datum).ToList()
        };

        var model = new Step2WrapperVM
        {
            Step2 = step2,
            Overzicht = sessionModel
        };

        return PartialView(model);
    }

    [HttpPost]
public IActionResult _BoekingWizardStep2([Bind(Prefix = "Step2")] Step2VM model)
{
    var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey);
    var user = _userManager.GetUserAsync(User).Result;

    if (user != null)
    {
        sessionModel.GebruikerId = user.Id;
    }

    Step2WrapperVM wrapperVM = new Step2WrapperVM
    {
        Step2 = model,
        Overzicht = sessionModel
    };

    model.BeschikbareBeestjes = _boekingRepository.GetBeestjesByDatum(sessionModel.Datum).ToList();

    // Validatie
    var validator = HttpContext.RequestServices.GetService<IValidator<Step2VM>>();
    var validationContext = new ValidationContext(model, null, null)
    {
        Items =
        {
            { "Datum", wrapperVM.Overzicht.Datum },
            { "User", user },
            { "Beestjes", model.BeschikbareBeestjes }
        }
    };

    var customValidationResults = validator.Validate(wrapperVM.Step2, validationContext);

    foreach (var result in customValidationResults)
    {
        ModelState.AddModelError("Step2." + result.MemberNames.First(), result.ErrorMessage);
    }

    if (!ModelState.IsValid)
    {
        return PartialView(wrapperVM);
    }

    // Update sessiemodel met gekozen beestjes
    sessionModel.GekozenBeestjes = model.BeschikbareBeestjes
        .Where(b => model.GeselecteerdeBeestjesIds.Contains(b.BeestjeId))
        .ToList();

    sessionModel.UiteindelijkePrijs = sessionModel.GekozenBeestjes.Sum(b => b.Prijs);

    // Pas korting toe
    var discountCalculator = HttpContext.RequestServices.GetService<DiscountService>();
    var kortingPercentage = discountCalculator.CalculateTotalDiscount(sessionModel, user);

    sessionModel.KortingPercentage = kortingPercentage;
    sessionModel.UiteindelijkePrijs -= sessionModel.UiteindelijkePrijs * (kortingPercentage / 100);

    // Update de prijzen in het huidige viewmodel
    model.TotalePrijs = sessionModel.GekozenBeestjes.Sum(b => b.Prijs);
    model.KortingPercentage = kortingPercentage;
    model.UiteindelijkePrijs = sessionModel.UiteindelijkePrijs;

    HttpContext.Session.Set(SessionKey, sessionModel);

    return RedirectToAction("_BoekingWizardStep3");
}

    [HttpGet]
    public IActionResult _BoekingWizardStep3()
    {
        Step3VM step3 = new Step3VM();
        var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey);

        var user = _userManager.GetUserAsync(User).Result;
        if (user != null)
        {
            step3.GebruikerId = user.Id;
            step3.Naam = user.Naam;
            step3.Adres = user.Adres;
            step3.Email = user.Email;
            step3.Telefoonnummer = user.TelefoonNummer;
        }

        var model = new Step3WrapperVM
        {
            Step3 = step3,
            Overzicht = sessionModel
        };

        return PartialView(model);
    }

    [HttpPost]
    public IActionResult _BoekingWizardStep3([Bind(Prefix = "Step3")] Step3VM model)
    {
        var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey);

        if (!ModelState.IsValid)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                }
            }

            var wrapperVM = new Step3WrapperVM
            {
                Step3 = model,
                Overzicht = sessionModel
            };
            return View(wrapperVM);
        }

        sessionModel.Naam = model.Naam;
        sessionModel.Adres = model.Adres;
        sessionModel.Email = model.Email;
        sessionModel.Telefoonnummer = model.Telefoonnummer;
        HttpContext.Session.Set(SessionKey, sessionModel);
        return RedirectToAction("_BoekingWizardStep4");
    }

    [HttpGet]
    public IActionResult _BoekingWizardStep4()
    {
        var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey);
        return PartialView(sessionModel);
    }

    [HttpPost]
    public IActionResult _BoekingWizardStep4(BoekingVM model)
    {
        var sessionModel = HttpContext.Session.Get<BoekingVM>(SessionKey);

        if (sessionModel == null)
        {
            return RedirectToAction("_BoekingWizardStep1");
        }

        // Vul de ontbrekende gegevens opnieuw in
        model.GekozenBeestjes = sessionModel.GekozenBeestjes;
        model.BeschikbareBeestjes = sessionModel.BeschikbareBeestjes;
        model.UiteindelijkePrijs = sessionModel.UiteindelijkePrijs;
        model.Datum = sessionModel.Datum;
        model.Naam = sessionModel.Naam;
        model.Email = sessionModel.Email;
        model.Adres = sessionModel.Adres;
        model.Telefoonnummer = sessionModel.Telefoonnummer;

        Console.WriteLine($"Datum: {model.Datum}, Naam: {model.Naam}, Adres: {model.Adres}");

        // Controleer of het model geldig is
        if (!ModelState.IsValid)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                }
            }

            return View(sessionModel); // Zorg dat je sessiemodel teruggeeft
        }

        // Maak een boeking aan en bevestig deze
        var boeking = CreateBoekingFromVM(model);
        Bevestigen(boeking);

        // (Optioneel) Wis de sessie
        HttpContext.Session.Clear();

        return RedirectToAction("UserBoekingen");
    }

    public void Bevestigen(Boeking newBoeking)
    {
        _boekingRepository.CreateBoeking(newBoeking);
    }

    public Boeking CreateBoekingFromVM(BoekingVM model)
    {
        var userId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : null;

        return new Boeking
        {
            AccountId = userId,
            Datum = model.Datum,
            Naam = model.Naam,
            Adres = model.Adres,
            Email = model.Email,
            Telefoonnummer = model.Telefoonnummer,
            UiteindelijkePrijs = model.GekozenBeestjes.Sum(b => b.Prijs),
            Beestjes = model.GekozenBeestjes.Select(b => new BeestjeBoeking
            {
                BeestjeId = b.BeestjeId,
                Naam = b.Naam,
                Prijs = b.Prijs
            }).ToList()
        };
    }

    public IActionResult Details(int id)
    {
        Boeking boeking = _boekingRepository.GetBoekingById(id);
        BoekingVM boekingVm = BoekingToBoekingVm(boeking);

        return View(boekingVm);
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

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _boekingRepository.Delete(id);
        return RedirectToAction("UserBoekingen");
    }

    private BoekingVM BoekingToBoekingVm(Boeking boeking)
    {
        return new BoekingVM
        {
            BoekingId = boeking.BoekingId,
            Datum = boeking.Datum,
            Naam = boeking.Naam,
            Adres = boeking.Adres,
            Email = boeking.Email,
            Telefoonnummer = boeking.Telefoonnummer,
            UiteindelijkePrijs = boeking.UiteindelijkePrijs,
            GekozenBeestjes = boeking.Beestjes.Select(bb => bb.Beestje).ToList()
        };
    }
}