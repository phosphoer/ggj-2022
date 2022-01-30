using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFX : MonoBehaviour
{
    public enum RobotFXStates{None,Moving,Stunned}

    public ParticleSystem[] MovingFX = new ParticleSystem[0];
    public float MinMoveForFX = .5f;
    public MeshRenderer treds; 
    public float TredsPanSpeedMod = .5f;

    public ParticleSystem[] StunnedFX = new ParticleSystem[0];
    

    RobotFXStates currentState = RobotFXStates.None;
    Material tredsMat;
    Vector3 lastPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        tredsMat = treds.material;
        lastPos = transform.position;
        ClearAll();
        
        
    }

    // Update is called once per frame
    void Update()
    {

        if(currentState==RobotFXStates.Moving)
        {
            float moveDist = Vector3.Distance(lastPos,transform.position);
            if(moveDist <= MinMoveForFX) SetFX(currentState, false);
            else SetFX(currentState, true);
            tredsMat.SetTextureOffset("_MainTex", new Vector2(0f,tredsMat.GetTextureOffset("_MainTex").y + (moveDist*TredsPanSpeedMod)));
        }
        lastPos = transform.position;
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
