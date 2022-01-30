using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayer
{
    Invalid,
    DevilPlayer,
    AngelPlayer
}

public class GameStateManager : Singleton<GameStateManager>
{
    public enum GameStage
    {
        Invalid,
        MainMenu,
        Settings,
        ScenarioIntro,
        ScenarioGameplay,
        ScenarioOutro,
        EndGame
    }

    public static event System.Action GameStateChangeEvent;

    private GameStage _gameStage = GameStage.Invalid;
    public GameStage CurrentStage => _gameStage;
    public GameStage EditorDefaultStage = GameStage.ScenarioIntro;

    public SoundBank MusicMenuLoop;
    public CameraControllerBase MenuCamera;

    private void Awake()
    {
        Instance = this;
        PlayerManager.PlayerJoined += OnPlayerJoined;
    }

    private void OnDestroy()
    {
        PlayerManager.PlayerJoined -= OnPlayerJoined;
    }

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
                if (GameUI.Instance.ScenarioIntroUI.IsComplete())
                {
                    nextGameStage = GameStage.ScenarioGameplay;
                }
                break;
            case GameStage.ScenarioGameplay:
                break;
            case GameStage.ScenarioOutro:
                if (GameUI.Instance.ScenarioOutroUI.IsComplete())
                {
                    nextGameStage = GameStage.ScenarioIntro;
                }
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

                    CameraManager.Instance.MenuCameraStack.PopCurrentController();

                    GameUI.Instance.MainMenuUI.Hide();
                }
                break;
            case GameStage.Settings:
                {
                    CameraManager.Instance.MenuCameraStack.PopCurrentController();

                    GameUI.Instance.SettingsUI.Hide();
                }
                break;
            case GameStage.ScenarioIntro:
                {
                    CameraManager.Instance.ScenarioCameraStack.PopCurrentController();

                    GameUI.Instance.ScenarioIntroUI.Hide();
                }
                break;
            case GameStage.ScenarioGameplay:
                {
                    CameraManager.Instance.ScenarioCameraStack.PopCurrentController();
                    GameUI.Instance.ScenarioUI.Hide();
                    GameUI.Instance.AngelUI.Hide();
                    GameUI.Instance.DevilUI.Hide();
                }
                break;
            case GameStage.ScenarioOutro:
                {
                    DespawnPlayers();
                    ScenarioManager.Instance.TeardownScenario();
                    CameraManager.Instance.ScenarioCameraStack.PopCurrentController();
                    GameUI.Instance.ScenarioOutroUI.Hide();
                }
                break;
            case GameStage.EndGame:
                {
                    CameraManager.Instance.ScenarioCameraStack.PopCurrentController();

                    GameUI.Instance.EndGameUI.Hide();
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
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MenuCamera);

                    GameUI.Instance.MainMenuUI.Show();

                    if (MusicMenuLoop != null)
                    {
                        AudioManager.Instance.FadeInSound(gameObject, MusicMenuLoop, 3.0f);
                    }

                    ResetGameStats();
                }
                break;
            case GameStage.Settings:
                {
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MenuCamera);

                    GameUI.Instance.SettingsUI.Show();
                    //CameraControllerStack.Instance.PushController(MenuCamera);
                }
                break;
            case GameStage.ScenarioIntro:
                {
                    ScenarioManager.Instance.SetupScenario();

                    SpawnPlayers();

                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.ScenarioCamera);
                    GameUI.Instance.ScenarioIntroUI.Show();
                }
                break;
            case GameStage.ScenarioGameplay:
                {
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MultiCamera);
                    
                    GameUI.Instance.ScenarioUI.Show();

                    GameUI.Instance.AngelUI.AssignPlayer(ePlayer.AngelPlayer);
                    GameUI.Instance.AngelUI.Show();

                    GameUI.Instance.DevilUI.AssignPlayer(ePlayer.DevilPlayer);
                    GameUI.Instance.DevilUI.Show();
                }
                break;
            case GameStage.ScenarioOutro:
                {
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.ScenarioCamera);
                    GameUI.Instance.ScenarioOutroUI.Show();
                }
                break;
            case GameStage.EndGame:
                {
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MenuCamera);
                    GameUI.Instance.EndGameUI.Show();
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

    public PlayerCharacterController GetPlayerGameObject(ePlayer player)
    {
        return null;
    }

    private void OnPlayerJoined(PlayerCharacterController player)
    {
        if (PlayerManager.Instance.Players.Count == 1)
        {
            player.CameraStack.Camera = CameraManager.Instance.LeftPlayerCamera;
        }
        else if (PlayerManager.Instance.Players.Count == 2)
        {
            player.CameraStack.Camera = CameraManager.Instance.RightPlayerCamera;
        }
    }

    private void SpawnPlayers()
    {
        PlayerManager.Instance.enabled = true;
    }

    private void DespawnPlayers()
    {
        PlayerManager.Instance.enabled = false;
        PlayerManager.Instance.DespawnPlayers();
    }
}
