﻿using CommunityNetwork.Common.Inerfaces;
using CommunityNetWork.Dal;
using CommunityNetWork.Dal.Interfaces;
using Identity.BL;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace IdentityService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Register<IDynamoDBFactory>(() => new AwsDynamoDBFactory());
            container.Register<IIdentityService>(() => new IdentityManager(container.GetInstance<IDynamoDBFactory>()));
            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}
