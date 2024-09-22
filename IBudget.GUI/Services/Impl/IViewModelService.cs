using IBudget.GUI.ViewModels;
using System;

namespace IBudget.GUI.Services.Impl
{
    public interface IViewModelService
    {
        ViewModelBase CreateViewModel(Type type);
    }
}
