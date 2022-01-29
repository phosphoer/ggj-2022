using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayer
{
    Invalid,
    LeftPlayer,
    RightPlayer
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

    private GameObject _leftPlayerObject = null;
    private GameObject _rightPlayerObject = null;

    public GameObject PlayerPrefab;

    public static event System.Action GameStateChangeEvent;

    private GameStage _gameStage = GameStage.Invalid;
    public GameStage CurrentStage => _gameStage;
    public GameStage EditorDefaultStage = GameStage.ScenarioIntro;

    public SoundBank MusicMenuLoop;
    public CameraControllerBase MenuCamera;

    private void Awake()
    {
        Instance = this;
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
                //TODO: Wait for the intro timer to
                nextGameStage = GameStage.ScenarioGameplay;
                break;
            case GameStage.ScenarioGameplay:
                break;
            case GameStage.ScenarioOutro:
                nextGameStage = GameStage.ScenarioIntro;
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

                    //GameUI.Instance.MainMenuUI.Hide();
                }
                break;
            case GameStage.Settings:
                {
                    CameraManager.Instance.MenuCameraStack.PopCurrentController();

                    //GameUI.Instance.SettingsUI.Hide();
                }
                break;
            case GameStage.ScenarioIntro:
                {
                    CameraManager.Instance.ScenarioCameraStack.PopCurrentController();

                    //GameUI.Instance.DayIntroUI.Hide();
                }
                break;
            case GameStage.ScenarioGameplay:
                {
                    CameraManager.Instance.ScenarioCameraStack.PopCurrentController();
                }
                break;
            case GameStage.ScenarioOutro:
                {
                    DespawnPlayers();
                    CameraManager.Instance.ScenarioCameraStack.PopCurrentController();
                }
                break;
            case GameStage.EndGame:
                {
                    CameraManager.Instance.ScenarioCameraStack.PopCurrentController();

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
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.SingleCamera);

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
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.SingleCamera);

                    //GameUI.Instance.SettingsUI.Show();
                    //CameraControllerStack.Instance.PushController(MenuCamera);
                }
                break;
            case GameStage.ScenarioIntro:
                {
                    SpawnPlayers();
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MultiCamera);
                    //GameUI.Instance.GameplayUI.Show();
                }
                break;
            case GameStage.ScenarioGameplay:
                {
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MultiCamera);
                    //GameUI.Instance.GameplayUI.Show();
                }
                break;
            case GameStage.ScenarioOutro:
                {
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MultiCamera);
                    //GameUI.Instance.GameplayUI.Show();
                }
                break;
            case GameStage.EndGame:
                {
                    CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.SingleCamera);
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

    public GameObject GetPlayerGameObject(ePlayer player)
    {
        switch (player)
        {
            case ePlayer.LeftPlayer:
                return _leftPlayerObject;
            case ePlayer.RightPlayer:
                return _rightPlayerObject;
        }

        return null;
    }

    private void SpawnPlayers()
    {
        // TODO: Get spawn point for the left player
        Vector2 leftSpawnLoc = Random.insideUnitCircle;

        // Spawn the left player object
        _leftPlayerObject= Instantiate(PlayerPrefab, new Vector3(leftSpawnLoc.x, 0, leftSpawnLoc.y), Quaternion.identity);

        // Push the left player camera controller on the camera control stack
        CameraManager.Instance.LeftPlayerCameraStack.PushController(CameraManager.Instance.LeftPlayerCameraController);

        //TODO: Get spawn point for the right player
        Vector2 rightSpawnLoc = Random.insideUnitCircle;

        // Spawn the right player
        _rightPlayerObject = Instantiate(PlayerPrefab, new Vector3(rightSpawnLoc.x, 0, rightSpawnLoc.y), Quaternion.identity);

        // Push the right player camera controller on the camera control stack
        CameraManager.Instance.RightPlayerCameraStack.PushController(CameraManager.Instance.RightPlayerCameraController);
    }

    private void DespawnPlayers()
    {
        CameraManager.Instance.LeftPlayerCameraStack.PopCurrentController();
        Destroy(_leftPlayerObject);
        _leftPlayerObject = null;

        CameraManager.Instance.RightPlayerCameraStack.PopCurrentController();
        Destroy(_rightPlayerObject);
        _rightPlayerObject = null;
    }
}
