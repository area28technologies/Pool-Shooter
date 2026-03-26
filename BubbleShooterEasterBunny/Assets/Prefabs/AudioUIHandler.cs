using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioUIHandler : MonoBehaviour
{
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

    //[SerializeField] private GameObject audioSettingsUI;

    private void Start()
    {
        RCGSoundSetting.Instance.UpdateUI(soundVolumeSlider, bgmVolumeSlider);
        RCGSoundSetting.Instance.SetVolume();
    }

    public void IncreaseSoundVolume(float value)
    {
        RCGSoundSetting.Instance.IncreaseSoundVolume(value);
    }

    public void DecreaseSoundVolume(float value)
    {
        RCGSoundSetting.Instance.DecreaseSoundVolume(value);
    }

    public void IncreaseMusicVolume(float value)
    {
        RCGSoundSetting.Instance.IncreaseMusicVolume(value);
    }

    public void DecreaseMusicVolume(float value)
    {
        RCGSoundSetting.Instance.DecreaseMusicVolume(value);
    }
    
    public void MuteSound()
    {
        RCGSoundSetting.Instance.MuteSound();
    }
}
