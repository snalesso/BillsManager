using System;

namespace BillsManager.Models
{
    public partial class Agent
    {
        #region ctor

        public Agent()
        {
        }

        public Agent(
            uint id,
            string name,
            string surname,
            string firstPhoneNumber,
            string secondPhoneNumber)
        {
            this.ID = id;
            this.Name = name;
            this.Surname = surname;
            this.FirstPhoneNumber = firstPhoneNumber;
            this.SecondPhoneNumber = secondPhoneNumber;
        }

        #endregion

        #region properties

        private uint id = 0;
        public uint ID
        {
            get { return this.id; }
            private set
            {
                this.id = value;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.Name != value)
                {
                    this.name = value;
                }
            }
        }

        private string surname;
        public string Surname
        {
            get
            {
                return this.surname;
            }
            set
            {
                if (this.Surname != value)
                {
                    this.surname = value;
                }
            }
        }

        private string firstPhoneNumber;
        public string FirstPhoneNumber
        {
            get
            {
                return this.firstPhoneNumber;
            }
            set
            {
                if (this.FirstPhoneNumber != value)
                {
                    this.firstPhoneNumber = value;
                }
            }
        }

        private string secondPhoneNumber;
        public string SecondPhoneNumber
        {
            get
            {
                return this.secondPhoneNumber;
            }
            set
            {
                if (this.SecondPhoneNumber != value)
                {
                    this.secondPhoneNumber = value;
                }
            }
        }

        #endregion
    }
}