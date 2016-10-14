using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Artisan.Startup))]
namespace Artisan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }        
    }
}
