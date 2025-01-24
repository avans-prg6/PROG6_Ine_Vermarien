using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services.DiscountRules;

public class WeekDayDiscountRule: IDiscountRule
{
    public decimal CalculateDiscount(BoekingVM boeking, decimal currentDiscountPercentage)
    {
        var dag = boeking.Datum.DayOfWeek;
        if (dag == DayOfWeek.Monday || dag == DayOfWeek.Tuesday)
        {
            return 15; // 15% korting
        }

        return 0;
    }
}