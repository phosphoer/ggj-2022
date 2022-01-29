using UnityEngine;

public class GameCharacterController : MonoBehaviour
{
  [Range(0, 1)]
  public float DesiredSpeed = 0.0f;

  [Range(-1, 1)]
  public float DesiredTurn = 0.0f;

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

  [SerializeField]
  private float _turnSpeed = 1.0f;

  [SerializeField]
  private float _gravity = 5;

  private RaycastHit _groundRaycast;
  private Vector3 _lastGroundPos;

  private Vector3 _raycastStartPos => transform.position + transform.up * _raycastUpStartOffset;

  private void Start()
  {
    _lastGroundPos = transform.position + Vector3.down * 100;
  }

  private void Update()
  {
    // Apply movement 
    transform.position += transform.forward * DesiredSpeed * _maxSpeed * Time.deltaTime;
    transform.Rotate(Vector3.up, DesiredTurn * _turnSpeed * Time.deltaTime, Space.Self);

    // Snap and align to ground
    if (Physics.SphereCast(_raycastStartPos, _raycastRadius, -transform.up, out _groundRaycast, 3.0f, _groundLayer))
    {
      _lastGroundPos = _groundRaycast.point;

      Vector3 toGroundPoint = _groundRaycast.point - transform.position;
      transform.position += Vector3.ClampMagnitude(toGroundPoint, 1f) * Time.deltaTime * _gravity;

      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, _groundRaycast.normal) * transform.rotation;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);
    }
    // If no ground, go towards where it was last
    else
    {
      Vector3 fallDir = (_lastGroundPos - transform.position).normalized;
      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, -fallDir) * transform.rotation;

      transform.position += fallDir * Time.deltaTime * _gravity;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.white;
    Gizmos.DrawWireSphere(_raycastStartPos, _raycastRadius);

    if (_groundRaycast.collider != null)
    {
      Gizmos.DrawWireSphere(_groundRaycast.point, _raycastRadius);
      Gizmos.DrawLine(_raycastStartPos, _groundRaycast.point);
    }
    else
    {
      Gizmos.DrawLine(_raycastStartPos, _lastGroundPos);
    }
  }
}