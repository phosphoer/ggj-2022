using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MainCamera : Singleton<MainCamera>
{
  public static event System.Action Initialized;
  public static event System.Action Uninitialized;

  public CameraControllerStack CameraStack => _cameraControllerStack;
  public Camera Camera => _camera;
  public PostProcessLayer PostProcessLayer => _postFxLayer;
  public Transform CachedTransform => _cachedTransform;

  [SerializeField]
  private CameraControllerStack _cameraControllerStack = null;

  [SerializeField]
  private Camera _camera = null;

  [SerializeField]
  private PostProcessLayer _postFxLayer = null;

  private Transform _cachedTransform = null;

#if UNITY_EDITOR
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
  private static void EditorStaticReset()
  {
    Instance = null;
  }
#endif


  private void Awake()
  {
    Instance = this;
    _cachedTransform = transform;
    Initialized?.Invoke();
  }

  private void OnDestroy()
  {
    _cachedTransform = null;
    Uninitialized?.Invoke();
  }
}