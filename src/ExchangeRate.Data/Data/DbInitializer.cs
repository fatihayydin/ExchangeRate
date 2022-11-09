namespace ExchangeRate.Data.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExchangeRateDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (!context.CustomerApiKeys.Any())
            {
                context.CustomerApiKeys.Add(new CustomerApiKey
                {
                    ApiKey = "gH121U2gKZABgj4Sltcda78YCYktZiz7",
                    CreatedDate = DateTime.Now,
                    Email = "fatihayydin@gmail.com",
                    IsActive = true
                });

                context.SaveChanges();

                return;   // DB has been seeded
            }

            context.SaveChanges();
        }
    }
}
