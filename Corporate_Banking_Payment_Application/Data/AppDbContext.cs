using Corporate_Banking_Payment_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ========== DbSets ==========
        public DbSet<User> Users { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<SalaryDisbursement> SalaryDisbursements { get; set; }
        public DbSet<BatchTransaction> BatchTransactions { get; set; }

        // ========== Model Configurations ==========
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== 1️⃣ User ↔ Bank (1:1) ==========
            modelBuilder.Entity<User>()
                .HasOne(u => u.Bank)
                .WithOne(b => b.User)
                .HasForeignKey<Bank>(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 2️⃣ User ↔ Customer (1:1) ==========
            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 3️⃣ Bank ↔ Customer (1:N) ==========
            modelBuilder.Entity<Bank>()
                .HasMany(b => b.Customers)
                .WithOne(c => c.Bank)
                .HasForeignKey(c => c.BankId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 4️⃣ Customer ↔ Client (1:1) ==========
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Client)
                .WithOne(cl => cl.Customer)
                .HasForeignKey<Client>(cl => cl.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 5️⃣ Customer ↔ Document (1:N) ==========
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Customer)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== 6️⃣ Bank ↔ Client (1:N) ==========
            modelBuilder.Entity<Bank>()
                .HasMany(b => b.Clients)
                .WithOne(c => c.Bank)
                .HasForeignKey(c => c.BankId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 7️⃣ Client ↔ Employee (1:N) ==========
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Employees)
                .WithOne(e => e.Client)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== 8️⃣ Client ↔ Beneficiary (1:N) ==========
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Beneficiaries)
                .WithOne(b => b.Client)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== 9️⃣ Client ↔ Payment (1:N) ==========
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Payments)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 🔟 Beneficiary ↔ Payment (1:N) ==========
            modelBuilder.Entity<Beneficiary>()
                .HasMany(b => b.Payments)
                .WithOne(p => p.Beneficiary)
                .HasForeignKey(p => p.BeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 11️⃣ Client ↔ SalaryDisbursement (1:N) ==========
            modelBuilder.Entity<Client>()
                .HasMany(c => c.SalaryDisbursements)
                .WithOne(s => s.Client)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 12️⃣ Employee ↔ SalaryDisbursement (1:N) ==========
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.SalaryDisbursements)
                .WithOne(s => s.Employee)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 13️⃣ Client ↔ BatchTransaction (1:N) ==========
            modelBuilder.Entity<Client>()
                .HasMany(c => c.BatchTransactions)
                .WithOne(b => b.Client)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 14️⃣ BatchTransaction ↔ SalaryDisbursement (1:N) ==========
            modelBuilder.Entity<BatchTransaction>()
                .HasMany(bt => bt.SalaryDisbursements)
                .WithOne(sd => sd.BatchTransaction)
                .HasForeignKey(sd => sd.BatchId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== 15️⃣ User ↔ Reports (1:N) ==========
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reports)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.GeneratedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== ENUM Conversions ==========
            modelBuilder.Entity<User>()
                .Property(u => u.UserRole)
                .HasConversion<string>();

            modelBuilder.Entity<Customer>()
                .Property(c => c.VerificationStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Report>()
                .Property(r => r.ReportType)
                .HasConversion<string>();
        }
    }
}