using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform car;
    public Transform camera;

    public float height;
    public float distance;

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = car.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (rb.velocity.magnitude > 0.1f)
        {
            Vector3 a = car.position - rb.velocity.normalized * distance;
            transform.position = new Vector3(a.x, height, a.z);
            transform.LookAt(new Vector3(car.position.x, transform.position.y, car.position.z));
            camera.transform.LookAt(car);
        }
	}
}
