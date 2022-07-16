using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsDrawer : MonoBehaviour
{
    // Create UI components from prefabs for each setting in the current GameSettingsController instance (according to setting type), placed under different Transforms according to setting category
    [SerializeField] private  GameObject dropboxPrefab;
    [SerializeField] private  GameObject inputFieldPrefab;
    [SerializeField] private  GameObject togglePrefab;

    [SerializeField] private Transform playerSection;
    [SerializeField] private Transform rulesSection;
    [SerializeField] private Transform piecesSection;

    void Start()
    {
        (Transform parent, List<GameSettingsController.GameSetting> settings)[]
            settingsByCategory = new (Transform, List<GameSettingsController.GameSetting>)[]
            {
                (playerSection, GameSettingsController.Instance.PlayerSettings),
                (rulesSection, GameSettingsController.Instance.RulesSettings),
                (piecesSection, GameSettingsController.Instance.PieceSettings)
            };
        
        foreach(var category in settingsByCategory)
            foreach(var setting in category.settings)
            {
                GameObject prefab;
                switch(setting.type)
                {
                    case GameSettingsController.GameSettingType.Dropbox:
                        prefab = dropboxPrefab;
                        break;
                    case GameSettingsController.GameSettingType.Integer:
                    case GameSettingsController.GameSettingType.Float:
                    case GameSettingsController.GameSettingType.String:
                        prefab = inputFieldPrefab;
                        break;
                    case GameSettingsController.GameSettingType.Boolean:
                        prefab = togglePrefab;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var instance = Instantiate(prefab, category.parent);
                instance.GetComponent<UISetting>().Initialize(setting);
            }
    }
}