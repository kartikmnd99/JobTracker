using JobTracker.Application.Interfaces;
using JobTracker.Infrastructure.Background;
using JobTracker.Infrastructure.Data;
using JobTracker.Infrastructure.Email;
using JobTracker.Infrastructure.Repositories;
using JobTracker.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.Configure<SmtpSettings>(config.GetSection("Smtp"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
        services.AddScoped<IInterviewRepository, InterviewRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // Real email only if SMTP is configured, otherwise just log the email
        if (!string.IsNullOrWhiteSpace(config["Smtp:Host"]))
            services.AddSingleton<IEmailSender, SmtpEmailSender>();
        else
            services.AddSingleton<IEmailSender, LogEmailSender>();

        services.AddHostedService<ReminderBackgroundService>();

        return services;
    }
}