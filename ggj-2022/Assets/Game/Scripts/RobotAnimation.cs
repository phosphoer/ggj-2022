using UnityEngine;

public class RobotAnimation : MonoBehaviour
{
  public event System.Action SlapAnimEvent;

  public bool IsTrundling
  {
    get => _isTrundling;
    set
    {
      if (value != _isTrundling)
      {
        _isTrundling = value;
        _animator.SetBool(kAnimIsTrundling, value);
      }
    }
  }

  [SerializeField]
  private Animator _animator = null;

  [SerializeField]
  private AnimatorCallbacks _animatorCallbacks = null;

  private bool _isTrundling;

  private static readonly int kAnimIsTrundling = Animator.StringToHash("IsTrundling");
  private static readonly int kAnimSlap = Animator.StringToHash("Slap");

  public void Slap()
  {
    _animator.SetTrigger(kAnimSlap);
  }

  private void Awake()
  {
    _animatorCallbacks.AddCallback("Interact", () =>
    {
      SlapAnimEvent?.Invoke();
    });
  }
}