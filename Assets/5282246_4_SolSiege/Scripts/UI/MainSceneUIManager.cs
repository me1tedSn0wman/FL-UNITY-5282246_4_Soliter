using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MainSceneUIManager : Singleton<MainSceneUIManager>
{
    [Header("Buttons")]
    public Button button_Settings;
    public Button button_Info;
    public Button button_StartGame;

    public Button button_CloseSettingsCanvas;
    public Button button_CloseInfoCanvas;

    [Header("Canvases")]
    public GameObject canvasGO_Settings;
    public GameObject canvasGO_Info;

    [Header("Slider")]
    public Slider slider_SoundVolume;

    [Header("Textes")]
    public TextMeshProUGUI text_SoundVolume;

    [Header("UI Audio Clip")]

    public AudioClip audioClipUI;
    public AudioSource audioSourceUI;

    public void Awake()
    {
        audioSourceUI = GetComponent<AudioSource>();

        canvasGO_Settings.SetActive(false);
        canvasGO_Info.SetActive(false);

        GameManager.OnSoundVolumeChanged += SetVolume;
        SetVolume(GameManager.soundVolume);
        /*
         * 
         buttons
         */
        button_Settings.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            canvasGO_Settings.SetActive(true);
        });
        button_Info.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            canvasGO_Info.SetActive(true);
        });
        button_StartGame.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            GameManager.LoadGameplayScene();
        });

        /*
         close buttons
         */

        button_CloseSettingsCanvas.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            canvasGO_Settings.SetActive(false);
        });
        button_CloseInfoCanvas.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            canvasGO_Info.SetActive(false);
        });

        /*
         sliders
         */

        slider_SoundVolume.onValueChanged.AddListener((value) =>
        {
            GameManager.soundVolume = value;
            text_SoundVolume.text = ((int)(value * 100)).ToString();
        });

        /*
         
         */

        Init();
    }

    private void SetVolume(float volume)
    {
        audioSourceUI.volume = volume;
        text_SoundVolume.text = ((int)(volume * 100)).ToString();
        slider_SoundVolume.value = volume;
    }

    public override void OnDestroy()
    {
        GameManager.OnSoundVolumeChanged -= SetVolume;
        base.OnDestroy();
    }

    private void Init() { 
    }
}
