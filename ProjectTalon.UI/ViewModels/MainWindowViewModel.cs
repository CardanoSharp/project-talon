using System;
using System.Collections.Generic;
using System.Text;
using ProjectTalon.Core.Data;

namespace ProjectTalon.UI.ViewModels
{
    public interface IMainWindowViewModel
    {
        string Greeting { get; }
    }
    
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private readonly IWalletDatabase _walletDatabase;

        public MainWindowViewModel(IWalletDatabase walletDatabase)
        {
            _walletDatabase = walletDatabase;
        }

        public string Greeting => "Welcome to Avalonia!";
    }
}
