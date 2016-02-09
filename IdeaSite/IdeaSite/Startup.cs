using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IdeaSite.Startup))]
namespace IdeaSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
