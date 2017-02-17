using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(lnoAzureWebApp.Startup))]
namespace lnoAzureWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
