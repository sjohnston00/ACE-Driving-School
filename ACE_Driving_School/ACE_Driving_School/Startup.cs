using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ACE_Driving_School.Startup))]
namespace ACE_Driving_School
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
