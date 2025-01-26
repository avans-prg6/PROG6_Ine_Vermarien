using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG6_2425.Models;
using PROG6_2425.Repositories;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Controllers;

public class BeestjeController : Controller
{
    private readonly IBeestjeRepository _beestjeRepository;

    public BeestjeController(IBeestjeRepository beestjeRepository)
    {
        _beestjeRepository = beestjeRepository;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Index()
    {
        var beestjes = _beestjeRepository.GetAll();
        return View(beestjes);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create(BeestjeVM _beestje)
    {
        if (!ModelState.IsValid)
        {
            return View(_beestje);
        }

        Beestje beestje = new Beestje
        {
            Naam = _beestje.Naam,
            Type = _beestje.Type,
            Prijs = _beestje.Prijs,
            AfbeeldingUrl = _beestje.AfbeeldingUrl
        };
        
        _beestjeRepository.Create(beestje);


        TempData["SuccessMessage"] = "Beestje succesvol aangemaakt!";
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        Beestje beestje = _beestjeRepository.GetById(id);
        if (beestje == null) return NotFound();

        // omzetten naar VM
        BeestjeVM model = new BeestjeVM
        {
            BeestjeId = beestje.BeestjeId,
            Naam = beestje.Naam,
            Type = beestje.Type,
            Prijs = beestje.Prijs,
            AfbeeldingUrl = beestje.AfbeeldingUrl
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Edit(BeestjeVM _beestje)
    {
        if (!ModelState.IsValid)
        {
            return View(_beestje);
        }

        // VM naar DB object
        Beestje beestje = new Beestje
        {
            BeestjeId = _beestje.BeestjeId,
            Naam = _beestje.Naam,
            Type = _beestje.Type,
            Prijs = _beestje.Prijs,
            AfbeeldingUrl = _beestje.AfbeeldingUrl
        };

        _beestjeRepository.Update(beestje);

        TempData["SuccessMessage"] = "Beestje succesvol bijgewerkt!";
        return RedirectToAction("Index");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        Beestje beestje = _beestjeRepository.GetById(id);
        if (beestje == null)
        {
            return NotFound();
        }

        _beestjeRepository.Delete(id);
        TempData["SuccessMessage"] = "Beestje is succesvol verwijderd.";
        return RedirectToAction("Index");
    }
}