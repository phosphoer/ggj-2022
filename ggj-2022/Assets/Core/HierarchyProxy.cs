using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class HierarchyProxy : MonoBehaviour
{
  public Transform Target;
  public List<TransformPair> TargetTransforms;

  [System.Serializable]
  public struct TransformPair
  {
    public Transform Local;
    public Transform Target;
    public bool UseLocalTransform;
  }

  private Transform[] _myTransforms;

  [ContextMenu("Map to target")]
  private void CreateMapping()
  {
    TargetTransforms = new List<TransformPair>();

    _myTransforms = GetComponentsInChildren<Transform>();
    Transform[] ts = Target.GetComponentsInChildren<Transform>();

    for (int i = 0; i < _myTransforms.Length; ++i)
    {
      Transform localChild = _myTransforms[i];
      string name = localChild.name;
      for (int j = 0; j < ts.Length; ++j)
      {
        if (ts[j].name == name)
        {
          bool useLocal = localChild.parent != transform;
          TargetTransforms.Add(new TransformPair() { Local = localChild, Target = ts[j], UseLocalTransform = useLocal });
          break;
        }
      }
    }
  }

  [ContextMenu("Apply Target to Local")]
  private void ApplyTargetTransformToLocal()
  {
    for (int i = 0; i < TargetTransforms.Count; ++i)
    {
      var pair = TargetTransforms[i];
      pair.Local.SetPositionAndRotation(pair.Target.position, pair.Target.rotation);
    }
  }

  private void Start()
  {
    if (Application.isPlaying)
    {
      if (TargetTransforms == null || TargetTransforms.Count == 0)
        CreateMapping();
    }
  }

  private void LateUpdate()
  {
    if (TargetTransforms != null)
    {
      for (int i = 0; i < TargetTransforms.Count; ++i)
      {
        TransformPair pair = TargetTransforms[i];
        if (pair.Target != null && pair.Local != null)
        {
          if (pair.UseLocalTransform)
          {
            pair.Target.localPosition = pair.Local.localPosition;
            pair.Target.localRotation = pair.Local.localRotation;
          }
          else
          {
            pair.Target.position = pair.Local.position;
            pair.Target.rotation = pair.Local.rotation;
          }
          pair.Target.localScale = pair.Local.localScale;
        }
      }
    }
  }
}