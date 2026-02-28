using Microsoft.AspNetCore.Antiforgery;

namespace Product.API.AttackPrevention.CSRF
{
    public static class AntiForgery
    {
        public static void AddAppAntiForgeryExtension(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
            });
        }

        public static void UseAntiForgeryTokenMiddlewareExtension(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                var antiforgery = app.ApplicationServices.GetRequiredService<IAntiforgery>();
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append(key: "XSRF-TOKEN", value: tokens.RequestToken!, options: new CookieOptions()
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
                return next(context);
            });
        }
    }
}
