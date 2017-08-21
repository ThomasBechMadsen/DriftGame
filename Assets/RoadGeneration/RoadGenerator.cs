using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {

    public float generationDistance; //Distance the generator will generate to
    public float deleteDistance; //Distance when the generator will start deleting pieces

    private List<GameObject> instantiatedPieces = new List<GameObject>();
    private List<int> failures = new List<int>();
    private GameObject[] roadPieces;

	// Use this for initialization
	void Start () {
        roadPieces = Resources.LoadAll<GameObject>("RoadPieces");
        GameObject[] startPieces = Resources.LoadAll<GameObject>("StartPieces");
        instantiatedPieces.Add(Instantiate(startPieces[Random.Range(0, startPieces.Length - 1)]));
        failures.Add(0);
        //StartCoroutine(debugRoutine());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        generatorPass();
	}

    void generatorPass()
    {
        //Check delete distance on first instantiated piece
        if (Vector3.Distance(transform.position, instantiatedPieces[0].transform.position) > deleteDistance)
        {
            Destroy(instantiatedPieces[0]);
            instantiatedPieces.RemoveAt(0);
            failures.RemoveAt(0);
        }
        //Check generation distance on the latest piece
        if (Vector3.Distance(transform.position, instantiatedPieces[instantiatedPieces.Count - 1].transform.position) < generationDistance)
        {
            generatePiece();
        }
    }

    void generatePiece()
    {
        //Previous piece
        instantiatedPieces[instantiatedPieces.Count - 1].transform.Find("Mesh").gameObject.SetActive(true);

        //New piece
        GameObject newPiece = Instantiate(roadPieces[Random.Range(0, roadPieces.Length)]);
        Transform lastPieceExit = instantiatedPieces[instantiatedPieces.Count - 1].transform.Find("ExitPoint");
        newPiece.transform.rotation = lastPieceExit.transform.rotation;
        newPiece.transform.position = lastPieceExit.position - newPiece.transform.Find("EntryPoint").position;
        newPiece.GetComponentInChildren<CollisionCheck>().InitializeCollisionCheck(this);
        instantiatedPieces.Add(newPiece);
        failures.Add(0);
    }

    public void HandleCollision(GameObject collider, GameObject collidedWith)
    {

        int colliderIndex = instantiatedPieces.IndexOf(collider);
        int collidedWithIndex = instantiatedPieces.IndexOf(collidedWith);

        if (collidedWithIndex >= 0 && colliderIndex > collidedWithIndex)
        {
            //print("Destroying " + colliderIndex + ", lost to " + instantiatedPieces.IndexOf(collidedWith));
            instantiatedPieces.RemoveAt(colliderIndex);
            failures.RemoveAt(colliderIndex);
            Destroy(collider);

            failures[colliderIndex - 1]++;
            for (int i = colliderIndex - 1; i > 0; i--)
            {
                if (failures[i] >= 3)
                {
                    //print("Destroying parent");
                    failures[i - 1]++;
                    Destroy(instantiatedPieces[i]);
                    instantiatedPieces.RemoveAt(i);
                    failures.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
        }
    }

    IEnumerator debugRoutine()
    {
        while (true)
        {
            generatorPass();
            yield return new WaitForSeconds(1);
        }
    }
}
