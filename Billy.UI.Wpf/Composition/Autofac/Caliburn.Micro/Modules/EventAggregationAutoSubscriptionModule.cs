//using Autofac;
//using Autofac.Core;
//using Autofac.Core.Registration;
//using Caliburn.Micro;

//namespace Billy.UI.Wpf.Composition.Autofac.Caliburn.Micro.Modules
//{
//    public class EventAggregationAutoSubscriptionModule : Module
//    {
//        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
//        {
//            // TODO: verify it works cause in old version the override didnt use base.<method>
//            base.AttachToComponentRegistration(componentRegistry, registration);

//            registration.Activated += EventAggregationAutoSubscriptionModule.OnComponentActivated;
//        }

//        private static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
//        {
//            if (!(e?.Instance is IHandle handle))
//                return;

//            e.Context.Resolve<IEventAggregator>().Subscribe(handle);
//        }
//    }
//}