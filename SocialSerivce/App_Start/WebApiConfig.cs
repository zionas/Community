using System;
using System.Collections.Generic;

using System.Linq;
using System.Web.Http;
using SimpleInjector.Advanced;
using SimpleInjector.Diagnostics.Debugger;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using CommunityNetWork.Dal.Interfaces;
using CommunityNetWork.Dal;
using Social.BL.Interfaces;
using Social.BL.Models;
using System.Web.Http.Cors;

namespace SocialSerivce
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.EnableCors(new EnableCorsAttribute("http://localhost:4200", headers: "*", methods: "*"));
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            InitContainer();
        }
        private static void InitContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.Register<IGraphFactory>(() => new Neo4JConnectorFactory());
            container.Register<IDynamoDBFactory>(() => new AwsDynamoDBFactory());
            container.Register<ICommunication>(() => new Communication(container.GetInstance< IGraphFactory>()));
            container.Register<IRepository>(() => new Repository(container.GetInstance<IGraphFactory>()));

            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
    }
}
