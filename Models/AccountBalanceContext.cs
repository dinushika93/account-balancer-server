using Microsoft.EntityFrameworkCore;

namespace AccountBalancer.Models
{
    public class AccountBalanceContext : DbContext
    {
        public AccountBalanceContext(DbContextOptions<AccountBalanceContext> options)
            : base(options)
        {
        }

        public DbSet<AccountBalance> AccountBalances { get; set; }
    }
}
