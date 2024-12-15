using Microsoft.EntityFrameworkCore;
using PlanningPoker.Data;
using PlanningPoker.Extensions;
using PlanningPoker.Hubs;
using PlanningPoker.Services;

namespace PlanningPoker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IavatarService, AvatarService>();
            builder.Services.AddScoped<IroomService, RoomService>();
            builder.Services.AddScoped<IuserService, UserService>();
            builder.Services.AddScoped<InameService, NameService>();

            builder.Services.AddDbContext<PokerContext>();
            builder.Services.AddLog4net();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PokerContext>();
                dbContext.Database.Migrate();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapHub<PokerHub>("/pokerHub");

            app.Run();
        }
    }
}
