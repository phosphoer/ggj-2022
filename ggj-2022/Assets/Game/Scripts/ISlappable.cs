using UnityEngine;

public interface ISlappable
{
  void ReceiveSlap(Vector3 slapOrigin, Vector3 slapDirection, float slapStrength);
}