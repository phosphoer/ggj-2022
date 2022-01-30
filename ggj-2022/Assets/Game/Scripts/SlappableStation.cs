using UnityEngine;
using System.Collections;

public class SlappableStation : MonoBehaviour, ISlappable
{
  [SerializeField]
  private AnimationCurve _slapScaleCurve = null;

  [SerializeField]
  private eBodyPart _bodyPart = default(eBodyPart);

  void ISlappable.ReceiveSlap(Vector3 slapOrigin, Vector3 slapDirection, float slapStrength)
  {
    StartCoroutine(SlapAnimAsync(slapDirection, slapStrength));

  }

  private IEnumerator SlapAnimAsync(Vector3 slapDirection, float slapStrength)
  {
    yield return Tween.CustomTween(1.0f, t =>
    {
      transform.localScale = Vector3.one + (slapDirection * slapStrength) * _slapScaleCurve.Evaluate(t);
    });
  }
}