
using Microsoft.EntityFrameworkCore;
using project.Repositories;

namespace project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddControllers();

            // Add this near the top with other service registrations
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            // Swagger classique pour Web API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //dbcontext
            var cnx = builder.Configuration.GetConnectionString("cnx");
            builder.Services.AddDbContext<project.Data.PharmacieDbContext>(
                options => options.UseSqlServer(cnx));
            //injection  de rep 
            builder.Services.AddScoped<IMedicamentRepository, MedicamentRepository>();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IntIPharmacienRepository, PharmacienRepository>();

            var app = builder.Build();
             
            
            // PAS D’ORDONNANCE ici si pas de repository

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
