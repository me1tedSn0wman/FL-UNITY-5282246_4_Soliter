using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public enum GameState { 
    PreGame
        , MainMenu
        , Gameplay
        , GameOver
        , FadeInOut
}

public class GameManager : Soliton<GameManager>
{
    [Header("Game state")]

    [SerializeField] private static GameState _gameState;
    public static GameState gameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }

    public bool IS_MOBILE;
    public bool SIMULATE_MOBILE;

    [Header("Gameplay Settings")]
    public int something;

    [Header("Game Settings")]
    [SerializeField] private static float _soundVolume = 1;
    public static float soundVolume {
        get { return _soundVolume; }
        set { 
            _soundVolume = Mathf.Clamp01(value);
            OnSoundVolumeChanged(_soundVolume);
        }
    }

    [Header("Screen Size")]
    public Vector2Int screenSize;

    /*
     Actions
     */

    public static event Action<float> OnSoundVolumeChanged;
    public static event Action<Vector2Int> OnScreenSizeChanged;

    public override void Awake()
    {
        base.Awake();

        gameState = GameState.PreGame;
        IS_MOBILE = false
            || SIMULATE_MOBILE;

        screenSize = new Vector2Int(Screen.width, Screen.height);

        OnScreenSizeChanged += Foo;

        gameState = GameState.MainMenu;
    }

    public void Update()
    {
        UpdateScreenSize();
    }

    #region SceneManagement

    public static void LoadMainMenuScene() {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    public static void LoadGameplayScene()
    {
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }

    #endregion SceneManagement

    #region TechManagement

    void UpdateScreenSize() {
        Vector2Int newScreenSize = new Vector2Int(Screen.width, Screen.height);
        if (screenSize != newScreenSize) { 
            OnScreenSizeChanged(newScreenSize);
            screenSize = newScreenSize;
        }
    }

    #endregion TechManagement

    private void Foo(Vector2Int newSize) { 
    }
}
