using Application.Interfaces.Services.Hangfire;
using Hangfire;
using System.Linq.Expressions;

namespace Infrastructure.BackgroundJobs.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        public void Enqueue<TJob>(Expression<Action<TJob>> job)
        {
            BackgroundJob.Enqueue(job);
        }

        public void Schedule<TJob>(Expression<Action<TJob>> job, TimeSpan delay)
        {
            BackgroundJob.Schedule(job, delay);
        }

        public void Recurring<TJob>(string jobId, Expression<Action<TJob>> job, string cron)
        {
            RecurringJob.AddOrUpdate(jobId, job, cron);
        }
    }
}
