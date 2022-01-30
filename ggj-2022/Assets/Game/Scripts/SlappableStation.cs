using UnityEngine;
using System.Collections;

public class SlappableStation : MonoBehaviour, ISlappable
{
  [SerializeField]
  private AnimationCurve _slapScaleCurve = null;

  void ISlappable.ReceiveSlap(Vector3 slapOrigin, Vector3 slapDirection, float slapStrength)
  {
    StartCoroutine(SlapAnimAsync(slapDirection, slapStrength));
  }

  private IEnumerator SlapAnimAsync(Vector3 slapDirection, float slapStrength)
  {
    yield return Tween.CustomTween(2.0f, t =>
    {
      transform.localScale = (Vector3.one * 0.5f + slapDirection * slapStrength * 0.5f) * _slapScaleCurve.Evaluate(t);
    });
  }
}