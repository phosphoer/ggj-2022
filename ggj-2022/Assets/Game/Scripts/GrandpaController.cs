using UnityEngine;
using System.Collections;

public class GrandpaController : MonoBehaviour
{
  [SerializeField]
  private BonePair[] _bonePairs = null;

  [SerializeField]
  private AnimationCurve _partSlapScaleCurve = null;

  [SerializeField]
  private float _boneAnimationScalar = 1.0f;

  [System.Serializable]
  private class BonePair
  {
    public eBodyPart BodyPart = default(eBodyPart);
    public Transform[] Bones = null;
  }

  private void OnEnable()
  {
    ScenarioManager.PartSlapped += OnPartSlapped;
  }

  private void OnDisable()
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
      foreach (var boneTransform in bone.Bones)
      {
        float scaleFactor = slapStrength * _boneAnimationScalar;
        Vector3 extraScale = (direction * scaleFactor) * _partSlapScaleCurve.Evaluate(t);
        boneTransform.localScale = Vector3.one + extraScale;
      }
    });

    foreach (var boneTransform in bone.Bones)
      boneTransform.localScale = Vector3.one;
  }
}