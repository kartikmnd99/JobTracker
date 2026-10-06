using JobTracker.Application.Interfaces;
using JobTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJobApplicationService, JobApplicationService>();
        services.AddScoped<IInterviewService, InterviewService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}