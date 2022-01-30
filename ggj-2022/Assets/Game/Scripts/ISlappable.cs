using UnityEngine;

public interface ISlappable
{
  void ReceiveSlap(ePlayer fromTeam, Vector3 slapOrigin, Vector3 slapDirection, float slapStrength);
}