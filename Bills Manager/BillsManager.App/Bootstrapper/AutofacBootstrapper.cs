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
            builder.RegisterType<XMLDBsProvider>().As<IDBsProvider>().SingleInstance();

            builder.RegisterType<XMLDBConnector>().As<IDBConnector>().InstancePerDependency();

            builder.RegisterType<XMLBackupsProvider>().As<IBackupsProvider>().InstancePerDependency();

            builder.RegisterGeneric(typeof(ReportPrinter<>)).As(typeof(ReportPrinter<>)); // TODO: review InstancePerDependency

            builder.RegisterType<EMailFeedbackSender>().As<IFeedbackSender>().InstancePerDependency();


            // VIEWMODELS
            builder.RegisterType<ShellViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<BackupCenterViewModel>().AsSelf().InstancePerDependency(); /* TODO: make singleton (when reshown view is empty),
                                                                                             * then look for other single instance vms */

            builder.RegisterType<DBsViewModel>().AsSelf().SingleInstance();
            builder.Register<DBsViewModel>(
                c =>
                {
                    return new DBsViewModel(
                        c.Resolve<IWindowManager>(),
                        c.Resolve<IEventAggregator>(),
                        c.Resolve<Func<string, IDBsProvider>>().Invoke(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Databases")), // TODO: config file
                        c.Resolve<Func<string, DBViewModel>>(),
                        c.Resolve<Func<IEnumerable<string>, string, DBAddEditViewModel>>());
                });


            builder.Register<Func<IEventAggregator, Bill, BillDetailsViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (ea, b) =>
                        {
                            return new BillDetailsViewModel(
                                ctx.Resolve<IWindowManager>(),
                                //ctx.Resolve<IEventAggregator>(),
                                ea,
                                b);
                        };
                });


            builder.RegisterType<BillAddEditViewModel>().AsSelf().InstancePerDependency();
            builder.Register<Func<IEventAggregator, Bill, BillAddEditViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (ea, b) =>
                        {
                            return new BillAddEditViewModel(
                                ctx.Resolve<IWindowManager>(),
                                //ctx.Resolve<IEventAggregator>(),
                                ea,
                                b);
                        };
                });

            builder.Register<Func<IEventAggregator, ISuppliersProvider, SuppliersViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (ea, sp) =>
                        {
                            return new SuppliersViewModel(
                                ctx.Resolve<IWindowManager>(),
                                //ctx.Resolve<IEventAggregator>(),
                                ea,
                                sp,
                                ctx.Resolve<Func<IEventAggregator, Supplier, SupplierAddEditViewModel>>(),
                                ctx.Resolve<Func<IEventAggregator, Supplier, SupplierDetailsViewModel>>());
                        };
                });

            builder.Register<Func<IEventAggregator, IBillsProvider, BillsViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (ea, bp) =>
                        {
                            return new BillsViewModel(
                                ctx.Resolve<IWindowManager>(),
                                //ctx.Resolve<IEventAggregator>(),
                                ea,
                                bp,
                                ctx.Resolve<Func<IEventAggregator, Bill, BillAddEditViewModel>>(),
                                ctx.Resolve<Func<IEventAggregator, Bill, BillDetailsViewModel>>());
                        };
                });

            builder.RegisterType<DBViewModel>().AsSelf().InstancePerDependency();
            builder.Register<Func<string, DBViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                    s =>
                    {
                        var iDBConnFac = ctx.Resolve<Func<string, IDBConnector>>();
                        var dbVMFac = ctx.Resolve<Func<IDBConnector, DBViewModel>>();

                        return dbVMFac(iDBConnFac(s));
                    };
                });

            builder.Register<Func<string, DBBackupsViewModel>>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();

                    return
                        (s) =>
                        {
                            return new DBBackupsViewModel(
                                ctx.Resolve<IWindowManager>(),
                                ctx.Resolve<IEventAggregator>(),
                                ctx.Resolve<Func<string, IBackupsProvider>>().Invoke(s));
                        };
                });

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

//using Autofac;
////using BillsManager.App.Modules;
//using BillsManager.Services.Providers;
//using BillsManager.ViewModels;
//using BillsManager.Views;
//using Caliburn.Micro;
//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace BillsManager.App.Bootstrappers
//{
//    public class AutofacBootstrapper : BootstrapperEx<ShellViewModel>
//    {
//        protected override void ConfigureContainer(Autofac.ContainerBuilder builder)
//        {
//            base.ConfigureContainer(builder);

//            // MODULES
//            // builder.RegisterModule<EventAggregationAutoUnsubscriptionModule>();

//            // SERVICES
//            builder.RegisterType<XMLDBsProvider>().As<IDBsProvider>().SingleInstance();

//            builder.RegisterType<XMLDBConnector>().As<IDBConnector>().InstancePerDependency();

//            builder.RegisterType<XMLBackupsProvider>().As<IBackupsProvider>().SingleInstance();


//            // VIEWMODELS
//            builder.RegisterType<ShellViewModel>().AsSelf().SingleInstance();

//            builder.RegisterType<DBsViewModel>().AsSelf().SingleInstance();
//            builder.Register<DBsViewModel>(
//                c =>
//                {
//                    return new DBsViewModel(
//                        c.Resolve<IWindowManager>(),
//                        c.Resolve<IEventAggregator>(),
//                        c.Resolve<Func<string, IDBsProvider>>().Invoke(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Databases")),
//                        c.Resolve<Func<string, DBViewModel>>());
//                });

//            builder.RegisterType<DBViewModel>().AsSelf().InstancePerDependency();
//            builder.Register<Func<string, DBViewModel>>(
//                c =>
//                {
//                    var iDBConnFac = c.Resolve<Func<string, IDBConnector>>();
//                    var dbVMFac = c.Resolve<Func<IDBConnector, DBViewModel>>();

//                    return s => dbVMFac(iDBConnFac(s));
//                });

//            builder.RegisterType<BackupsViewModel>().AsSelf().SingleInstance();
//        }

//        protected override IEnumerable<Assembly> SelectAssemblies()
//        {
//            return new[]
//               {
//                   GetType().Assembly, 
//                   typeof(ShellViewModel).Assembly, 
//                   typeof(ShellView).Assembly
//               };
//        }
//    }
//}