using Autofac;
using Autofac.Integration.Mvc;
using PlayPalMini.DAL;
using PlayPalMini.Service;
using PlayPalMini.Service.Common;
using PlayPalMini.Repository;
using PlayPalMini.Repository.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlayPalMini.MVC.App_Start
{
    public class DIContainer
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<EFContext>().AsSelf();

            builder.RegisterType<BoardGameService>().As<IBoardGameService>();
            builder.RegisterType<BoardGameRepository>().As<IBoardGameRepository>();
            builder.RegisterType<ReviewService>().As<IReviewService>();
            builder.RegisterType<ReviewRepository>().As<IReviewRepository>();
            builder.RegisterType<RegisteredUserService>().As<IRegisteredUserService>();
            builder.RegisterType<RegisteredUserRepository>().As<IRegisteredUserRepository>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}