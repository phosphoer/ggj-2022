using UnityEngine;
using Rewired;

public class PlayerCharacterController : MonoBehaviour
{
  public int RewiredPlayerId = 0;
  public ePlayer Team = default(ePlayer);
  public GameCharacterController Character = null;
  public CameraControllerStack CameraStack = null;
  public CameraControllerPlayer PlayerCameraPrefab = null;

  private CameraControllerPlayer _playerCamera;

  private void Update()
  {
    if (CameraStack.Camera != null && _playerCamera == null)
    {
      _playerCamera = Instantiate(PlayerCameraPrefab);
      _playerCamera.TargetPlayer = this;

      CameraStack.PushController(_playerCamera);
    }

    var rewiredPlayer = ReInput.players.GetPlayer(RewiredPlayerId);
    if (rewiredPlayer != null)
    {
      Character.DesiredSpeed = rewiredPlayer.GetAxis(RewiredConsts.Action.Move);
      Character.DesiredTurn = rewiredPlayer.GetAxis(RewiredConsts.Action.Turn);

      if (rewiredPlayer.GetButtonDoublePressDown(RewiredConsts.Action.Slap))
      {
        Character.DoubleSlap();
      }
      else if (rewiredPlayer.GetButtonSinglePressDown(RewiredConsts.Action.Slap))
      {
        Character.FastSlap();
      }
    }
  }
}