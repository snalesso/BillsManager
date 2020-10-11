using System.Collections.Generic;
using Billy.Domain.Billing.Models;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class AgentEditorViewModel : ValueObjectEditorViewModel<Agent>
    {
        public AgentEditorViewModel(Agent agent)
            : base(agent)
        {
            // TODO: make this setup automatic with some config which handles both setup & changes calc
            this.Name = agent?.Name;
            this.Surname = agent?.Surname;
            this.Phone = agent?.Phone;
        }

        #region properties

        private string _name;
        public string Name
        {
            get => this._name;
            set => this.Set(ref this._name, value);
            //get => this.GetTracked<string>();
            //set => this.SetAndTrack(value);
        }

        private string _surname;
        public string Surname
        {
            get => this._surname;
            set => this.Set(ref this._surname, value);
            //get => this.GetTracked<string>();
            //set => this.SetAndTrack(value);
        }

        private string _phone;
        public string Phone
        {
            get => this._phone;
            set => this.Set(ref this._phone, value);
            //get => this.GetTracked<string>();
            //set => this.SetAndTrack(value);
        }

        #endregion

        protected override void UpdateChanges()
        {
            this.UpdateChange(this, vm => vm.Name, agent => agent.Name);
            this.UpdateChange(this, vm => vm.Surname, agent => agent.Surname);
            this.UpdateChange(this, vm => vm.Phone, agent => agent.Phone);
        }

        protected override Agent GetEditedValueParser(IReadOnlyDictionary<string, object> changes)
        {
            return Agent.Create(
                this.GetFinalPropertyValue(x => x.Name),
                this.GetFinalPropertyValue(x => x.Surname),
                this.GetFinalPropertyValue(x => x.Phone));
        }

        //public override Agent GetEdited()
        //{
        //    var changes = this.GetChanges();
        //    //return Agent.Create(this._)
        //    throw new NotImplementedException();
        //}

        //protected override void SetOriginalValues()
        //{
        //    this.TrackOriginalValue(this, x => x.Name, x => x.Name);
        //    this.TrackOriginalValue(this, x => x.Surname, x => x.Surname);
        //    this.TrackOriginalValue(this, x => x.Phone, x => x.Phone);
        //}
    }
}