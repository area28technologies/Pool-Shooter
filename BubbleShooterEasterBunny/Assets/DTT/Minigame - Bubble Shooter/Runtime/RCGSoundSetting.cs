using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RCGSoundSetting : MonoBehaviour
{
    public AudioSource soundSource;
    public AudioSource bgMusicSource;


    //[SerializeField] private GameObject audioSettingsUI;

    [SerializeField] private float currentSoundVolume;
    [SerializeField] private float currentMusicVolume;

    [SerializeField] private AudioClip backgroundMusic;

    private static string playerPrefsSound = "SoundVolume";
    private static string playerPrefsMusic = "MusicVolume";

    private static RCGSoundSetting instance;
    public static RCGSoundSetting Instance => instance;

    public bool IsSoundMuted => soundSource.volume <= 0;
    public bool IsMusicMuted => bgMusicSource.volume <= 0;

    public float CurrentSoundVolume { get => currentSoundVolume; }
    public float CurrentMusicVolume { get => currentMusicVolume; }

    private void Awake()
    {
        //// Debug.Log("Sound Get: " + PlayerPrefs.GetFloat(playerPrefsSound, soundSource.volume));
        //// Debug.Log("Music Get: " + PlayerPrefs.GetFloat(playerPrefsMusic, bgMusicSource.volume));
        //SetVolume();
        //// Debug.Log("Sound Get: " + PlayerPrefs.GetFloat(playerPrefsSound, soundSource.volume));
        //// Debug.Log("Music Get: " + PlayerPrefs.GetFloat(playerPrefsMusic, bgMusicSource.volume));

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume()
    {
        if (soundSource != null)
        {
            currentSoundVolume = PlayerPrefs.GetFloat(playerPrefsSound, soundSource.volume);
            soundSource.volume = currentSoundVolume;
        }

        if (bgMusicSource != null)
        {
            currentMusicVolume = PlayerPrefs.GetFloat(playerPrefsMusic, bgMusicSource.volume);
            bgMusicSource.volume = currentMusicVolume;
        }

        //if (soundVolumeSlider != null) soundVolumeSlider.value = currentSoundVolume;
        //if (bgmVolumeSlider != null) bgmVolumeSlider.value = currentMusicVolume;
    }

    public void IncreaseSoundVolume(float value)
    {
        currentSoundVolume += value;
        if (currentSoundVolume > 1) currentSoundVolume = 1;
        if (soundSource.mute) soundSource.mute = false;
        soundSource.volume = currentSoundVolume;
        //soundVolumeSlider.value = currentSoundVolume;
        SaveSoundVolume();
    }

    public void DecreaseSoundVolume(float value)
    {
        currentSoundVolume -= value;
        if (currentSoundVolume < 0) currentSoundVolume = 0;
        if (soundSource.mute) soundSource.mute = false;
        soundSource.volume = currentSoundVolume;
        //soundVolumeSlider.value = currentSoundVolume;
        SaveSoundVolume();
    }

    public void IncreaseMusicVolume(float value)
    {
        currentMusicVolume += value;
        if (currentMusicVolume > 1) currentMusicVolume = 1;
        if (bgMusicSource.mute) bgMusicSource.mute = false;
        bgMusicSource.volume = currentMusicVolume;
        //bgmVolumeSlider.value = currentMusicVolume;
        SaveSoundVolume();
    }

    public void DecreaseMusicVolume(float value)
    {
        currentMusicVolume -= value;
        if (currentMusicVolume < 0) currentMusicVolume = 0;
        if (bgMusicSource.mute) bgMusicSource.mute = false;
        bgMusicSource.volume = currentMusicVolume;
        //bgmVolumeSlider.value = currentMusicVolume;
        SaveSoundVolume();
    }

    public void SaveSoundVolume()
    {
        if (soundSource != null)
        {
            PlayerPrefs.SetFloat(playerPrefsSound, currentSoundVolume);
        }

        if (bgMusicSource != null)
        {
            PlayerPrefs.SetFloat(playerPrefsMusic, currentMusicVolume);
        }
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         // float a = PlayerPrefs.GetFloat(playerPrefsSound, soundSource.volume);
    //         // float b= PlayerPrefs.GetFloat(playerPrefsMusic, bgMusicSource.volume);
    //         // Debug.Log("Sound Get: " + a);
    //         // Debug.Log("Music Get: " + b);
    //     }
    // }

    public void PlayBGMusic(AudioClip audioClip)
    {
        bgMusicSource.clip = audioClip;
        bgMusicSource.loop = true;
        bgMusicSource.Play();

        //Debug.Log("Play BackGround music");
    }

    public void PlaySFX(List<AudioClip> clip)
    {
        //if (soundSource.isPlaying) return;
        int randomClip = Random.Range(0, clip.Count);

        soundSource.PlayOneShot(clip[randomClip]);
        //soundSource.PlayOneShot(clip[0]);
    }

    public void UpdateUI(Slider soundSlider, Slider bgSlider)
    {
        //this.soundVolumeSlider = soundSlider;
        //this.bgmVolumeSlider = bgSlider;
        //this.audioSettingsUI = audioSettingUI;
    }

    public void MuteSound()
    {
        bgMusicSource.mute = !bgMusicSource.mute;
    }

    public void PlayClickSound(AudioClip clip)
    {
        if (soundSource != null && clip != null)
        {
            soundSource.PlayOneShot(clip);
        }
    }

    public void PlayHoverSound(AudioClip clip)
    {
        if (soundSource != null && clip != null)
        {
            soundSource.PlayOneShot(clip);
        }
    }
}
