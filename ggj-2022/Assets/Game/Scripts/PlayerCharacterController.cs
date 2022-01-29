using UnityEngine;
using Rewired;

public class PlayerCharacterController : MonoBehaviour
{
  public int RewiredPlayerId = 0;
  public GameCharacterController Character = null;
  public SlapController SlapController = null;

  private void Update()
  {
    var player = ReInput.players.GetPlayer(RewiredPlayerId);
    Character.DesiredSpeed = player.GetAxis(RewiredConsts.Action.Move);
    Character.DesiredTurn = player.GetAxis(RewiredConsts.Action.Turn);

    if (player.GetButtonDown(RewiredConsts.Action.Slap))
    {
      SlapController.Slap();
    }
  }
}