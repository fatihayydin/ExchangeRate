using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRate.Data.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExchangeRateDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.CustomerApiLogs.Any())
            {
                return;   // DB has been seeded
            }

            context.SaveChanges();
        }
    }
}
