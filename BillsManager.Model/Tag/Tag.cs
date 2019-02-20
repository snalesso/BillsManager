namespace BillsManager.Models
{
    public partial class Tag
    {
        #region ctor

        public Tag(uint id)
        {
            this.id = id;
        }

        public Tag(
            uint id,
            string name,
            string color)
            : this(id)
        {
            this.Name = name;
            this.Color = color; // IDEA: add string format validation to prevent runtime parse exception?
        }

        #endregion

        #region properties

        private uint id;
        public uint ID
        {
            get { return this.id; }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name == value) return;

                this.name = value;
            }
        }

        private string color;
        public string Color
        {
            get { return this.color; }
            set
            {
                if (this.color == value) return;

                this.color = value;
            }
        }

        #endregion
    }
}