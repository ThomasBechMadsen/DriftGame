using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicBarScript : MonoBehaviour {

    public int band;
    public float minScale;
    public float scaleMultiplier;

    MusicVisualizer mv;

	// Use this for initialization
	void Start () {
        mv = transform.parent.GetComponent<MusicVisualizer>();

    }

    // Update is called once per frame
    void Update() {
        transform.localScale = new Vector3(transform.localScale.x, mv.bands[band].bandBuffer * scaleMultiplier + minScale, transform.localScale.z);
	}
}
