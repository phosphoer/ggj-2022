using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientBodySounds : MonoBehaviour
{
    public SoundBank AmbientBodySounds_Heart;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance?.PlaySound(AmbientBodySounds_Heart);
    }
}

