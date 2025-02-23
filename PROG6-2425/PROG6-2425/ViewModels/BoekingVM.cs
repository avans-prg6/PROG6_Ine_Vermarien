﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PROG6_2425.Attributes;
using PROG6_2425.Models;

namespace PROG6_2425.ViewModels;

public class BoekingVM
{
    public int BoekingId { get; set; }
    
    //Step1
    [NoDateInPast] 
    public DateTime Datum { get; set; }

    //Step2
    public decimal UiteindelijkePrijs { get; set; }

    public decimal KortingPercentage { get; set; }
    
    public decimal TotalePrijs => GekozenBeestjes?.Sum(b => b.Prijs) ?? 0;

    public List<string> KortingDetails { get; set; } = new List<string>();

    public List<Beestje> BeschikbareBeestjes { get; set; }

    public List<Beestje> GekozenBeestjes { get; set; }
    
    //Step3
    public string? GebruikerId { get; set; }
    public string Naam { get; set; }
    public string Adres { get; set; }
    public string Email { get; set; }
    public string Telefoonnummer { get; set; }
}

public class Step1VM
{
    [NoDateInPast] 
    [Required]
    public DateTime? Datum { get; set; }
}


public class Step2WrapperVM
{
    public Step2VM Step2 { get; set; }
    public BoekingVM Overzicht { get; set; }
}



public class Step2VM
{
    public List<Beestje>? BeschikbareBeestjes { get; set; }
    [Required]
    public List<int> GeselecteerdeBeestjesIds { get; set; }
    public List<Beestje>? AlleBeestjes { get; set; }
    
    public decimal TotalePrijs{ get; set; }

    public decimal KortingPercentage { get; set; }
    public decimal UiteindelijkePrijs { get; set; } // met korting


}

public class Step3WrapperVM
{
    public Step3VM Step3 { get; set; }
    public BoekingVM Overzicht { get; set; }
}

public class Step3VM
{
    public string? GebruikerId { get; set; }
    public string Naam { get; set; }
    public string Adres { get; set; }
    public string Email { get; set; }
    public string Telefoonnummer { get; set; }
}