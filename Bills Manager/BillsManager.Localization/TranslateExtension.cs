using System;
using System.Linq.Expressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace BillsManager.Localization
{
    public class TranslateExtension : MarkupExtension
    {
        #region ctor

        //#if DEBUG

        //        private static ITranslationProvider traslationProvider = new ResxTranslationProvider(
        //            Assembly.GetAssembly(typeof(;

        //#endif

        public TranslateExtension(string key)
        {
            this.key = key;
        }

        #endregion

        #region properties

        private string key;
        [ConstructorArgument("key")]
        public string Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        #endregion

        #region methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var td = new TranslationData(this.key);
            BindingMode bindingMode = BindingMode.OneWay;

            var pvt = (serviceProvider as IProvideValueTarget); // TODO: add design time support: serviceProvider == null
            if (pvt != null)
            {
                var targetType = pvt.TargetObject.GetType();

                if (targetType != typeof(Run))
                    bindingMode = BindingMode.Default;
            }

            var propertyName = this.GetPropertyName(() => td.Value);

            var binding =
                new Binding(propertyName)
                {
                    Source = td,
                    Mode = bindingMode
                };

            return binding.ProvideValue(serviceProvider);
        }

        String GetPropertyName<TValue>(Expression<Func<TValue>> propertyId)
        {
            return ((MemberExpression)propertyId.Body).Member.Name;
        }

        #endregion
    }
}