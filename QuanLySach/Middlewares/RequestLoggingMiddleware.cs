using Microsoft.AspNetCore.Http;

namespace QuanLySach.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Lấy thời gian hiện tại
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            // Lấy phương thức HTTP
            var method = context.Request.Method;

            // Lấy đường dẫn
            var path = context.Request.Path;

            // Log trước khi vào Controller
            Console.WriteLine(
                $"[{time}] Method: {method} - Path: {path}"
            );

            // Kiểm tra URL Details
            if (path.StartsWithSegments("/Books/Details"))
            {
                var segments = path.Value?.Split('/');

                if (segments != null && segments.Length >= 4)
                {
                    if (int.TryParse(segments[3], out int id))
                    {
                        // ID <= 0 thì chặn
                        if (id <= 0)
                        {
                            context.Response.StatusCode = 400;

                            await context.Response.WriteAsync(
                                "Book id khong hop le"
                            );

                            Console.WriteLine(
                                $"Status Code: {context.Response.StatusCode}"
                            );

                            return;
                        }
                    }
                }
            }

            // Cho request đi tiếp
            await _next(context);

            // Log Status Code sau khi Controller xử lý
            Console.WriteLine(
                $"Status Code: {context.Response.StatusCode}"
            );
        }
    }
}