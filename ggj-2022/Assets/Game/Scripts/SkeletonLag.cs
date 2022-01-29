using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonLag : MonoBehaviour
{
    public Transform SkeletonRoot;
    public float LagTime = .1f;
    public float MaxLagDist = .2f;

    float timer = 0f;
    List<Transform> skel = new List<Transform>();
    Vector3[] initOffsets = new Vector3[0];
    Vector3 lagChainPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        skel.AddRange(SkeletonRoot.GetComponentsInChildren<Transform>());
        GatherInitOffsets();
        RecordPosition();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>=LagTime)
        {
            timer = 0f;
            RecordPosition();
        }

        UpdateSkelPositions();
    }

    void RecordPosition()
    {
        lagChainPos = skel[skel.Count-1].position;
    }

    void UpdateSkelPositions()
    {
        Vector3 endTargetPos = lagChainPos;
        Vector3 endTargetInitOffsetPos = initOffsets[initOffsets.Length-1]+skel[0].position;
        float lagPosDist = Vector3.Distance(endTargetInitOffsetPos,lagChainPos);
        Vector3 lagPosDir = Vector3.Normalize(lagChainPos-endTargetInitOffsetPos);

        

        if(lagPosDist>0f)
        {
            if(lagPosDist > MaxLagDist)
            {
                endTargetPos = endTargetInitOffsetPos + (lagPosDir*MaxLagDist);
                lagPosDist = MaxLagDist;
            }

            
            int i=0;
            foreach (Transform t in skel)
            {
                //Offset..
                float traversalPerc = (float)i/skel.Count;
                //Debug.Log(traversalPerc);
                Debug.DrawRay(skel[i].position,lagPosDir*(lagPosDist*traversalPerc), Color.red);
                skel[i].position = (initOffsets[i] + skel[0].position) + (lagPosDir*(lagPosDist*traversalPerc));
                
                //Orient..
                if(i<skel.Count-1)
                {
                    Vector3 newUpDir = Vector3.Normalize(skel[i+1].position-skel[i].position);
                    skel[i].up = newUpDir*-1f;
                }

                i++;
            }
        }
    }

    void GatherInitOffsets()
    {
        initOffsets = new Vector3[skel.Count];
        for (int i=0; i<initOffsets.Length; i++)
        {
            initOffsets[i] = skel[i].position - skel[0].position;
        }
    }
}
