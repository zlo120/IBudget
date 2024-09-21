using System;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class StepBase : ViewModelBase
    {
        public event EventHandler? RequestStepOver;
        protected void OnSteppingOver()
        {
            RequestStepOver?.Invoke(this, EventArgs.Empty);
        }
    }
}
