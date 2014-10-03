using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RequirementGathering.Startup))]
namespace RequirementGathering
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
