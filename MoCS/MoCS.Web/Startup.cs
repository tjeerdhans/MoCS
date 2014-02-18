using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MoCS.Web.Startup))]
namespace MoCS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
