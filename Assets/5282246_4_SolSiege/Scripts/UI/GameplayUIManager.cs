using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameplayUIManager : Singleton<GameplayUIManager>
{
    [Header("Buttons")]
    public Button button_Settings;
    public Button button_Info;
    public Button button_Restart;
    public Button button_Undo;

    public Button button_SettingsToMainMenu;

    public Button button_GameOverRestart;
    public Button button_GameOverToMainMenu;
    

    public Button button_CloseSettingCanvas;
    public Button button_CloseInfoCanvas;

    [Header("Canvases")]
    public GameObject canvasGO_Settings;
    public GameObject canvasGO_Info;
    public GameObject canvasGO_GameOver;

    public GameObject canvasGO_GameplayArea;

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
        canvasGO_GameOver.SetActive(false);

        GameManager.OnSoundVolumeChanged += SetVolume;

        SetVolume(GameManager.soundVolume);
        /*
         buttons 
         */

        button_Settings.onClick.AddListener(() => {
            audioSourceUI.PlayOneShot(audioClipUI);
            canvasGO_Settings.SetActive(true);
        });
        button_Info.onClick.AddListener(() => {
            audioSourceUI.PlayOneShot(audioClipUI);
            canvasGO_Info.SetActive(true);
        });
        button_Restart.onClick.AddListener(() => {
            audioSourceUI.PlayOneShot(audioClipUI);
            GameManager.LoadGameplayScene();
        });
        button_Undo.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            GameplayManagerSiegeSol.Instance.LoadTurn();
        });


        button_SettingsToMainMenu.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            GameManager.LoadMainMenuScene();
        });

        /*
         close buttons
         */

        button_CloseSettingCanvas.onClick.AddListener(() =>
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
         game over buttons
         */

        button_GameOverRestart.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            GameManager.LoadGameplayScene();
        });
        button_GameOverToMainMenu.onClick.AddListener(() =>
        {
            audioSourceUI.PlayOneShot(audioClipUI);
            GameManager.LoadMainMenuScene();
        });

        /*
         sliders
         */

        slider_SoundVolume.onValueChanged.AddListener((value) =>
        {
            GameManager.soundVolume = value;
            text_SoundVolume.text = ((int)(value*100)).ToString();
        });
    }

    public static void GAME_OVER() {
        Instance.canvasGO_GameOver.SetActive(true);
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

    public Vector2 GetGameplayAreaXMinMax() {
        RectTransform gameplayAreaRectTransf = canvasGO_GameplayArea.GetComponent<RectTransform>();
        Vector3[] v = new Vector3[4];  // 0 - bottomLeft , 3 - bottom rigth
        gameplayAreaRectTransf.GetWorldCorners(v);
        

        Vector2 tVect = new Vector2(v[0].x, v[3].x);
        Debug.Log("Area xMin, xMax:" +  tVect);

        return tVect;
    }
}
