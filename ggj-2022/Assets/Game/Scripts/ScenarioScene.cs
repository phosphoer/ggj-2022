using UnityEngine;

public class ScenarioScene : MonoBehaviour
{
  public enum ScenarioState
  {
    Invalid,
    InGame,
    GoodEnd,
    BadEnd
  }

  public ScenarioState CurrentState
  {
    get => _state;
    set
    {
      _state = value;
      EnsureState();
    }
  }

  public CameraControllerBase CurrentCamera
  {
    get
    {
      if (_state == ScenarioState.InGame)
        return _inGameCamera;
      else if (_state == ScenarioState.GoodEnd)
        return _goodEndCamera;
      else if (_state == ScenarioState.BadEnd)
        return _badEndCamera;

      return null;
    }
  }

  [SerializeField]
  private CameraControllerBase _inGameCamera = null;

  [SerializeField]
  private CameraControllerBase _goodEndCamera = null;

  [SerializeField]
  private CameraControllerBase _badEndCamera = null;

  [SerializeField]
  private GameObject _inGameSceneRoot = null;

  [SerializeField]
  private GameObject _goodEndSceneRoot = null;

  [SerializeField]
  private GameObject _badEndSceneRoot = null;

  private ScenarioState _state = default(ScenarioState);

  public void SetWinner(ePlayer winningPlayer)
  {
    if (winningPlayer == ePlayer.DevilPlayer)
    {
      CurrentState = ScenarioState.BadEnd;
    }
    else if (winningPlayer == ePlayer.AngelPlayer)
    {
      CurrentState = ScenarioState.GoodEnd;
    }
  }

  private void EnsureState()
  {
    if (_inGameSceneRoot != null)
      _inGameSceneRoot.SetActive(_state == ScenarioState.InGame);

    if (_goodEndSceneRoot != null)
      _goodEndSceneRoot.SetActive(_state == ScenarioState.GoodEnd);

    if (_badEndSceneRoot != null)
      _badEndSceneRoot.SetActive(_state == ScenarioState.BadEnd);
  }
}