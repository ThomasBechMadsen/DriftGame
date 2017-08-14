using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    int turn = 0;
    // Update is called once per frame
    void FixedUpdate () {
        transform.rotation = Quaternion.Euler(turn, 45, 0);
        turn++;
	}
}
