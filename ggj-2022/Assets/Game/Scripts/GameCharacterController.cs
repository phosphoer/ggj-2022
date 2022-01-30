using UnityEngine;

public class GameCharacterController : MonoBehaviour, ISlappable
{
  public ePlayer Team { get; set; }

  public bool IsStunned => _stunTimer > 0;
  public float CurrentSpeed => DesiredSpeed * _maxSpeed;

  [Range(0, 1)]
  public float DesiredSpeed = 0.0f;

  [Range(-1, 1)]
  public float DesiredTurn = 0.0f;

  [SerializeField]
  private RobotAnimation _robotAnim = null;

  [SerializeField]
  private SlapController _slapper = null;

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

  [SerializeField]
  private SoundBank _fastSlapSound = null;

  [SerializeField]
  private SoundBank _doubleSlapSound = null;

  [SerializeField]
  private SoundBank _slapReceivedSound = null;

  [SerializeField]
  private SoundBank _trundleSound = null;

  private RaycastHit _groundRaycast;
  private RaycastHit _obstacleRaycast;
  private Vector3 _lastGroundPos;
  private float _stunTimer;
  private float _nextSlapStrength;
  private AudioManager.AudioInstance _trundleAudio;
  private AudioManager.AudioInstance _idleAudio;

  private Vector3 _raycastStartPos => transform.position + transform.up * _raycastUpStartOffset;
  private bool _isSlapping;

  void ISlappable.ReceiveSlap(ePlayer fromTeam, Vector3 slapOrigin, Vector3 slapDirection, float slapStrength)
  {
    Debug.Log($"{name} got slapped with strength {slapStrength}!");
    _stunTimer = slapStrength;

    AudioManager.Instance?.PlaySound(_slapReceivedSound);
  }

  public void FastSlap()
  {
    if (!_isSlapping)
    {
      _isSlapping = true;
      AudioManager.Instance?.PlaySound(_fastSlapSound);

      _nextSlapStrength = 1.0f;
      if (_robotAnim != null)
        _robotAnim.FastSlap();
    }
  }

  public void DoubleSlap()
  {
    if (!_isSlapping)
    {
      _isSlapping = true;
      AudioManager.Instance?.PlaySound(_doubleSlapSound);

      _nextSlapStrength = 3.0f;
      if (_robotAnim != null)
        _robotAnim.DoubleSlap();
    }
  }

  private void Start()
  {
    _lastGroundPos = transform.position + Vector3.down * 100;

    if (_robotAnim != null)
    {
      _robotAnim.SlapAnimEvent += OnAnimEventSlap;
      _robotAnim.SlapAnimCompleteEvent += OnAnimEventSlapComplete;

    }
  }

  private void Update()
  {
    // Update animation
    if (_robotAnim != null)
    {
      _robotAnim.TrundleAmount = Mathf.Abs(DesiredSpeed);
      _robotAnim.IsStunned = _stunTimer > 0;
    }

    // Stunned logic
    if (_stunTimer > 0)
    {
      _stunTimer -= Time.deltaTime;
      return;
    }

    if (_trundleAudio == null)
    {
      _trundleAudio = AudioManager.Instance?.PlaySound(gameObject, _trundleSound, 0);
    }
    else
    {
      _trundleAudio.AudioSource.volume = Mathf.Abs(DesiredSpeed);
    }

    // Calculate next position based on movement
    Vector3 newPosition = transform.position + transform.forward * DesiredSpeed * _maxSpeed * Time.deltaTime;

    // Snap and align to ground
    Vector3 raycastDir = -transform.up + transform.forward * DesiredSpeed * 0.5f;
    if (Physics.SphereCast(_raycastStartPos, _groundRaycastRadius, raycastDir, out _groundRaycast, 3.0f, _groundLayer))
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

  private void OnAnimEventSlap()
  {
    _slapper.Slap(Team, _nextSlapStrength);
  }

  private void OnAnimEventSlapComplete()
  {
    _isSlapping = false;
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