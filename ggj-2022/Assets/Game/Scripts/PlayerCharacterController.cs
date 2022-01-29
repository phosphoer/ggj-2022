using UnityEngine;
using Rewired;

public class PlayerCharacterController : MonoBehaviour
{
  public int RewiredPlayerId = 0;
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

      if (rewiredPlayer.GetButtonDown(RewiredConsts.Action.Slap))
      {
        Character.Slap();
      }
    }
  }
}