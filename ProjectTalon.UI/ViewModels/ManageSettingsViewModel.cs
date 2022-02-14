using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json.Serialization;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ReactiveUI;
using Splat;

namespace ProjectTalon.UI.ViewModels;

public class ManageSettingsViewModel: ViewModelBase
{
    private readonly ISettingsDatabase _settingsDatabase;

    private SettingsForm form;
    public SettingsForm Form
    {
        get => form;
        set => this.RaiseAndSetIfChanged(ref form, value);
    }

    private bool showSaveSuccess = false;
    public bool ShowSaveSuccess
    {
        get => showSaveSuccess;
        set => this.RaiseAndSetIfChanged(ref showSaveSuccess, value);
    }
    
    public ManageSettingsViewModel(ISettingsDatabase settingsDatabase)
    {
        _settingsDatabase = settingsDatabase;
        form = new SettingsForm();
        
        LoadCurrentSettings();
    }

    private async void LoadCurrentSettings()
    {
        var settings = await _settingsDatabase.ListAsync();
        var form = new SettingsForm();
        foreach (var s in settings)
        {
            switch (s.Key)
            {
                case SettingKeys.NETWORK:
                    form.NetworkValue = s.Value == NetworkOptions.TESTNET
                        ? 0
                        : 1;
                    break;
                case SettingKeys.API_ENABLED:
                    form.ApiEnabledValue = bool.Parse(s.Value);
                    break;
            }
        }

        Form = form;
    }

    public async void SaveSettings()
    {
        var settings = await _settingsDatabase.ListAsync();
        foreach (var s in settings)
        {
            switch (s.Key)
            {
                case SettingKeys.NETWORK:
                    s.Value = Form.NetworkValue == 0
                        ? NetworkOptions.TESTNET
                        : NetworkOptions.MAINNET;
                    break;
                case SettingKeys.API_ENABLED:
                    s.Value = Form.ApiEnabledValue.ToString();
                    break;
            }

            await _settingsDatabase.SaveAsync(s);
        }

        ShowSaveSuccess = true;
        await Task.Delay(3000);
        ShowSaveSuccess = false;
    }
}

public class SettingsForm
{
    public int NetworkValue { get; set; }
    public bool ApiEnabledValue { get; set; }
}