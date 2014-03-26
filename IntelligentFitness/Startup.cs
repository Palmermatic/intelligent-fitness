using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IntelligentFitness.Startup))]
namespace IntelligentFitness
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
