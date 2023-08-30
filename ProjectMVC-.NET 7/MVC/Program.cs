using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Common;
using Service;
using Service.Common;
using Common;

namespace MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // DBcontext 
            builder.Services.AddDbContext<JustStudentsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBconnection")));
            // ----------

            // dependency injection
            builder.Services.AddScoped<IService, StudentService>();
            builder.Services.AddScoped<IRepository, StudentRepository>();
            // ----------

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
            // -----------

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Student}/{action=ListStudents}/{id?}");

            app.Run();
        }
    }
}