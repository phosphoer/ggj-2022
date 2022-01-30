using UnityEngine;

public class RobotAnimation : MonoBehaviour
{
  public event System.Action SlapAnimEvent;
  public event System.Action SlapAnimCompleteEvent;

  public float TrundleAmount
  {
    get => _trundleAmount;
    set
    {
      if (value != _trundleAmount)
      {
        _trundleAmount = value;
        _animator.SetFloat(kAnimTrundleAmount, value);
      }
    }
  }

  public bool IsStunned
  {
    get => _isStunned;
    set
    {
      if (value != _isStunned)
      {
        _isStunned = value;
        _animator.SetBool(kAnimIsStunned, value);
      }
    }
  }

  [SerializeField]
  private Animator _animator = null;

  [SerializeField]
  private AnimatorCallbacks _animatorCallbacks = null;

  [SerializeField]
  private SoundBank _idleClinkSound1 = null;

  [SerializeField]
  private SoundBank _idleClinkSound2 = null;

  [SerializeField]
  private SoundBank _idleClinkSound3 = null;

  private float _trundleAmount;
  private bool _isStunned;

  private static readonly int kAnimIsStunned = Animator.StringToHash("IsStunned");
  private static readonly int kAnimTrundleAmount = Animator.StringToHash("TrundleAmount");
  private static readonly int kAnimFastSlap = Animator.StringToHash("FastSlap");
  private static readonly int kAnimDoubleSlap = Animator.StringToHash("DoubleSlap");

  public void FastSlap()
  {
    _animator.SetTrigger(kAnimFastSlap);
  }

  public void DoubleSlap()
  {
    _animator.SetTrigger(kAnimDoubleSlap);
  }

  private void Awake()
  {
    _animatorCallbacks.AddCallback("Interact", () =>
    {
      SlapAnimEvent?.Invoke();
    });

    _animatorCallbacks.AddCallback("InteractComplete", () =>
    {
      SlapAnimCompleteEvent?.Invoke();
    });

    _animatorCallbacks.AddCallback("Clink1", () =>
    {
      AudioManager.Instance?.PlaySound(_idleClinkSound1);
    });
    _animatorCallbacks.AddCallback("Clink2", () =>
    {
      AudioManager.Instance?.PlaySound(_idleClinkSound2);
    });
    _animatorCallbacks.AddCallback("Clink3", () =>
    {
      AudioManager.Instance?.PlaySound(_idleClinkSound3);
    });
  }
}