using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableVolume : MonoBehaviour
{
    [SerializeField] private SettingsManager.VolumeType volumeType;
    [SerializeField] private AudioSource audioSource;
    
    void Update()
    {
        audioSource.volume = SettingsManager.GetVolume(volumeType);
    }
}
