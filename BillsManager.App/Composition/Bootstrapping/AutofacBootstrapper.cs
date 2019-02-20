using Autofac;
using BillsManager.Localization;
using BillsManager.Services;
//using BillsManager.App.Modules;
using BillsManager.Services.DB;
using BillsManager.Services.Feedback;
using BillsManager.Services.Reporting;
using BillsManager.ViewModels;
//using BillsManager.ViewModels.Search;
using BillsManager.Views;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BillsManager.App.Composition.Bootstrapping
{
    // TIP: https://code.google.com/p/autofac/wiki/RelationshipTypes
    public class AutofacBootstrapper : BootstrapperEx<ShellViewModel>
    {
        public AutofacBootstrapper()
        {
            this.Initialize();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            // EXTRA MODULES
            // builder.RegisterModule<EventAggregationAutoUnsubscriptionModule>();

            // SERVICES
            builder.RegisterType<XMLSettingsService>().As<ISettingsService>().SingleInstance();

#if !DEBUG
            // DON'T TOUCH
            builder.RegisterInstance(new XMLDBConnector(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB\db.bmdb"))).AsImplementedInterfaces().SingleInstance();
#else
            builder.RegisterInstance(new XMLDBService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB\db.bmdb"))).AsImplementedInterfaces().SingleInstance();
            builder.RegisterInstance(new MockedDBConnector(0, 7)).AsImplementedInterfaces().SingleInstance();
#endif

            builder.RegisterGeneric(typeof(ReportPrinter<>)).As(typeof(ReportPrinter<>)).InstancePerDependency();

            builder.Register<EMailFeedbackService>(
                ctx => new EMailFeedbackService(ctx.Resolve<ISettingsService>().Settings.FeedbackToEmailAddress))
                //ctx => new EMailFeedbackSender("nalesso.sergio@gmail.com"))
                .As<IFeedbackService>().SingleInstance();

            builder.RegisterInstance(
                new ResxTranslationProvider(
                    typeof(BillsManager.Views.Languages.Resources).FullName,
                    Assembly.GetAssembly(typeof(BillsManager.Views.ShellView))))
                    .As<ITranslationProvider>().SingleInstance();

            builder.RegisterInstance(TranslationManager.Instance)
                .AsSelf()
                //.PropertiesAutowired()
                .AutoActivate()
                .OnActivated(
                (tm) =>
                {
                    tm.Instance.CurrentLanguage = tm.Context.Resolve<ISettingsService>().Settings.Language;
                    tm.Instance.TranslationProvider = tm.Context.Resolve<ITranslationProvider>();
                });

            // VIEWMODELS
            builder.RegisterType<ShellViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<DBViewModel>().AsSelf().SingleInstance();

            //builder.Register<Func<SearchViewModel<BillDetailsViewModel>>>(
            //    c =>
            //    {
            //        var ctx = c.Resolve<IComponentContext>();

            //        return
            //            (filter) => new SearchViewModel<BillDetailsViewModel>(ctx.Resolve<IEventAggregator>(), filter);
            //    });

            builder.RegisterType<StatusBarViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<SendFeedbackViewModel>().AsSelf().InstancePerDependency();

            builder.RegisterType<SuppliersViewModel>().InstancePerDependency();

            builder.RegisterType<BillsViewModel>().InstancePerDependency();

            builder.RegisterType<SearchSuppliersViewModel>().InstancePerDependency();

            builder.RegisterType<SearchBillsViewModel>().InstancePerDependency();

            builder.Register<Func<IEnumerable<BillReportViewModel>, string, string, PrintReportViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (bills, header, comment) =>
                        {
                            return
                                new PrintReportViewModel(
                                    ctx.Resolve<Func<IEnumerable<BillReportViewModel>, ReportPrinter<BillReportViewModel>>>(),
                                    bills,
                                    header,
                                    comment);
                        };
                });
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
               {
                   this.GetType().Assembly,
                   typeof(ShellViewModel).Assembly, 
                   typeof(ShellView).Assembly
               };
        }
    }
}