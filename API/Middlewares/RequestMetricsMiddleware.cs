//using Application.DTOs.Commons;

//namespace API.Middlewares
//{
//    public class RequestMetricsMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public RequestMetricsMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            AppMetrics.Requests.Add(1);
//            NotificationMetrics.SendTotal.Add(1);

//            await _next(context);
//        }
//    }
//}
