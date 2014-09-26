using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

/*
 * See WebApiConfig
 * */

namespace ToDoApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "ToDos", action = "Get", id = UrlParameter.Optional }
            //);

            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate:"api/Todos/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //    );

//            routes.Routes.MapHttpRoute(
//    name: "DefaultApi",
//    routeTemplate: "api/{controller}/{id}",
//    defaults: new { id = RouteParameter.Optional }
//);

        }
    }
}