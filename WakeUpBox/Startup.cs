using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WakeUpBox.Startup))]
namespace WakeUpBox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
