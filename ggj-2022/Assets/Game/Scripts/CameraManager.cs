using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
  public enum eScreenLayout
  {
    Invalid,
    MenuCamera,
    ScenarioCamera,
    MultiCamera
  }

  public CameraControllerStack ScenarioCameraStack => _scenarioCameraStack;
  public CameraControllerStack MenuCameraStack => _menuCameraStack;

  public Camera ScenarioCamera => _scenarioCamera;
  public Camera LeftPlayerCamera => _leftPlayerCamera;
  public Camera RightPlayerCamera => _rightPlayerCamera;

  public Camera getPlayerCamera(ePlayer player)
  {
    switch (player)
    {
      case ePlayer.DevilPlayer:
        return _leftPlayerCamera;
      case ePlayer.AngelPlayer:
        return _rightPlayerCamera;
    }

    return null;
  }

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

  private eScreenLayout _cameraLayout = eScreenLayout.Invalid;
  public eScreenLayout CameraLayout => _cameraLayout;

  private void Awake()
  {
    Instance = this;
  }

  private void SetCameraSize(Camera camera, float newWidth, float newHeight)
  {
    float newX = 1.0f - newWidth;
    float newY = 1.0f - newHeight;

    camera.rect = new Rect(newX, newY, newWidth, newHeight);
  }

  IEnumerator SqueezeCameraScreenRoutine(Camera camera, float fromWidth, float toWidth, float fromHeight, float toHeight, float duration)
  {
    for (float time = 0; time < duration; time += Time.unscaledDeltaTime)
    {
      yield return null;

      float t = time / duration;
      float width = Mathf.Lerp(fromWidth, toWidth, t);
      float height = Mathf.Lerp(fromWidth, toHeight, t);

      SetCameraSize(camera, width, height);
    }
  }

  public Coroutine SqueezeCameraScreen(Camera camera, float fromWidth, float toWidth, float fromHeight, float toHeight, float duration)
  {
    return StartCoroutine(SqueezeCameraScreenRoutine(camera, fromWidth, toWidth, fromHeight, toHeight, duration));
  }

  public void SetScreenLayout(eScreenLayout targetLayout)
  {
    if (targetLayout != _cameraLayout)
    {
      switch (targetLayout)
      {
        case eScreenLayout.MenuCamera:
          _scenarioCamera.enabled = false;
          _leftPlayerCamera.enabled = false;
          _rightPlayerCamera.enabled = false;
          _menuCamera.enabled = true;
          break;
        case eScreenLayout.ScenarioCamera:
          _scenarioCamera.enabled = true;
          SetCameraSize(_scenarioCamera, 1.0f, 1.0f);
          _leftPlayerCamera.enabled = false;
          _rightPlayerCamera.enabled = false;
          _menuCamera.enabled = false;
          break;
        case eScreenLayout.MultiCamera:
          _scenarioCamera.enabled = true;
          if (_cameraLayout == eScreenLayout.ScenarioCamera)
          {
            SqueezeCameraScreen(_scenarioCamera, 1.0f, 0.85f, 1.0f, 0.5f, 0.5f);
          }
          else
          {
            SetCameraSize(_scenarioCamera, 0.85f, 0.5f);
          }
          _leftPlayerCamera.enabled = true;
          _rightPlayerCamera.enabled = true;
          _menuCamera.enabled = false;
          break;
      }

      _cameraLayout = targetLayout;
    }
  }
}
