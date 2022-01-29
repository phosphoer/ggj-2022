using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
  public enum eScreenLayout
  {
    SingleCamera,
    MultiCamera
  }

  public CameraControllerStack ScenarioCameraStack => _scenarioCameraStack;
  public CameraControllerStack MenuCameraStack => _menuCameraStack;
  public Camera LeftPlayerCamera => _leftPlayerCamera;
  public Camera RightPlayerCamera => _rightPlayerCamera;

  [SerializeField]
  private CameraControllerStack _scenarioCameraStack = null;

  [SerializeField]
  private CameraControllerStack _menuCameraStack = null;

  [SerializeField]
  private Camera _scenarioCamera = null;

  [SerializeField]
  private Camera _menuCamera = null;

  [SerializeField]
  private Camera _leftPlayerCamera = null;

  [SerializeField]
  private Camera _rightPlayerCamera = null;

  private eScreenLayout _cameraLayout = eScreenLayout.SingleCamera;
  public eScreenLayout CameraLayout => _cameraLayout;

  private void Awake()
  {
    Instance = this;
  }

  public void SetScreenLayout(eScreenLayout targetLayout)
  {
    switch (targetLayout)
    {
      case eScreenLayout.SingleCamera:
        _scenarioCamera.enabled = false;
        _leftPlayerCamera.enabled = false;
        _rightPlayerCamera.enabled = false;
        _menuCamera.enabled = true;
        break;
      case eScreenLayout.MultiCamera:
        _scenarioCamera.enabled = true;
        _leftPlayerCamera.enabled = true;
        _rightPlayerCamera.enabled = true;
        _menuCamera.enabled = false;
        break;
    }

    _cameraLayout = targetLayout;
  }
}
