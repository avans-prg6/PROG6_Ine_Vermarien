using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PROG6_2425.Data;

public class BeestFeestDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public BeestFeestDbContext(DbContextOptions<BeestFeestDbContext> options): base(options)
    {
    }
}