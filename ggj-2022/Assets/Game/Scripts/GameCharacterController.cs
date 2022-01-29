using UnityEngine;

public class GameCharacterController : MonoBehaviour, ISlappable
{
  [Range(0, 1)]
  public float DesiredSpeed = 0.0f;

  [Range(-1, 1)]
  public float DesiredTurn = 0.0f;

  [SerializeField]
  private LayerMask _groundLayer = default(LayerMask);

  [SerializeField]
  private LayerMask _obstacleLayer = default(LayerMask);

  [SerializeField]
  private float _groundRaycastRadius = 0.4f;

  [SerializeField]
  private float _obstacleRaycastRadius = 0.7f;

  [SerializeField]
  private float _minDistToObstacle = 0.8f;

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
  private RaycastHit _obstacleRaycast;
  private Vector3 _lastGroundPos;

  private Vector3 _raycastStartPos => transform.position + transform.up * _raycastUpStartOffset;

  void ISlappable.ReceiveSlap(Vector3 slapOrigin, Vector3 slapDirection)
  {
    transform.position += slapDirection;
  }

  private void Start()
  {
    _lastGroundPos = transform.position + Vector3.down * 100;
  }

  private void Update()
  {
    // Calculate next position based on movement
    Vector3 newPosition = transform.position + transform.forward * DesiredSpeed * _maxSpeed * Time.deltaTime;

    // Snap and align to ground
    if (Physics.SphereCast(_raycastStartPos, _groundRaycastRadius, -transform.up, out _groundRaycast, 3.0f, _groundLayer))
    {
      _lastGroundPos = _groundRaycast.point;

      Vector3 toGroundPoint = _groundRaycast.point - newPosition;
      newPosition += Vector3.ClampMagnitude(toGroundPoint, 1f) * Time.deltaTime * _gravity;

      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, _groundRaycast.normal) * transform.rotation;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);
    }
    // If no ground, go towards where it was last
    else
    {
      Vector3 fallDir = (_lastGroundPos - newPosition).normalized;
      Quaternion desiredRot = Quaternion.FromToRotation(transform.up, -fallDir) * transform.rotation;

      newPosition += fallDir * Time.deltaTime * _gravity;
      transform.rotation = Mathfx.Damp(transform.rotation, desiredRot, 0.25f, Time.deltaTime * _terrainAlignmentSpeed);
    }

    // Collide with obstacles
    Vector3 moveVec = newPosition - transform.position;
    if (Physics.SphereCast(transform.position, _obstacleRaycastRadius, moveVec.normalized, out _obstacleRaycast, _minDistToObstacle + 1, _obstacleLayer))
    {
      // Find the plane representing the point + normal we hit
      Plane hitPlane = new Plane(_obstacleRaycast.normal, _obstacleRaycast.point);

      // Now project our position onto that plane and use the vector from
      // the projected point to our pos as the adjusted normal
      Vector3 closestPoint = hitPlane.ClosestPointOnPlane(newPosition);
      Vector3 closestPointToPos = newPosition - closestPoint;

      // "Clamp" our distance from the plane to a min distance
      float planeDist = closestPointToPos.magnitude;
      float adjustedDist = Mathf.Max(planeDist, _minDistToObstacle);
      newPosition = closestPoint + closestPointToPos.normalized * adjustedDist;
    }

    // Apply movement
    transform.position = newPosition;
    transform.Rotate(Vector3.up, DesiredTurn * _turnSpeed * Time.deltaTime, Space.Self);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.white;
    Gizmos.DrawWireSphere(_raycastStartPos, _groundRaycastRadius);

    if (_groundRaycast.collider != null)
    {
      Gizmos.DrawWireSphere(_groundRaycast.point, _groundRaycastRadius);
      Gizmos.DrawLine(_raycastStartPos, _groundRaycast.point);
    }
    else
    {
      Gizmos.DrawLine(_raycastStartPos, _lastGroundPos);
    }

    if (_obstacleRaycast.collider != null)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, _minDistToObstacle);
    }
    else
    {
      Gizmos.color = Color.white;
      Gizmos.DrawWireSphere(transform.position, _minDistToObstacle);
    }
  }
}