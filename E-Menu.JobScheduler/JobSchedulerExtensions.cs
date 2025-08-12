using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace E_Menu.JobScheduler;

public static class JobSchedulerExtensions
{
    public static IServiceCollection AddJobScheduler(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            var jobAssembly = typeof(JobSchedulerExtensions).Assembly;
            var jobTypes = jobAssembly.GetTypes()
                .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var jobType in jobTypes)
            {
                var jobKey = new JobKey(jobType.Name);
                q.AddJob(jobType, jobKey);
                var cron = configuration[$"Quartz:CronSchedules:{jobType.Name}"] ?? "0 0/10 * * * ?";
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{jobType.Name}Trigger", jobType.Namespace)
                    .WithCronSchedule(cron));
            }
        });

        services.AddScoped<IScheduler>(provider =>
        {
            var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
            return schedulerFactory.GetScheduler().Result;
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
}
