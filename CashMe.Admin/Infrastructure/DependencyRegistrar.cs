using Autofac;
using Autofac.Integration.Mvc;
using CashMe.Service;
using CashMe.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CashMe.Service.Role;

namespace CashMe.Infrastructure
{
    public static class DependencyRegistrar
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            // MVC - OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // MVC - OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // MVC - OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // MVC - OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            //installation localization service
            builder.RegisterType<AccountServices>().As<IAccountServices>().InstancePerLifetimeScope();
            builder.RegisterType<RoleServices>().As<IRoleServices>().InstancePerLifetimeScope();

            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<ClaimsServices>().As<IClaimsServices>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigServices>().As<IConfigServices>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentServices>().As<IPaymentServices>().InstancePerLifetimeScope();
            builder.RegisterType<UserRefServices>().As<IUserRefServices>().InstancePerLifetimeScope();
            builder.RegisterType<WalletServices>().As<IWalletServices>().InstancePerLifetimeScope();
            builder.RegisterType<UserInfoServices>().As<IUserInfoServices>().InstancePerLifetimeScope();
            builder.RegisterType<GameService>().As<IGameService>().InstancePerLifetimeScope();
            builder.RegisterType<TargetServices>().As<ITargetServices>().InstancePerLifetimeScope();
            builder.RegisterType<TargetMasterServices>().As<ITargetMasterServices>().InstancePerLifetimeScope();
            builder.RegisterType<AnimalMasterServices>().As<IAnimalMasterServices>().InstancePerLifetimeScope();
            builder.RegisterType<ResultRaceServices>().As<IResultRaceServices>().InstancePerLifetimeScope();
            builder.RegisterType<ReportTOPServices>().As<IReportTOPServices>().InstancePerLifetimeScope();
            builder.RegisterType<BetServices>().As<IBetServices>().InstancePerLifetimeScope();
            builder.RegisterType<ChatMessageDetailServices>().As<IChatMessageDetailServices>().InstancePerLifetimeScope();
            builder.RegisterType<ChatPrivateMessageDetailServices>().As<IChatPrivateMessageDetailServices>().InstancePerLifetimeScope();
            builder.RegisterType<ChatUserDetailServices>().As<IChatUserDetailServices>().InstancePerLifetimeScope();
            builder.RegisterType<IPlockServices>().As<IIPlockServices>().InstancePerLifetimeScope();
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }

}
