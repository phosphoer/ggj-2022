using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public enum GameStage
    {
        Invalid,
        MainMenu,
        Settings,
        ScenarioIntro,
        ScenarioGameplay,
        ScenarioEnd,
        EndGame
    }

    public static event System.Action GameStateChangeEvent;

    private GameStage _gameStage = GameStage.Invalid;
    public GameStage CurrentStage => _gameStage;
    public GameStage EditorDefaultStage = GameStage.ScenarioGameplay;

    public SoundBank MusicMenuLoop;
    //public CameraControllerBase MenuCamera;


    // Start is called before the first frame update
    private void Start()
    {
        // Base camera controller
        //CameraControllerStack.Instance.PushController(MenuCamera);

        GameStage InitialStage = GameStage.MainMenu;
#if UNITY_EDITOR
        InitialStage = EditorDefaultStage;
#endif

        SetGameStage(InitialStage);
    }

    // Update is called once per frame
    private void Update()
    {
        GameStage nextGameStage = _gameStage;

        switch (_gameStage)
        {
            case GameStage.MainMenu:
            case GameStage.Settings:
                break;
            case GameStage.ScenarioIntro:
                break;
            case GameStage.ScenarioGameplay:
                break;
            case GameStage.ScenarioEnd:
                //nextGameStage = GameStage.Daytime;
                break;
            case GameStage.EndGame:
                break;
        }

        SetGameStage(nextGameStage);
    }

    public void NewGame()
    {
        SetGameStage(GameStage.ScenarioGameplay);
    }

    public void Settings()
    {
        SetGameStage(GameStage.Settings);
    }

    public void SetGameStage(GameStage newGameStage)
    {
        if (newGameStage != _gameStage)
        {
            OnExitStage(_gameStage, newGameStage);
            OnEnterStage(newGameStage);
            _gameStage = newGameStage;
        }
    }

    public void OnExitStage(GameStage oldGameStage, GameStage newGameStage)
    {
        switch (oldGameStage)
        {
            case GameStage.MainMenu:
                {
                    if (MusicMenuLoop != null && newGameStage != GameStage.Settings)
                    {
                        AudioManager.Instance.FadeOutSound(gameObject, MusicMenuLoop, 3f);
                    }

                    //CameraControllerStack.Instance.PopController(MenuCamera);

                    //GameUI.Instance.MainMenuUI.Hide();
                }
                break;
            case GameStage.Settings:
                {
                    //CameraControllerStack.Instance.PopController(MenuCamera);

                    //GameUI.Instance.SettingsUI.Hide();
                }
                break;
            case GameStage.ScenarioIntro:
                {
                    //CameraControllerStack.Instance.PopController(MenuCamera);

                    //GameUI.Instance.DayIntroUI.Hide();
                }
                break;
            case GameStage.ScenarioGameplay:
                {

                }
                break;
            case GameStage.ScenarioEnd:
                {

                }
                break;
            case GameStage.EndGame:
                {
                    //CameraControllerStack.Instance.PopController(MenuCamera);

                    //GameUI.Instance.WinGameUI.Hide();
                }
                break;
        }
    }

    public void OnEnterStage(GameStage newGameStage)
    {
        GameStateChangeEvent?.Invoke();

        switch (newGameStage)
        {
            case GameStage.MainMenu:
                {
                    //GameUI.Instance.MainMenuUI.Show();

                    if (MusicMenuLoop != null)
                    {
                        AudioManager.Instance.FadeInSound(gameObject, MusicMenuLoop, 3.0f);
                    }

                    ResetGameStats();
                }
                break;
            case GameStage.Settings:
                {
                    //GameUI.Instance.SettingsUI.Show();
                    //CameraControllerStack.Instance.PushController(MenuCamera);
                }
                break;
            case GameStage.ScenarioIntro:
                {
                    //GameUI.Instance.GameplayUI.Show();
                }
                break;
            case GameStage.ScenarioGameplay:
                {
                    //GameUI.Instance.GameplayUI.Show();
                }
                break;
            case GameStage.ScenarioEnd:
                {
                    //GameUI.Instance.GameplayUI.Show();
                }
                break;
            case GameStage.EndGame:
                {
                    //GameUI.Instance.WinGameUI.Show();
                    //CameraControllerStack.Instance.PushController(MenuCamera);

                    //if (WinAlert != null)
                    //{
                    //    AudioManager.Instance.PlaySound(WinAlert);
                    //}
                }
                break;
        }
    }

    void ResetGameStats()
    {
    }
}
