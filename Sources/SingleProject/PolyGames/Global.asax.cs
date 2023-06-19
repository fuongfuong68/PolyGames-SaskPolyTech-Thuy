using Ninject;
using Ninject.Web.Common.WebHost;
using PolyGames.Common;
using PolyGames.Services;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PolyGames
{
    public class MvcApplication : NinjectHttpApplication
    {
        protected override IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel();
            RegisterSevices(kernel);

            return kernel;
        }

        private void RegisterSevices(IKernel kernel)
        {
            kernel.Bind<UploadConfiguration>().To<UploadConfiguration>().InSingletonScope()
                .WithPropertyValue("MaxAllowedImageContentLength", int.Parse(ConfigurationManager.AppSettings["MaxAllowedImageContentLength"]))
                .WithPropertyValue("MaxAllowedVideoContentLength", int.Parse(ConfigurationManager.AppSettings["MaxAllowedVideoContentLength"]))
                .WithPropertyValue("MaxAllowedExecutableFileContentLength", int.Parse(ConfigurationManager.AppSettings["MaxAllowedExecutableFileContentLength"]));

            kernel.Bind<SmtpConfiguration>().To<SmtpConfiguration>().InSingletonScope()
                .WithPropertyValue("Server", ConfigurationManager.AppSettings["SMTP_Server"])
                .WithPropertyValue("Port", int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]))
                .WithPropertyValue("Username", ConfigurationManager.AppSettings["SMTP_Username"])
                .WithPropertyValue("Password", ConfigurationManager.AppSettings["SMTP_Password"])
                .WithPropertyValue("Enable", bool.Parse(ConfigurationManager.AppSettings["SMTP_Enable"]));

            kernel.Bind<SqlConfiguration>().To<SqlConfiguration>().InSingletonScope()
                .WithPropertyValue("ConnectionString", ConfigurationManager.ConnectionStrings["PolyGameConnection"].ConnectionString);

            kernel.Bind<ContentTypeManager>().To<ContentTypeManager>().InTransientScope();
            kernel.Bind<FileNameGenerator>().To<FileNameGenerator>().InTransientScope();
            kernel.Bind<PolyGameService>().To<PolyGameService>().InTransientScope();
            kernel.Bind<SmtpService>().To<SmtpService>().InTransientScope();
        }

        protected override void OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}