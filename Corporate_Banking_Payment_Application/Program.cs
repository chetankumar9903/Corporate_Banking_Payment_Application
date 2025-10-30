
using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Repository;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //builder.Services.AddAutoMapper(typeof(Program));


            // for timezone
            //TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //TimeZoneInfo.ClearCachedData();
            //TimeZoneInfo.Local.Equals(indianZone);





            // Add services to the container.

            //User
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            //Bank
            builder.Services.AddScoped<IBankRepository, BankRepository>();
            builder.Services.AddScoped<IBankService, BankService>();

            //Customer
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            //Client

            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IClientService, ClientService>();

            //Employee
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            //Beneficiary
            builder.Services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
            builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();

            // Payment
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            //Salary Disbursement
            builder.Services.AddScoped<ISalaryDisbursementRepository, SalaryDisbursementRepository>();
            builder.Services.AddScoped<ISalaryDisbursementService, SalaryDisbursementService>();

            //Batch Transaction
            builder.Services.AddScoped<IBatchTransactionRepository, BatchTransactionRepository>();
            builder.Services.AddScoped<IBatchTransactionService, BatchTransactionService>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            builder.Services.AddControllers();



            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // TODO: add Authentication (JWT), Authorization, AutoMapper, Repositories, Services, etc.

            var app = builder.Build();




            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();





            app.Run();
        }
    }
}
