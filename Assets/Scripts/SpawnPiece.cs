using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiece : MonoBehaviour
{
    private List<Vector3> piecePos = new List<Vector3>();
    private List<Vector3> randPos = new List<Vector3>();
    public GameObject currentPiece;
    private GameObject newPiece;
    private Vector3 newRandPos;

    void Start()
    {  
        foreach (Transform child in transform)
        {
            if(child.gameObject.tag == "PiecePos")
            {
                piecePos.Add(child.transform.position);
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < piecePos.Count; i++)
        {
            newPiece = Instantiate(currentPiece, piecePos[i], Quaternion.identity);
            newPiece.transform.SetParent(transform);
        }
    }


    public void Shuffle()
    {
        foreach (Transform child in transform)
        {
            newRandPos = piecePos[Random.Range(0, piecePos.Count)];

            if (!randPos.Contains(newRandPos) && child.gameObject.tag == "FoodPiece")
            {
                child.transform.position = newRandPos;
                randPos.Add(newRandPos);
                Debug.Log(randPos);
            }
            else if (randPos.Count >= (transform.childCount - 2))
            {
                Debug.Log("rand cleared");
                randPos.Clear();
                break;
            }
            else
            {
               newRandPos = piecePos[Random.Range(0, piecePos.Count)]; 
            }
        }
    }
}
