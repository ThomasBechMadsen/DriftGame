using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class engineScript : MonoBehaviour {

    public CarController cc;
    public AudioSource engineSpeedupSound;
    public Gear[] gears = new Gear[6];
    private int currentGear;

    public float minPitch;
    public float maxPitch;
    private float pitchShare;

    void Start()
    {
        currentGear = 0;
        pitchShare = (maxPitch - minPitch) / gears.Length;
        for (int i = 0; i < gears.Length; i++)
        {
            gears[i].minPitch = minPitch + (pitchShare / 2) * i;
            gears[i].maxPitch = gears[i].minPitch + pitchShare;
        }
        gears[0].minPitch = 0;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Gear gear = gears[currentGear];
        float a = (cc.totalVel - gear.minVel) / (gear.maxVel - gear.minVel);
        engineSpeedupSound.pitch = Mathf.LerpUnclamped(gear.minPitch, gear.maxPitch, a);

        if (cc.totalVel > gear.maxVel || cc.totalVel < gear.minVel)
        {
            currentGear = switchGears();
        }
    }

    int switchGears()
    {
        if (cc.totalVel < gears[0].minVel)
        {
            return 0;
        }
        if(cc.totalVel > gears[gears.Length - 1].maxVel)
        {
            return gears.Length - 1;
        }
        for (int i = 0; i < gears.Length; i++)
        {
            if (gears[i].maxVel > cc.totalVel && gears[i].minVel < cc.totalVel)
            {
                print("new gear: " + (i+1));
                return i;
            }
        }
        return -1;
    }
}

[System.Serializable]
public class Gear
{
    public float minVel;
    public float maxVel;
    public float minPitch;
    public float maxPitch;
}
