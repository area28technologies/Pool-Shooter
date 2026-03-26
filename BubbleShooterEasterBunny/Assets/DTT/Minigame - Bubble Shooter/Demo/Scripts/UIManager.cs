using DTT.MinigameBase.LevelSelect;
using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance => instance;

        [SerializeField] private GameObject settingPanel;
        //[SerializeField] private Slider soundVolumeSlider;
        //[SerializeField] private Slider bgmVolumeSlider;
        //[SerializeField] private AudioSource soundSource;

        [SerializeField] private Slider soundVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;

        [SerializeField] private AudioClip mainMenuBgMusic;

        private bool checkSetting;
        //private float soundValues;

        private void Awake()
        {
            //soundVolumeSlider.value = 1;
            //MusicVolume();
            //soundSource.Play();
        }

        private void Start()
        {
            soundVolumeSlider.value = RCGSoundSetting.Instance.soundSource.volume;
            bgmVolumeSlider.value = RCGSoundSetting.Instance.bgMusicSource.volume;

            RCGSoundSetting.Instance.PlayBGMusic(mainMenuBgMusic);
        }

        private void Update()
        {
            //if (soundVolumeSlider.value != soundValues)
            //{
            //    MusicVolume();
            //}
        }
        private void OnEnable()
        {
            bgmVolumeSlider.onValueChanged.AddListener(MusicVolumeSlider);
            soundVolumeSlider.onValueChanged.AddListener(MusicSoundSlider);
        }

        private void OnDisable()
        {
            bgmVolumeSlider.onValueChanged.RemoveListener(MusicVolumeSlider);
            soundVolumeSlider.onValueChanged.RemoveListener(MusicSoundSlider);
        }

        //private void MusicVolume()
        //{
        //    soundValues = soundVolumeSlider.value;

        //    soundSource.volume = soundValues;
        //}

        public void OnClickFreeModel()
        {
            SceneManager.LoadScene("Portrait_Refactor");
        }

        public void OnClickBackToMenu()
        {
            SceneManager.LoadScene("Main");
        }

        public void OnClickSetting()
        {
            if (!checkSetting)
            {
                settingPanel.SetActive(true);

                checkSetting = true;
            }
            else
            {
                settingPanel.SetActive(false);

                checkSetting = false;
            }

        }

        public void PlayClickSound(AudioClip clip)
        {
            RCGSoundSetting.Instance.PlayClickSound(clip);
        }

        public void PlayClickHover(AudioClip clip)
        {
            RCGSoundSetting.Instance.PlayHoverSound(clip);
        }

        private void MusicVolumeSlider(float value)
        {
            RCGSoundSetting.Instance.bgMusicSource.volume = value;
        }

        private void MusicSoundSlider(float value)
        {
            RCGSoundSetting.Instance.soundSource.volume = value;
        }
    }
}
