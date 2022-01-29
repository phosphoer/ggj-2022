using UnityEngine;

public class RobotAnimation : MonoBehaviour
{
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

  private bool _isTrundling;

  private static readonly int kAnimIsTrundling = Animator.StringToHash("IsTrundling");
}