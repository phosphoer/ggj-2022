using UnityEngine;

public class GameCharacterController : MonoBehaviour
{
  [Range(0, 1)]
  public float DesiredSpeed = 0.0f;

  [SerializeField]
  private LayerMask _groundLayer = default(LayerMask);

  [SerializeField]
  private float _raycastRadius = 0.5f;

  [SerializeField]
  private float _raycastUpStartOffset = 1.0f;

  [SerializeField]
  private float _terrainAlignmentSpeed = 3.0f;

  [SerializeField]
  private float _maxSpeed = 1.0f;

  private RaycastHit _groundRaycast;

  private void Update()
  {
    // Apply movement 
    transform.position += transform.forward * DesiredSpeed * _maxSpeed * Time.deltaTime;

    // Snap and align to ground
    Vector3 raycastStartPos = transform.position + transform.up * _raycastUpStartOffset;
    if (Physics.SphereCast(raycastStartPos, _raycastRadius, -transform.up, out _groundRaycast, 10.0f, _groundLayer))
    {
      transform.position = new Vector3(transform.position.x, _groundRaycast.point.y, transform.position.z);

      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, _groundRaycast.normal) * transform.rotation;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);

      // Debug.DrawLine(raycastStartPos, _groundRaycast.point, Color.white);
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.white;
    Gizmos.DrawWireSphere(transform.position + transform.up * _raycastUpStartOffset, _raycastRadius);
    Gizmos.DrawWireSphere(_groundRaycast.point, _raycastRadius);
    Gizmos.DrawLine(transform.position + transform.up * _raycastUpStartOffset, _groundRaycast.point);
  }
}