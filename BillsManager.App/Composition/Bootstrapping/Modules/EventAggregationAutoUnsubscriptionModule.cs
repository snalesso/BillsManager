using Autofac;
using Autofac.Core;
using Caliburn.Micro;

namespace BillsManager.App.Composition.Bootstrapping.Modules
{
    public class EventAggregationAutoUnsubscriptionModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Activated += OnComponentActivated;
        }

        private static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            if (e == null)
                return;
            var handle = e.Instance as IDeactivate;
            if (handle == null)
                return;
            handle.Deactivated += (s, ev) => e.Context.Resolve<IEventAggregator>().Unsubscribe(handle);
        }
    }
}