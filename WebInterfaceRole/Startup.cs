using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebInterfaceRole.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace WebInterfaceRole
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
