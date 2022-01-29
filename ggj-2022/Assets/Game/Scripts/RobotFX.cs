using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFX : MonoBehaviour
{
    public enum RobotFXStates{None,Moving,Stunned}

    public ParticleSystem[] MovingFX = new ParticleSystem[0];
    public MeshRenderer treds; 
    public float TredsPanSpeed = .1f;

    public ParticleSystem[] StunnedFX = new ParticleSystem[0];
    

    RobotFXStates currentState = RobotFXStates.None;
    Material tredsMat;

    // Start is called before the first frame update
    void Start()
    {
        tredsMat = treds.material;
        ClearAll();
    }

    // Update is called once per frame
    void Update()
    {

        if(currentState==RobotFXStates.Moving)
        {
            float tredOffset = Time.time*TredsPanSpeed;
            tredsMat.SetTextureOffset("_MainTex", new Vector2(0f,tredOffset));
        }
    }

    public void AnimEvent(string eventString)
    {
        if(eventString=="Moving") SetState(RobotFXStates.Moving);
        else if(eventString=="Stunned") SetState(RobotFXStates.Stunned);
        else SetState(RobotFXStates.None);
    }

    public void SetState(RobotFXStates newState)
    {
        SetFX(currentState,false);
        currentState = newState;
        SetFX(currentState,true);
    }

    public void ClearAll()
    {
        SetFX(RobotFXStates.Moving, false);
        SetFX(RobotFXStates.Stunned, false);
        SetState(RobotFXStates.None);
    }

    void SetFX(RobotFXStates state, bool isEnabled)
    {
        if(state == RobotFXStates.Moving)
        {
            foreach (ParticleSystem fx in MovingFX)
            {
                if(isEnabled) fx.Play();
                else fx.Stop();
            }
        }
        else if(state == RobotFXStates.Stunned)
        {
            foreach (ParticleSystem fx in StunnedFX)
            {
                if(isEnabled) fx.Play();
                else fx.Stop();
            }
        }
    }
}
