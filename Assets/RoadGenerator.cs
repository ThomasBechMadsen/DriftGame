using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {

    public float generationDistance; //Distance the generator will generate to
    public float deleteDistance; //Distance when the generator will start deleting pieces

    private List<GameObject> instantiatedPieces = new List<GameObject>();
    private GameObject[] roadPieces;

	// Use this for initialization
	void Start () {
        roadPieces = Resources.LoadAll<GameObject>("RoadPieces");
        GameObject[] startPieces = Resources.LoadAll<GameObject>("StartPieces");
        instantiatedPieces.Add(Instantiate(startPieces[Random.Range(0, startPieces.Length - 1)]));
	}
	
	// Update is called once per frame
	void Update () {
        //Check delete distance on first instantiated piece
        if (Vector3.Distance(transform.position, instantiatedPieces[0].transform.position) > deleteDistance)
        {
            Destroy(instantiatedPieces[0]);
            instantiatedPieces.RemoveAt(0);
        }
        //Check generation distance on the latest piece
        if (Vector3.Distance(transform.position, instantiatedPieces[instantiatedPieces.Count-1].transform.position) < generationDistance)
        {
            generatePiece();
        }
	}

    void generatePiece()
    {
        GameObject newPiece = Instantiate(roadPieces[Random.Range(0, roadPieces.Length - 1)]);
        Transform lastPieceExit = instantiatedPieces[instantiatedPieces.Count - 1].transform.Find("ExitPoint");
        newPiece.transform.rotation = lastPieceExit.transform.rotation;
        newPiece.transform.position = lastPieceExit.position - newPiece.transform.Find("EntryPoint").position;
        instantiatedPieces.Add(newPiece);
    }
}
