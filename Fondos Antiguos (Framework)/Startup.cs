using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Fondos_Antiguos.Startup))]
namespace Fondos_Antiguos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
