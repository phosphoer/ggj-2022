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
  public float ObstacleRaycastRadius = 0.5f;
  public float ObstacleMinDist = 1.0f;
  public LayerMask ObstacleLayer = default(LayerMask);

  public override void CameraStart()
  {
    MountPoint.position = TargetPlayer.transform.TransformPoint(CameraLocalOffset);
    MountPoint.rotation = Quaternion.LookRotation(TargetPlayer.transform.position - MountPoint.position, TargetPlayer.transform.up);
  }

  public override void CameraStop()
  {

  }

  public override void CameraUpdate()
  {
    if (TargetPlayer != null)
    {
      Vector3 playerPos = TargetPlayer.transform.position;
      Vector3 desiredCameraPos = TargetPlayer.transform.TransformPoint(CameraLocalOffset);

      Vector3 cameraLook = TargetPlayer.transform.TransformPoint(CameraLookLocalOffset);
      cameraLook += TargetPlayer.transform.forward * TargetPlayer.Character.DesiredSpeed * MoveLookAheadScale;
      Quaternion desiredCameraRot = Quaternion.LookRotation(cameraLook - desiredCameraPos, TargetPlayer.transform.up);

      Vector3 cameraPos = MountPoint.position;
      Quaternion cameraRot = MountPoint.rotation;
      cameraPos = Mathfx.Damp(cameraPos, desiredCameraPos, 0.25f, Time.deltaTime * PositionInterpolationSpeed);
      cameraRot = Mathfx.Damp(cameraRot, desiredCameraRot, 0.25f, Time.deltaTime * RotationInterpolationSpeed);

      // Collide with obstacles
      Vector3 moveVec = cameraPos - MountPoint.position;
      RaycastHit hitInfo;
      if (Physics.SphereCast(transform.position, ObstacleRaycastRadius, moveVec.normalized, out hitInfo, ObstacleMinDist, ObstacleLayer))
      {
        // Find the plane representing the point + normal we hit
        Plane hitPlane = new Plane(hitInfo.normal, hitInfo.point);

        // Now project our position onto that plane and use the vector from
        // the projected point to our pos as the adjusted normal
        Vector3 closestPoint = hitPlane.ClosestPointOnPlane(cameraPos);
        Vector3 closestPointToPos = cameraPos - closestPoint;

        // "Clamp" our distance from the plane to a min distance
        float planeDist = closestPointToPos.magnitude;
        float adjustedDist = Mathf.Max(planeDist, ObstacleMinDist);
        cameraPos = closestPoint + closestPointToPos.normalized * adjustedDist;
      }

      MountPoint.position = cameraPos;
      MountPoint.rotation = cameraRot;
    }
  }
}