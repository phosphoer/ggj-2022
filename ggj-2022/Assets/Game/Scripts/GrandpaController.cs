using UnityEngine;
using System.Collections;

public class GrandpaController : MonoBehaviour
{
  [SerializeField]
  private BonePair[] _bonePairs = null;

  [SerializeField]
  private AnimationCurve _partSlapScaleCurve = null;

  [System.Serializable]
  private class BonePair
  {
    public eBodyPart BodyPart = default(eBodyPart);
    public Transform Bone = null;
  }

  private void Start()
  {
    ScenarioManager.PartSlapped += OnPartSlapped;
  }

  private void OnDestroy()
  {
    ScenarioManager.PartSlapped -= OnPartSlapped;
  }

  private void OnPartSlapped(eBodyPart part, float slapStrength)
  {
    var bonePair = GetBonePair(part);
    if (bonePair != null)
    {
      StartCoroutine(SlapAnimAsync(bonePair, slapStrength));
    }
    else
    {
      Debug.LogWarning($"No bone pair for {part}");
    }
  }

  private BonePair GetBonePair(eBodyPart part)
  {
    foreach (var pair in _bonePairs)
    {
      if (pair.BodyPart == part)
      {
        return pair;
      }
    }

    return null;
  }

  private IEnumerator SlapAnimAsync(BonePair bone, float slapStrength)
  {
    Vector3 direction = Random.onUnitSphere;
    yield return Tween.CustomTween(1.0f, t =>
    {
      bone.Bone.localScale = Vector3.one + (direction * slapStrength) * _partSlapScaleCurve.Evaluate(t);
    });
  }
}