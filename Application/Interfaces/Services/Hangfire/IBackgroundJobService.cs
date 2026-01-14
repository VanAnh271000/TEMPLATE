using System.Linq.Expressions;

namespace Application.Interfaces.Services.Hangfire
{
    public interface IBackgroundJobService
    {
        //Ghi mô tả công việc vào database để thực thi sau
        void Enqueue<TJob>(Expression<Action<TJob>> job);
        void Schedule<TJob>(Expression<Action<TJob>> job, TimeSpan delay);
        void Recurring<TJob>(string jobId, Expression<Action<TJob>> job, string cron);
    }
}
