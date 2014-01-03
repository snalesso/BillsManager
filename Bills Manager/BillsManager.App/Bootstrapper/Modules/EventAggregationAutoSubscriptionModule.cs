using Autofac;
using Autofac.Core;
using Caliburn.Micro;

namespace BillsManager.App.Modules
{
    public class EventAggregationAutoSubscriptionModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Activated += OnComponentActivated;
        }

        private static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            if (e == null)
                return;
            var handle = e.Instance as IHandle;
            if (handle == null)
                return;
            e.Context.Resolve<IEventAggregator>().Subscribe(handle);
        }
    }
}