using System;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class StepBase : ViewModelBase
    {
        public event EventHandler? RequestStepOver;
        public event EventHandler? RequestStepBack;
        protected void OnSteppingOver()
        {
            RequestStepOver?.Invoke(this, EventArgs.Empty);
        }
        protected void OnSteppingBack()
        {
            RequestStepBack?.Invoke(this, EventArgs.Empty);
        }
    }
}
