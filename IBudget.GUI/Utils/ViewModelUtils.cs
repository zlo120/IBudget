using IBudget.GUI.ViewModels;
using System;
using System.Collections.Generic;

namespace IBudget.GUI.Utils
{
    public static class ViewModelUtils
    {
        public static ViewModelBase ResolveViewModel(this List<ViewModelBase> viewModels, Type viewModelType)
        {
            if (!viewModelType.IsSubclassOf(typeof(ViewModelBase)))
                throw new ArgumentException($"The type {viewModelType.Name} is not a ViewModelBase type");

            foreach (ViewModelBase viewModel in viewModels)
            {
                if (viewModelType.IsAssignableFrom(viewModel.GetType()))
                    return viewModel;
            }

            throw new Exception($"There were no view models of type {viewModelType.Name}");
        }
    }
}
