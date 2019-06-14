using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KH.Startup))]
namespace KH
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
