using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour {

    RoadGenerator roadGenerator;

    public void InitializeCollisionCheck(RoadGenerator roadGenerator)
    {
        this.roadGenerator = roadGenerator;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "CollisionChecker" && roadGenerator != null) {
            roadGenerator.HandleCollision(transform.parent.gameObject, other.transform.parent.gameObject);
        }
    }
}
