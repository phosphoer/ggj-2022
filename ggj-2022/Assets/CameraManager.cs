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

    private CameraControllerPlayer _leftPlayerCameraController;
    public CameraControllerPlayer LeftPlayerCameraController => _leftPlayerCameraController;

    private CameraControllerPlayer _rightPlayerCameraController;
    public CameraControllerPlayer RightPlayerCameraController => _rightPlayerCameraController;

    private Camera _scenarioCamera;
    private Camera _leftPlayerCamera;
    private Camera _rightPlayerCamera;
    private Camera _menuCamera;

    private eScreenLayout _cameraLayout = eScreenLayout.SingleCamera;
    public eScreenLayout CameraLayout => _cameraLayout;

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

        _leftPlayerCameraController = leftPlayerCameraObject.GetComponent<CameraControllerPlayer>();
        _rightPlayerCameraController = rightPlayerCameraObject.GetComponent<CameraControllerPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScreenLayout(eScreenLayout targetLayout)
    {
        switch (targetLayout)
        {
            case eScreenLayout.SingleCamera:
                _scenarioCamera.enabled= false;
                _leftPlayerCamera.enabled= false;
                _rightPlayerCamera.enabled= false;
                _menuCamera.enabled= true;
                break;
            case eScreenLayout.MultiCamera:
                _scenarioCamera.enabled= true;
                _leftPlayerCamera.enabled= true;
                _rightPlayerCamera.enabled= true;
                _menuCamera.enabled= false;
                break;
        }

        _cameraLayout = targetLayout;
    }
}
