using UnityEngine;
using System.Collections.Generic;

public class SlapController : MonoBehaviour
{
  [SerializeField]
  private LayerMask _slapMask = default(LayerMask);

  [SerializeField]
  private float _slapRadius = 0.5f;

  [SerializeField]
  private Transform _slapOrigin = null;

  private Collider[] _slapColliders = new Collider[10];
  private List<ISlappable> _slappedThisFrame = new List<ISlappable>();

  public void Slap(float slapStrength = 1f)
  {
    int numColliders = Physics.OverlapSphereNonAlloc(_slapOrigin.position, _slapRadius, _slapColliders, _slapMask, QueryTriggerInteraction.Collide);
    Debug.Log($"Slap hit {numColliders} colliders");

    _slappedThisFrame.Clear();
    for (int i = 0; i < numColliders; i++)
    {
      var slappable = _slapColliders[i].GetComponentInParent<ISlappable>();
      if (slappable != null && !_slapColliders[i].transform.IsChildOf(transform) && !_slappedThisFrame.Contains(slappable))
      {
        Debug.Log($"Slapping {_slapColliders[i].name}");
        _slappedThisFrame.Add(slappable);
        slappable.ReceiveSlap(_slapOrigin.position, _slapOrigin.forward, slapStrength);
        Debug.DrawLine(_slapOrigin.position, _slapColliders[i].transform.position, Color.red, 1.0f);
      }
    }
  }
}