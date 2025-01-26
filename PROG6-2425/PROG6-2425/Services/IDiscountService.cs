using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services;

public interface IDiscountService
{
    public decimal CalculateTotalDiscount(BoekingVM boeking, Account gebruiker);

    // protected decimal CalculateTypeDiscount(BoekingVM boeking);
    //
    // protected decimal CalculateEendDiscount(BoekingVM boeking);
    //
    // protected decimal CalculateWeekdayDiscount(BoekingVM boeking);
    //
    // protected decimal CalculateLetterDiscount(BoekingVM boeking);
    //
    // protected decimal CalculateKlantenKaartDiscount(Account gebruiker);


}