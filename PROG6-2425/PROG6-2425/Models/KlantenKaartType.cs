using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace PROG6_2425.Models;

public class KlantenKaartType
{
    public int Id { get; set; }
    public string Naam { get; set; }
}