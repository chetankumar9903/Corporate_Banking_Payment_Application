using Corporate_Banking_Payment_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Apply migrations automatically if pending
            await context.Database.MigrateAsync();

            // ✅ Seed SuperAdmin User
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    UserRole = UserRole.SUPERADMIN,
                    UserName = "superadmin",
                    Password = "Admin@123", // Hash later
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailId = "admin@corpbank.com",
                    PhoneNumber = "9999999999",
                    Address = "HQ - Corporate Center",
                    IsActive = true
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }

            // ✅ Seed Default Bank if none exists
            if (!context.Banks.Any())
            {
                var adminUser = context.Users.FirstOrDefault(u => u.UserRole == UserRole.SUPERADMIN);

                var defaultBank = new Bank
                {
                    BankName = "Corporate Central Bank",
                    Branch = "Headquarters",
                    IFSCCode = "CORP0001",
                    ContactNumber = "1800123456",
                    EmailId = "info@corpbank.com",
                    IsActive = true,
                    UserId = adminUser.UserId
                };

                context.Banks.Add(defaultBank);
                await context.SaveChangesAsync();
            }

            // ✅ Optionally seed sample Customer and Client
            if (!context.Customers.Any())
            {
                var adminBank = context.Banks.First();

                var customerUser = new User
                {
                    UserRole = UserRole.CLIENTUSER,
                    UserName = "client1",
                    Password = "Client@123",
                    FirstName = "Chetan",
                    LastName = "Kumar",
                    EmailId = "client1@corpbank.com",
                    PhoneNumber = "9876543210",
                    Address = "Mumbai",
                    IsActive = true
                };
                context.Users.Add(customerUser);
                await context.SaveChangesAsync();

                var newCustomer = new Customer
                {
                    UserId = customerUser.UserId,
                    BankId = adminBank.BankId,
                    VerificationStatus = Status.APPROVED,
                    IsActive = true,
                    OnboardingDate = DateTime.UtcNow
                };

                context.Customers.Add(newCustomer);
                await context.SaveChangesAsync();

                var newClient = new Client
                {
                    CustomerId = newCustomer.CustomerId,
                    BankId = adminBank.BankId,
                    CompanyName = "TechNova Solutions",
                    Balance = 50000,
                    IsActive = true
                };

                context.Clients.Add(newClient);
                await context.SaveChangesAsync();
            }
        }
    }
}