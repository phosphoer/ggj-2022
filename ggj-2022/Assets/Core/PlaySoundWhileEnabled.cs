using UnityEngine;
using System.Collections;

public class PlaySoundWhileEnabled : MonoBehaviour
{
  public SoundBank SoundBank;
  public float FadeTime = 0;

  private bool _queued;
  private GameObject _audioChild;

  private void OnEnable()
  {
    if (!_queued)
    {
      _queued = true;

      if (FadeTime > 0)
      {
        if (_audioChild == null)
        {
          _audioChild = new GameObject($"{name}-playsoundwhileenabled");
        }

        StartCoroutine(AudioManager.QueueFadeInSoundRoutine(_audioChild, SoundBank, 1, FadeTime));
        StartCoroutine(UpdateAudioChildAsync());
      }
      else
      {
        StartCoroutine(AudioManager.QueuePlaySoundRoutine(gameObject, SoundBank));
      }
    }
  }

  private void OnDisable()
  {
    _queued = false;

    if (AudioManager.Instance != null)
    {
      if (FadeTime > 0)
      {
        AudioManager.Instance.StartCoroutine(FadeOutAsync());
      }
      else
        AudioManager.Instance.StopSound(gameObject, SoundBank);
    }
  }

  private IEnumerator FadeOutAsync()
  {
    yield return AudioManager.Instance.FadeOutSound(_audioChild, SoundBank, FadeTime);
    Destroy(_audioChild);
  }

  private IEnumerator UpdateAudioChildAsync()
  {
    while (enabled)
    {
      _audioChild.transform.SetPositionAndRotation(transform.position, transform.rotation);
      yield return null;
    }
  }
}