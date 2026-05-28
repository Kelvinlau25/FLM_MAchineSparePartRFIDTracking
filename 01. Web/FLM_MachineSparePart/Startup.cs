using Microsoft.AspNet.SignalR;
using Owin;

namespace FILM_Sparepart_MVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable detailed errors in development for easier debugging
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true
            };

            // Map SignalR hubs to ~/signalr
            app.MapSignalR(hubConfiguration);
        }
    }
}
