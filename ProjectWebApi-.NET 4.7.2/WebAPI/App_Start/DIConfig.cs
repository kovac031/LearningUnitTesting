using Autofac.Integration.WebApi;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using DataAccessLayer;
using Service;
using Service.Common;
using Repository;
using Repository.Common;
using AutoMapper;
using Common;

namespace Project.WebAPI.App_Start
{
    public class DIConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<EFContext>().AsSelf();
            builder.RegisterType<StudentService>().As<IService>();
            builder.RegisterType<StudentRepository>().As<IRepository>();

            //----------  AutoMapper Configurations -------------
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            builder.RegisterInstance(mapper).As<IMapper>();
            //---------------------------------------------------

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}