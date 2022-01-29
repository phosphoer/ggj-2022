using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerPlayer : CameraControllerBase
{
  public PlayerCharacterController TargetPlayer = null;

  public Vector3 CameraLocalOffset = new Vector3(3, 5, -5);
  public Vector3 CameraLookLocalOffset = Vector3.zero;
  public float MoveLookAheadScale = 1.0f;
  public float PositionInterpolationSpeed = 1.0f;
  public float RotationInterpolationSpeed = 1.0f;

  public override void CameraStart()
  {

  }

  public override void CameraStop()
  {

  }

  public override void CameraUpdate()
  {
    if (TargetPlayer != null)
    {
      Vector3 playerPos = TargetPlayer.transform.position;
      Vector3 cameraPos = TargetPlayer.transform.TransformPoint(CameraLocalOffset);
      Vector3 cameraLook = TargetPlayer.transform.TransformPoint(CameraLookLocalOffset);
      cameraLook += TargetPlayer.transform.forward * TargetPlayer.Character.DesiredSpeed * MoveLookAheadScale;
      Quaternion cameraRot = Quaternion.LookRotation(cameraLook - cameraPos, TargetPlayer.transform.up);

      transform.position = Mathfx.Damp(transform.position, cameraPos, 0.25f, Time.deltaTime * PositionInterpolationSpeed);
      transform.rotation = Mathfx.Damp(transform.rotation, cameraRot, 0.25f, Time.deltaTime * RotationInterpolationSpeed);
    }
  }
}