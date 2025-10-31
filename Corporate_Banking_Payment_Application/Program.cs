
using CloudinaryDotNet;
using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Repository;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();

            //authentication
            builder.Services.AddScoped<IAuthService, AuthService>();

            // automapper
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

            // 1. Get configuration settings for Cloudinary (assuming they are in appsettings.json or secrets)
            var cloudinaryAccount = new Account(
                 builder.Configuration["CloudinarySettings:CloudName"],
                 builder.Configuration["CloudinarySettings:ApiKey"],
                 builder.Configuration["CloudinarySettings:ApiSecret"]
            );

            // 2. Register the Cloudinary client as a Singleton
            // Singleton is appropriate because the client object is thread-safe and expensive to create.
            var cloudinary = new Cloudinary(cloudinaryAccount) { Api = { Secure = true } };
            builder.Services.AddSingleton(cloudinary);


            //2. jwt
            var jwtKey = builder.Configuration["Jwt:Key"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
                };
            });

            builder.Services.AddAuthorization();


            // 3. add cors  request origin

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("MyPolicy", o =>
                {
                    o.AllowAnyHeader();
                    o.AllowAnyMethod();
                    o.AllowAnyOrigin();

                });
            });


            //4. Add Logging to Console
            //builder.Services.AddLogging(builder =>
            //{
            //    builder.AddConsole();
            //    builder.SetMinimumLevel(LogLevel.Error);
            //});






            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Corporate Banking Payment Application"
                });
                //Define Security Scheme for JWT BEarer Tokens
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT Bearer Token only",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

                //Add Security Requirement
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[]{ } }
    });
            });


            var app = builder.Build();




            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            if (app.Environment.IsDevelopment())

            {

                app.UseSwagger();

                //Configure the Swagger UI Options ==============================

                app.UseSwaggerUI(options =>

                {

                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Corporate Banking Payment Application");

                    options.EnablePersistAuthorization();

                });
            }

            app.UseCors("MyPolicy");
            app.UseHttpsRedirection();

            //app.UseAuthorization();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();





            app.Run();
        }
    }
}
