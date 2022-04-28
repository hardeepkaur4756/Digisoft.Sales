using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Digisoft.Sales.Startup))]
namespace Digisoft.Sales
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
