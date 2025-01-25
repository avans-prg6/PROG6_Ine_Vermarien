using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services;

public class DiscountService
{
    private readonly Random _random = new Random();
    
    public decimal CalculateTotalDiscount(BoekingVM boeking, Account gebruiker)
    {
        boeking.KortingDetails.Clear();
        if (boeking == null) throw new ArgumentNullException(nameof(boeking));

        decimal totaleKorting = 0;
        
        // 1. 10% korting bij 3 beestjes van hetzelfde type
        var typeKorting = CalculateTypeDiscount(boeking);
        if (typeKorting > 0)
        {
            boeking.KortingDetails.Add("10% korting omdat er 3 dieren van hetzelfde type zijn.");
        }
        totaleKorting += typeKorting;

        // 2. Kans op korting met 'Eend'
        var eendKorting = CalculateEendDiscount(boeking);
        if (eendKorting > 0)
        {
            boeking.KortingDetails.Add("50% korting omdat er een 'Eend' aanwezig is en de kans in jouw voordeel was.");
        }
        totaleKorting += eendKorting;
        
        // 3. 15% korting voor boekingen op maandag of dinsdag
        var weekDayKorting = CalculateWeekdayDiscount(boeking);
        if (weekDayKorting > 0)
        {
            boeking.KortingDetails.Add("15% voor een boeking op maandag of dinsdag");
        }

        totaleKorting += weekDayKorting;

        // 4. Extra korting op basis van letters in de naam
        var letterKorting = CalculateLetterDiscount(boeking);
        if (letterKorting > 0)
        {
            boeking.KortingDetails.Add("2% per opvolgende letter");
        }
        totaleKorting += letterKorting;

        
        // 5. 10% korting voor klanten met een klantenkaart
        var klantenkaartKorting = CalculateKlantenKaartDiscount(gebruiker);
        if (klantenkaartKorting > 0)
        {
            boeking.KortingDetails.Add("10% omdat je een klantenkaart hebt");
        }
        totaleKorting += klantenkaartKorting;

        // Zorg dat de totale korting nooit meer dan 60% is
        return Math.Min(totaleKorting, 60);
    }

    private decimal CalculateTypeDiscount(BoekingVM boeking)
    {
        var korting = boeking.GekozenBeestjes
            .GroupBy(b => b.Type)
            .Where(g => g.Count() >= 3)
            .Sum(_ => 10); // 10% korting voor elk type met 3+ dieren

        return korting;
    }

    private decimal CalculateEendDiscount(BoekingVM boeking)
    {
        bool heeftEend = boeking.GekozenBeestjes.Any(b => b.Naam.Equals("Eend", StringComparison.OrdinalIgnoreCase));
        if (heeftEend && _random.Next(1, 7) == 1) // 1 op 6 kans
        {
            return 50; // 50% korting
        }

        return 0;
    }

    private decimal CalculateWeekdayDiscount(BoekingVM boeking)
    {
        var dag = boeking.Datum.DayOfWeek;
        if (dag == DayOfWeek.Monday || dag == DayOfWeek.Tuesday)
        {
            return 15; // 15% korting
        }

        return 0;
    }

    private decimal CalculateLetterDiscount(BoekingVM boeking)
    {
        if (boeking.GekozenBeestjes == null) return 0;

        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int korting = 0;
        foreach (Beestje beestje in boeking.GekozenBeestjes)
        {
            string name = beestje.Naam.ToUpper();
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == letters[i])
                {
                    korting += 2;
                }
                else
                {
                    break;
                }
            }
        }

        return korting;
    }

    private decimal CalculateKlantenKaartDiscount(Account gebruiker)
    {
        if (gebruiker?.KlantenKaart != null)
        {
            return 10; // 10% korting voor klanten met een klantenkaart
        }

        return 0;
    }
}