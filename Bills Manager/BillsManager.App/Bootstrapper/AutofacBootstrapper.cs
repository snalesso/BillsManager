using Autofac;
using BillsManager.Models;
using BillsManager.Services.Feedback;
//using BillsManager.App.Modules;
using BillsManager.Services.Providers;
using BillsManager.Services.Reporting;
using BillsManager.ViewModels;
using BillsManager.ViewModels.Reporting;
using BillsManager.Views;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BillsManager.App.Bootstrappers
{
    // TODO: https://code.google.com/p/autofac/wiki/RelationshipTypes
    public class AutofacBootstrapper : BootstrapperEx<ShellViewModel>
    {
        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            // EXTRA MODULES
            // builder.RegisterModule<EventAggregationAutoUnsubscriptionModule>();

            // SERVICES
            // URGENT: settings for db path
            builder.RegisterInstance(new XMLDBConnector(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB\db.bmdb"))).AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<XMLBackupsProvider>().As<IBackupsProvider>().SingleInstance();

            builder.RegisterGeneric(typeof(ReportPrinter<>)).As(typeof(ReportPrinter<>)).InstancePerDependency();

            builder.RegisterType<EMailFeedbackSender>().As<IFeedbackSender>().SingleInstance();


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

            builder.Register<Func<IEnumerable<BillReportViewModel>, string, string, ReportCenterViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (bills, header, comment) =>
                        {
                            return
                                new ReportCenterViewModel(
                                    ctx.Resolve<Func<IEnumerable<BillReportViewModel>, ReportPrinter<BillReportViewModel>>>(),
                                    bills,
                                    header,
                                    comment);
                        };
                });


            builder.Register<Func<Bill, BillDetailsViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (b) =>
                        {
                            return new BillDetailsViewModel(
                                ctx.Resolve<IWindowManager>(),
                                ctx.Resolve<IEventAggregator>(),
                                b);
                        };
                });

            builder.Register<Func<IEnumerable<Supplier>, Bill, BillAddEditViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (avsupps, b) =>
                        {
                            return new BillAddEditViewModel(
                                ctx.Resolve<IWindowManager>(),
                                ctx.Resolve<IEventAggregator>(),
                                avsupps,
                                b);
                        };
                });

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