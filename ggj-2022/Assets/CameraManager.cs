using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public enum ScreenLayout
    {
        SingleCamera,
        MultiCamera
    }

    public GameObject scenarioCameraObject;
    public GameObject leftPlayerCameraObject;
    public GameObject rightPlayerCameraObject;
    public GameObject menuPlayerCameraObject;

    private CameraControllerStack _scenarioCameraStack;
    public CameraControllerStack ScenarioCameraStack => _scenarioCameraStack;

    private CameraControllerStack _leftPlayerCameraStack;
    public CameraControllerStack LeftPlayerCameraStack => _leftPlayerCameraStack;

    private CameraControllerStack _rightPlayerCameraStack;
    public CameraControllerStack RightPlayerCameraStack => _rightPlayerCameraStack;

    private CameraControllerStack _menuCameraStack;
    public CameraControllerStack MenuCameraStack => _menuCameraStack;

    private Camera _scenarioCamera;
    private Camera _leftPlayerCamera;
    private Camera _rightPlayerCamera;
    private Camera _menuCamera;

    private ScreenLayout _cameraLayout = ScreenLayout.SingleCamera;
    public ScreenLayout CameraLayout => _cameraLayout;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _scenarioCameraStack = scenarioCameraObject.GetComponent<CameraControllerStack>();
        _leftPlayerCameraStack = leftPlayerCameraObject.GetComponent<CameraControllerStack>();
        _rightPlayerCameraStack = rightPlayerCameraObject.GetComponent<CameraControllerStack>();
        _menuCameraStack = menuPlayerCameraObject.GetComponent<CameraControllerStack>();

        _scenarioCamera = scenarioCameraObject.GetComponent<Camera>();
        _leftPlayerCamera = leftPlayerCameraObject.GetComponent<Camera>();
        _rightPlayerCamera = rightPlayerCameraObject.GetComponent<Camera>();
        _menuCamera = menuPlayerCameraObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScreenLayout(ScreenLayout targetLayout)
    {
        switch (targetLayout)
        {
            case ScreenLayout.SingleCamera:
                _scenarioCamera.enabled= false;
                _leftPlayerCamera.enabled= false;
                _rightPlayerCamera.enabled= false;
                _menuCamera.enabled= true;
                break;
            case ScreenLayout.MultiCamera:
                _scenarioCamera.enabled= true;
                _leftPlayerCamera.enabled= true;
                _rightPlayerCamera.enabled= true;
                _menuCamera.enabled= false;
                break;
        }

        _cameraLayout = targetLayout;
    }
}
