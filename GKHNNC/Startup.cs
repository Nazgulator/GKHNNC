using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GKHNNC.Startup))]

namespace GKHNNC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
            
        }
    }
}
