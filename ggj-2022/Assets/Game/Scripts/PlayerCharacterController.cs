using UnityEngine;
using Rewired;

public class PlayerCharacterController : MonoBehaviour
{
  public Rewired.Player RewiredPlayer;
  public GameCharacterController Character = null;
  public SlapController SlapController = null;
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

    if (RewiredPlayer != null)
    {
      Character.DesiredSpeed = RewiredPlayer.GetAxis(RewiredConsts.Action.Move);
      Character.DesiredTurn = RewiredPlayer.GetAxis(RewiredConsts.Action.Turn);

      if (RewiredPlayer.GetButtonDown(RewiredConsts.Action.Slap))
      {
        SlapController.Slap();
      }
    }
  }
}