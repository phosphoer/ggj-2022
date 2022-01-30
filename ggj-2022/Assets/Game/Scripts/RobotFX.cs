using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFX : MonoBehaviour
{
  public enum RobotFXStates { None, Moving, Stunned }

  public GameCharacterController CharacterController = null;
  public MeshRenderer Treads;
  public float TredsPanSpeedMod = .5f;

  public ParticleSystem[] MovingFX = null;
  public ParticleSystem[] StunnedFX = null;

  private RobotFXStates currentState = RobotFXStates.None;
  private Material treadsMat;

  private static readonly int kTreadsTexture = Shader.PropertyToID("_MainTex");

  private void Start()
  {
    treadsMat = Instantiate(Treads.sharedMaterial);
    Treads.sharedMaterial = treadsMat;
    ClearAll();
  }

  private void OnDestroy()
  {
    Destroy(treadsMat);
  }

  private void Update()
  {
    if (CharacterController != null)
    {
      SetFX(RobotFXStates.Moving, Mathf.Abs(CharacterController.DesiredSpeed) > 0);
      SetFX(RobotFXStates.Stunned, CharacterController.IsStunned);

      treadsMat.SetTextureOffset(kTreadsTexture, new Vector2(0f, treadsMat.GetTextureOffset(kTreadsTexture).y + (CharacterController.CurrentSpeed * TredsPanSpeedMod * Time.deltaTime)));
    }
  }

  private void ClearAll()
  {
    SetFX(RobotFXStates.Moving, false);
    SetFX(RobotFXStates.Stunned, false);
  }

  private void SetFX(RobotFXStates state, bool isEnabled)
  {
    if (state == RobotFXStates.Moving)
    {
      foreach (ParticleSystem fx in MovingFX)
      {
        if (isEnabled)
        {
          if (!fx.isPlaying)
            fx.Play();
        }
        else
        {
          if (fx.isPlaying)
            fx.Stop();
        }
      }
    }
    else if (state == RobotFXStates.Stunned)
    {
      foreach (ParticleSystem fx in StunnedFX)
      {
        if (isEnabled)
        {
          if (!fx.isPlaying)
            fx.Play();
        }
        else
        {
          if (fx.isPlaying)
            fx.Stop();
        }
      }
    }
  }
}
