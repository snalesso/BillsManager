using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace BillsManager.Views.Controls
{
    public class PopupEx : Popup
    {
        #region fields

        protected Window parentWindow;

        private readonly MethodInfo updatePositionMethod;

        #endregion

        #region ctor

        public PopupEx()
            : base()
        {
            this.updatePositionMethod = typeof(Popup)
                .GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }

        #endregion

        #region properties

        private bool follow = true;
        public bool Follow
        {
            get { return this.follow; }
            set
            {
                if (this.follow == value) return;

                this.follow = value;
            }
        }

        #endregion

        #region methods

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            this.parentWindow = Window.GetWindow(this);
            if (parentWindow == null) return;

            parentWindow.LocationChanged += OnParentWindowLocationChanged;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (parentWindow != null)
            {
                parentWindow.LocationChanged -= OnParentWindowLocationChanged;
                this.parentWindow = null;
            }
            else
                throw new NullReferenceException();
        }

        private void OnParentWindowLocationChanged(object sender, EventArgs e)
        {
            if (this.IsOpen & this.Follow)
                this.UpdateLocation();
        }

        public void UpdateLocation()
        {
            if (this.updatePositionMethod != null)
                this.updatePositionMethod.Invoke(this, null);
        }

        #endregion

    }
}