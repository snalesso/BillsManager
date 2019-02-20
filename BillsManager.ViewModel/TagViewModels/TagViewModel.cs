using BillsManager.Models;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public class TagViewModel : Screen
    {
        #region ctor

        public TagViewModel(Tag tag)
        {
            this.ExposedTag = tag;
        }

        #endregion

        #region properties

        private Tag exposedTag;
        public Tag ExposedTag
        {
            get { return this.exposedTag; }
            protected set
            {
                if (this.exposedTag == value) return;

                this.exposedTag = value;
                this.NotifyOfPropertyChange(() => this.ExposedTag);
            }
        }

        public uint ID
        {
            get { return this.ExposedTag.ID; }
        }

        public string Name
        {
            get { return this.ExposedTag.Name; }
            set
            {
                if (this.ExposedTag.Name == value) return;

                this.ExposedTag.Name = value;
                this.NotifyOfPropertyChange(() => this.Name);
            }
        }

        public string Color
        {
            get { return this.ExposedTag.Color; }
            set
            {
                if (this.ExposedTag.Color == value) return;

                this.ExposedTag.Color = value;
                this.NotifyOfPropertyChange(() => this.Color);
                this.NotifyOfPropertyChange(() => this.Red);
                this.NotifyOfPropertyChange(() => this.Green);
                this.NotifyOfPropertyChange(() => this.Blue);
            }
        }

        // TODO: improve RGB logic
        public ushort Red
        {
            get { return this.GetRed(); }
            set
            {
                if (this.Red == value) return;

                this.SetRed(value);
                this.NotifyOfPropertyChange(() => this.Red);
            }
        }

        public ushort Green
        {
            get { return this.GetGreen(); }
            set
            {
                if (this.Green == value) return;

                this.SetGreen(value);
                this.NotifyOfPropertyChange(() => this.Green);
            }
        }

        public ushort Blue
        {
            get { return this.GetBlue(); }
            set
            {
                if (this.Blue == value) return;

                this.SetBlue(value);
                this.NotifyOfPropertyChange(() => this.Blue);
            }
        }
        
        #endregion

        #region methods

        private ushort GetRed()
        {
            var sharpOffset = this.Color.StartsWith(@"#") ? 1 : 0;

            var colStr = this.Color.Substring(sharpOffset + 2, 2);

            return ushort.Parse(colStr);
        }
        private void SetRed(ushort red)
        {
            var sharpOffset = this.Color.StartsWith(@"#") ? 1 : 0;
            var redStr = string.Format("{0:00}", red);
            var sharpAStr = this.Color.Substring(0, sharpOffset + 2);
            var GBStr = this.Color.Substring(sharpOffset + 4, 4);
            this.Color = sharpAStr + redStr + GBStr;
        }

        private ushort GetGreen()
        {
            var sharpOffset = this.Color.StartsWith(@"#") ? 1 : 0;

            var colStr = this.Color.Substring(sharpOffset + 4, 2);

            return ushort.Parse(colStr);
        }
        private void SetGreen(ushort green)
        {
            var sharpOffset = this.Color.StartsWith(@"#") ? 1 : 0;
            var greenStr = string.Format("{0:00}", green);
            var sharpARStr = this.Color.Substring(0, sharpOffset + 4);
            var BStr = this.Color.Substring(sharpOffset + 6, 2);
            this.Color = sharpARStr + greenStr + BStr;
        }

        private ushort GetBlue()
        {
            var sharpOffset = this.Color.StartsWith(@"#") ? 1 : 0;

            var colStr = this.Color.Substring(sharpOffset + 6, 2);

            return ushort.Parse(colStr);
        }
        private void SetBlue(ushort blue)
        {
            var sharpOffset = this.Color.StartsWith(@"#") ? 1 : 0;
            var blueStr = string.Format("{0:00}", blue);
            var sharpARGStr = this.Color.Substring(0, sharpOffset + 6);
            this.Color = sharpARGStr + blueStr;
        }

        #endregion
    }
}