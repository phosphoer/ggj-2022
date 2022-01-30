using UnityEngine;
using System.Collections;

public class SlappableStation : MonoBehaviour, ISlappable
{
  [SerializeField]
  private AnimationCurve _slapScaleCurve = null;

  [SerializeField]
  private eBodyPart _bodyPart = default(eBodyPart);

  [SerializeField]
  private SoundBank _meatSlapSounds = null;

  void ISlappable.ReceiveSlap(ePlayer fromTeam, Vector3 slapOrigin, Vector3 slapDirection, float slapStrength)
  {
    StartCoroutine(SlapAnimAsync(slapDirection, slapStrength));

    AudioManager.Instance?.PlaySound(_meatSlapSounds, slapStrength / 3.0f);

    ScenarioManager.Instance?.ApplySlapDamage(fromTeam, _bodyPart, slapStrength);
  }

  private IEnumerator SlapAnimAsync(Vector3 slapDirection, float slapStrength)
  {
    yield return Tween.CustomTween(1.0f, t =>
    {
      transform.localScale = Vector3.one + (slapDirection * slapStrength) * _slapScaleCurve.Evaluate(t);
    });
  }
}