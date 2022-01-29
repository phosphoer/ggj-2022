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
  private Vector3 _lastGroundDir = Vector3.down;

  private void Update()
  {
    // Apply movement 
    transform.position += transform.forward * DesiredSpeed * _maxSpeed * Time.deltaTime;
    transform.Rotate(Vector3.up, DesiredTurn * _turnSpeed * Time.deltaTime, Space.Self);

    // Snap and align to ground
    Vector3 raycastStartPos = transform.position + transform.up * _raycastUpStartOffset;
    if (Physics.SphereCast(raycastStartPos, _raycastRadius, -transform.up, out _groundRaycast, 10.0f, _groundLayer))
    {
      _lastGroundDir = _groundRaycast.point - raycastStartPos;

      float distToGround = _groundRaycast.distance + _raycastRadius - _raycastUpStartOffset;
      transform.position -= transform.up * distToGround * Time.deltaTime * _gravity;

      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, _groundRaycast.normal) * transform.rotation;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);
    }
    // If no ground, go towards where it was last
    else
    {
      transform.position -= _lastGroundDir * Time.deltaTime * _gravity;
      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, -_lastGroundDir) * transform.rotation;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);
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