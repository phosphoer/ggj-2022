using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlattenHierarchy : MonoBehaviour
{
#if UNITY_EDITOR
  [MenuItem("GameObject/Flatten Selected Hierarchy")]
  public static void FlattenSelected()
  {
    if (Selection.activeGameObject != null)
      Flatten(Selection.activeGameObject.transform);
  }
#endif

  public static void Flatten(Transform t, IReadOnlyList<Transform> excludeList = null)
  {
    var ts = t.GetComponentsInChildren<Transform>();
    for (int i = 0; i < ts.Length; ++i)
    {
      if (excludeList == null || !excludeList.Contains(ts[i]))
        ts[i].SetParent(t);
    }
  }

  [SerializeField]
  private Transform[] _exclude = null;

  private void Start()
  {
    Flatten(transform, _exclude);
  }
}