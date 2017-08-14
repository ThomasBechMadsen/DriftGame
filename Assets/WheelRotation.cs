using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotation : MonoBehaviour {

    public CarController controller;
    public Transform[] wheels = new Transform[4];
    public float smokeBeginVelocity;

    void Update()
    {
        //Turn tires based on forwardsVelocity
        //http://hyperphysics.phy-astr.gsu.edu/hbase/rotwe.html
        //w = radians per second
        //r = radius of wheel
        //v = translational velocity
        float degreesToTurn = (Mathf.Rad2Deg * (controller.forwardVel / 0.3376186f)) * Time.deltaTime;
        foreach (Transform wheel in wheels)
        {
            wheel.transform.Rotate(Vector3.right, degreesToTurn);
            if (controller.sidewaysVel > smokeBeginVelocity)
            {
                wheel.parent.GetChild(1).GetComponent<ParticleSystem>().Play();
            }
            else
            {
                wheel.parent.GetChild(1).GetComponent<ParticleSystem>().Stop();
            }
        }
    }
}
