using UnityEngine;

public class Rotate : MonoBehaviour
{
  public Vector3 RotateAmount;
  public Space Space;
  public float RotateSpeed = 1.0f;

  private void Update()
  {
    transform.Rotate(RotateAmount * RotateSpeed * Time.deltaTime, Space);
  }
}