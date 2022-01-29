using UnityEngine;

public class RobotAnimation : MonoBehaviour
{
  public event System.Action SlapAnimEvent;

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
  }
}