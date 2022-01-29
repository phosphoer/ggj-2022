using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerPlayer : CameraControllerBase
{
    public ePlayer AssignedPlayer = ePlayer.LeftPlayer;

    public Vector3 CameraWorldOffset = new Vector3(5, 10, -5);
    public Vector3 CameraLookOffset = Vector3.zero;

    public override void CameraStart()
    {
        
    }

    public override void CameraStop()
    {
        
    }

    public override void CameraUpdate()
    {
        var playerController = GameStateManager.Instance.GetPlayerGameObject(AssignedPlayer);

        if (playerController != null)
        {
            Vector3 playerPos = playerController.transform.position;
            Vector3 cameraPos = playerPos + CameraWorldOffset;
            Vector3 cameraLook = playerPos + CameraLookOffset;

            transform.position = cameraPos;
            transform.rotation = Quaternion.LookRotation(cameraLook - cameraPos);
        }
    }
}