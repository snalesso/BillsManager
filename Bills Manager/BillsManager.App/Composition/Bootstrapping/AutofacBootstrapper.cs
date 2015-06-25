using Autofac;
using BillsManager.Localization;
//using BillsManager.App.Modules;
using BillsManager.Services.Data;
using BillsManager.Services.Feedback;
using BillsManager.Services.Reporting;
using BillsManager.Services;
using BillsManager.ViewModels;
using BillsManager.ViewModels.Reporting;
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
            builder.RegisterType<XMLSettingsProvider>().As<ISettingsProvider>().SingleInstance();

#if !DEBUG
            builder.RegisterInstance(new XMLDBConnector(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB\db.bmdb"))).AsImplementedInterfaces().SingleInstance();
#else
            builder.RegisterInstance(new XMLDBConnector(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB\db.bmdb"))).AsImplementedInterfaces().SingleInstance();
            //builder.RegisterInstance(new MockedDBConnector(12, 7)).AsImplementedInterfaces().SingleInstance();
#endif
            builder.RegisterType<XMLBackupsProvider>().As<IBackupsProvider>().SingleInstance();

            builder.RegisterGeneric(typeof(ReportPrinter<>)).As(typeof(ReportPrinter<>)).InstancePerDependency();

            builder.Register<EMailFeedbackSender>(
                ctx => new EMailFeedbackSender(ctx.Resolve<ISettingsProvider>().Settings.FeedbackToEmailAddress))
                //ctx => new EMailFeedbackSender("nalesso.sergio@gmail.com"))
                .As<IFeedbackSender>().SingleInstance();

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
                    tm.Instance.CurrentLanguage = tm.Context.Resolve<ISettingsProvider>().Settings.Language;
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

            builder.RegisterType<BackupCenterViewModel>().AsSelf().SingleInstance();

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

            builder.Register<Func<DBBackupsViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        () => new DBBackupsViewModel(
                            ctx.Resolve<IWindowManager>(),
                            ctx.Resolve<IEventAggregator>(),
                            ctx.Resolve<Func<string, IBackupsProvider>>().Invoke(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Backups\")));
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