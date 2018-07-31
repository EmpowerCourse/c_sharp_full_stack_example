using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using System.Threading;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Armoire.Services;
using Armoire.Infrastructure;
using nh = NHibernate;
using Armoire.Entities;
using Armoire.Common;
using Armoire.Automapper;
using Armoire.Persistence;
using System.Net.Mail;
using System.Security.Claims;

namespace Armoire
{
    public class Startup
    {
        private readonly AsyncLocal<Scope> scopeProvider = new AsyncLocal<Scope>();
        private IKernel Kernel { get; set; }

        private object Resolve(Type type) => Kernel.Get(type);
        private object RequestScope(IContext context) => scopeProvider.Value;

        private sealed class Scope : DisposableObject { }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddRequestScopingMiddleware(() => scopeProvider.Value = new Scope());
            services.AddCustomControllerActivation(Resolve);
            services.AddCustomViewComponentActivation(Resolve);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
                options.LoginPath = "/Home/Login";
                options.LogoutPath = "/Home/Logout";
                options.AccessDeniedPath = "/Home/Error";
            });
            services.AddMvc();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Users", policy => policy.RequireClaim(AppConstants.CLAIM_TYPE_USER_ID));
                options.AddPolicy("Administrators", policy => policy.RequireClaim(ClaimTypes.Role, ((int)TypeOfUserRole.Administrator).ToString()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            this.Kernel = this.RegisterApplicationComponents(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private IKernel RegisterApplicationComponents(IApplicationBuilder app)
        {
            var kernel = new StandardKernel();
            // Register application services
            foreach (var ctrlType in app.GetControllerTypes())
            {
                kernel.Bind(ctrlType).ToSelf().InScope(RequestScope);
            }
            kernel.Bind<IAutoMapperMappingCollection>().To<ModelDtoMappings>();
            kernel.Bind<IAutoMapperMappingCollection>().To<ViewModelDtoMappings>();
            kernel.Load<AutoMapperModule>();

            kernel.Bind<IConfiguration>().ToMethod(m => this.Configuration).InSingletonScope();
            kernel.Bind<ISettingsService>().To<SettingsService>().InSingletonScope();
            kernel.Bind<nh.ISession>().ToMethod(m => new NhHelper(
                 new SettingsService(Configuration)
                ).Session).InScope(RequestScope);
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InScope(RequestScope);
            kernel.Bind(typeof(IRepository<>)).To(typeof(NHibernateRepository<>)).InScope(RequestScope);
            kernel.Bind<INotificationService>().To<NotificationService>().InScope(RequestScope);
            kernel.Bind<IUserService>().To<UserService>().InScope(RequestScope);
            kernel.Bind<ICipherService>().To<CipherService>().InScope(RequestScope);
            kernel.Bind<SmtpClient>().ToMethod(ctx => {
                var settings = ctx.Kernel.Get<ISettingsService>();
                SmtpClient client = new SmtpClient(settings.GetStringValue("Email:MailServer"), settings.GetIntValue("Email:MailServerPort"));
                if (settings.GetBoolValue("Email:MailServerRequiresAuthentication"))
                {
                    client.Credentials = new System.Net.NetworkCredential(settings.GetStringValue("Email:MailServerUsername"), settings.GetStringValue("Email:MailServerPassword"));
                }
                client.EnableSsl = settings.GetBoolValue("Email:MailServerRequiresSSL");
                return client;
            }).InThreadScope();
            return kernel;
        }
    }
}
