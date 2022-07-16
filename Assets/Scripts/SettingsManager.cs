using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public enum VolumeType
    {
        Music,
        Sound
    }

    [System.Serializable]
    public class VolumeSlider
    {
        public VolumeType type;
        public Slider slider;
    }

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private VolumeSlider[] volumeSliders;

    private static bool _initialized;
    private static float _masterVolume;
    private static Dictionary<VolumeType, float> _volumes;

    public void Awake()
    {
        TryInitialize();

        masterVolumeSlider.value = _masterVolume;
        
        foreach (var slider in volumeSliders)
            slider.slider.value = _volumes[slider.type];
    }
    
    public void SetMasterVolume(float vol) => _masterVolume = vol;
    public void SetMusicVolume(float vol) => _volumes[VolumeType.Music] = vol;
    public void SetSoundVolume(float vol) => _volumes[VolumeType.Sound] = vol;

    public static float GetVolume(VolumeType type)
    {
        TryInitialize();

        return _masterVolume * _volumes[type];
    }
    private static void TryInitialize()
    {
        if(_initialized)
            return;

        _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        _volumes = new Dictionary<VolumeType, float>();
        foreach (var type in Enum.GetNames(typeof(VolumeType)))
        {
            var value = (VolumeType)Enum.Parse(typeof(VolumeType), type);
            _volumes.Add(value, PlayerPrefs.GetFloat(type, 1f));
        }

        _initialized = true;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
        foreach (var type in Enum.GetNames(typeof(VolumeType)))
        {
            var value = (VolumeType)Enum.Parse(typeof(VolumeType), type);
            PlayerPrefs.SetFloat(type, _volumes[value]);
        }
        
        PlayerPrefs.Save();
    }
}
