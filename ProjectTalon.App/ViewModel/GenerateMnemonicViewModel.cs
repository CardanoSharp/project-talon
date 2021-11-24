using CardanoSharp.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.ViewModel
{
    public interface IGenerateMnemonicViewModel
    {
        string GenerateMnemonicAsync();
    }
    public class GenerateMnemonicViewModel : IGenerateMnemonicViewModel
    {
        private IMnemonicService _mnemonicService;

        public GenerateMnemonicViewModel(IMnemonicService mnemonicService)
        {
            _mnemonicService = mnemonicService;
        }

        public string GenerateMnemonicAsync()
        {
            return _mnemonicService.Generate(24).Words;
        }
    }
}
