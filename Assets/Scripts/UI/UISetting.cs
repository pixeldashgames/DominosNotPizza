using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Setting = GameSettingsController.GameSetting;

public class UISetting : MonoBehaviour
{
    static GameSettingsController Settings => GameSettingsController.Instance;
    private Setting _setting;

    [SerializeField] private TMP_Text label;
    
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Toggle toggle;
    
    public void Initialize(Setting setting)
    {
        _setting = setting;
        label.text = setting.name;

        switch (setting.type)
        {
            case GameSettingsController.GameSettingType.Dropbox:
                // add options in enum to dropdown
                dropdown.ClearOptions();
                dropdown.AddOptions(new List<string>(Enum.GetNames(setting.enumType)));
                dropdown.onValueChanged.AddListener(OnDropboxValueChanged);
                break;
            case GameSettingsController.GameSettingType.Integer
                or GameSettingsController.GameSettingType.Float or GameSettingsController.GameSettingType.String:
                inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                break;
            case GameSettingsController.GameSettingType.Boolean:
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
                break;
            default:
                throw new IndexOutOfRangeException();
        }
    }

    private void LateUpdate()
    {
        if(_setting == null)
            return;

        switch (_setting.type)
        {
            case GameSettingsController.GameSettingType.Dropbox:
                dropdown.value = (int)Settings.GetSettingValue(_setting.name);
                break;
            case GameSettingsController.GameSettingType.Boolean:
                toggle.isOn = (bool)Settings.GetSettingValue(_setting.name);
                break;
            case GameSettingsController.GameSettingType.Integer:
            case GameSettingsController.GameSettingType.Float:
            case GameSettingsController.GameSettingType.String:
                inputField.text = Settings.GetSettingValue(_setting.name).ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateValue(object value) => Settings.ChangeSetting(_setting, value);

    void OnDropboxValueChanged(int value) => UpdateValue(value);
    void OnInputFieldValueChanged(string value) => UpdateValue(value);
    void OnToggleValueChanged(bool value) => UpdateValue(value);
}