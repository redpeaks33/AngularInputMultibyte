using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace WordCurationSite
{
    public class Global : System.Web.HttpApplication
    {
        public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
        {
            protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                return new MyHttpControllerHandler(requestContext.RouteData);
            }
        }

        public class MyHttpControllerHandler : HttpControllerHandler, IRequiresSessionState
        {
            public MyHttpControllerHandler(RouteData routeData)
                : base(routeData)
            {
            }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            var route = routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{function}/{category}/{id}",
                //routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    function = System.Web.Http.RouteParameter.Optional,
                    category = System.Web.Http.RouteParameter.Optional,
                    id = System.Web.Http.RouteParameter.Optional
                }
            );
            route.RouteHandler = new MyHttpControllerRouteHandler();
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("C:\\Users\\redpeak3\\Desktop\\Projects_WordInfo\\WordChunkSite\\WordChunkSite\\log4net.xml"));
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}