using Autofac;
using BillsManager.Localization;
using BillsManager.Services.Feedback;
//using BillsManager.App.Modules;
using BillsManager.Services.Providers;
using BillsManager.Services.Reporting;
using BillsManager.Services.Settings;
using BillsManager.ViewModels;
using BillsManager.ViewModels.Reporting;
using BillsManager.Views;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace BillsManager.App.Bootstrappers
{
    // TIP: https://code.google.com/p/autofac/wiki/RelationshipTypes
    public class AutofacBootstrapper : BootstrapperEx<ShellViewModel>
    {
        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            
            // EXTRA MODULES
            // builder.RegisterModule<EventAggregationAutoUnsubscriptionModule>();

            // SERVICES
            builder.RegisterType<XMLSettingsProvider>().As<ISettingsProvider>().SingleInstance();

            builder.RegisterInstance(new XMLDBConnector(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB\db.bmdb"))).AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<XMLBackupsProvider>().As<IBackupsProvider>().SingleInstance();

            builder.RegisterGeneric(typeof(ReportPrinter<>)).As(typeof(ReportPrinter<>)).InstancePerDependency();

            builder.Register<EMailFeedbackSender>(
                ctx => new EMailFeedbackSender(ctx.Resolve<ISettingsProvider>().Settings.FeedbackToEmailAddress))
                .As<IFeedbackSender>().SingleInstance();

            builder.RegisterInstance(
                new ResxTranslationProvider(
                    typeof(BillsManager.Views.Languages.Resources).FullName,
                    Assembly.GetAssembly(typeof(BillsManager.Views.ShellView)))).As<ITranslationProvider>().SingleInstance();

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


            //builder.Register<Func<Bill, BillDetailsViewModel>>(
            //    c =>
            //    {
            //        var ctx = c.Resolve<IComponentContext>();

            //        return
            //            (b) =>
            //            {
            //                return new BillDetailsViewModel(
            //                    ctx.Resolve<IWindowManager>(),
            //                    ctx.Resolve<IEventAggregator>(),
            //                    b);
            //            };
            //    });

            //builder.Register<Func<IEnumerable<Supplier>, Bill, BillAddEditViewModel>>( // TODO: check whether this registration is needed
            //    c =>
            //    {
            //        var ctx = c.Resolve<IComponentContext>();

            //        return
            //            (avsupps, b) =>
            //            {
            //                return new BillAddEditViewModel(
            //                    ctx.Resolve<IWindowManager>(),
            //                    ctx.Resolve<IEventAggregator>(),
            //                    avsupps,
            //                    b);
            //            };
            //    });

            builder.Register<Func<DBBackupsViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        () =>
                        {
                            return new DBBackupsViewModel(
                                ctx.Resolve<IWindowManager>(),
                                ctx.Resolve<IEventAggregator>(),
                                ctx.Resolve<Func<string, IBackupsProvider>>().Invoke(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Backups\")));
                        };
                });
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
               {
                   GetType().Assembly, 
                   typeof(ShellViewModel).Assembly, 
                   typeof(ShellView).Assembly
               };
        }
    }
}