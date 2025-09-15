using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace IBudget.GUI.ViewModels
{
    public partial class DictionariesPageViewModel : ViewModelBase
    {
        public ObservableCollection<InfoContainer> ExpenseDictionariesInfo { get; } = new();
        public ObservableCollection<InfoContainer> RuleDictionariesInfo { get; } = new();

        public List<ExpenseTag> ExpenseDictionaries { get; set; }
        public List<RuleDictionary> RuleDictionaries { get; set; }

        [ObservableProperty]
        private bool _isLoadingED = true;

        [ObservableProperty]
        private bool _isLoadingRD = true;

        private readonly IUserDictionaryService _userDictionaryService;

        public DictionariesPageViewModel(IUserDictionaryService userDictionaryService)
        {
            _userDictionaryService = userDictionaryService;
            InitaliseDbSearch();
        }

        public void RefreshView()
        {
            InitaliseDbSearch();
        }

        private async void InitaliseDbSearch()
        {
            ExpenseDictionariesInfo.Clear();
            RuleDictionariesInfo.Clear();
            try
            {
                var expenseDictionariesTask = GetExpenseDictionariesAsync();
                var ruleDictionariesTask = GetRuleDictionariesAsync();

                await Task.WhenAll(expenseDictionariesTask, ruleDictionariesTask);

                ExpenseDictionaries = await expenseDictionariesTask;
                RuleDictionaries = await ruleDictionariesTask;

                FinishEdDbSearch();
                IsLoadingED = false;

                FinishRdDbSearch();
                IsLoadingRD = false;
            }
            catch (Exception)
            {
            }

            // Dummy data
            //for (int i = 0; i < 30; i++)
            //{
            //    ExpenseDictionariesInfo.Add(new InfoContainer() { Key = $"Sample_Expense_{i + 1}", Value = $"Sample_Value_{i + 1}" });
            //    RuleDictionariesInfo.Add(new InfoContainer() { Key = $"Sample_Rule_{i + 1}", Value = $"Sample_Value_{i + 1}" });
            //}
        }

        private async Task<List<ExpenseTag>> GetExpenseDictionariesAsync()
        {
            return await _userDictionaryService.GetExpenseDictionaries(-1);
        }

        private async Task<List<RuleDictionary>> GetRuleDictionariesAsync()
        {
            return await _userDictionaryService.GetRuleDictionaries(-1);
        }

        private void FinishEdDbSearch()
        {
            if (ExpenseDictionaries is not null)
            {
                foreach (var eD in ExpenseDictionaries)
                {
                    ExpenseDictionariesInfo.Add(new InfoContainer() { Key = eD.title, Value = eD.tags.First() });
                }
            }
        }

        private void FinishRdDbSearch()
        {
            if (RuleDictionaries is not null)
            {
                foreach (var rD in RuleDictionaries)
                {
                    RuleDictionariesInfo.Add(new InfoContainer() { Key = rD.rule, Value = rD.tags.First() });
                }
            }
        }
    }

    public class InfoContainer
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
