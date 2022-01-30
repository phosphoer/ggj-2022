using UnityEngine;
using System.Collections;

public class SlappableStation : MonoBehaviour, ISlappable
{
  [SerializeField]
  private AnimationCurve _slapScaleCurve = null;

  [SerializeField]
  private SoundBank _meatSlapSounds = null;

  void ISlappable.ReceiveSlap(Vector3 slapOrigin, Vector3 slapDirection, float slapStrength)
  {
    StartCoroutine(SlapAnimAsync(slapDirection, slapStrength));

    AudioManager.Instance?.PlaySound(_meatSlapSounds);
  }

  private IEnumerator SlapAnimAsync(Vector3 slapDirection, float slapStrength)
  {
    yield return Tween.CustomTween(1.0f, t =>
    {
      transform.localScale = Vector3.one + (slapDirection * slapStrength) * _slapScaleCurve.Evaluate(t);
    });
  }
}