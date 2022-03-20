using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnPiece : NetworkBehaviour
{
    private List<Vector3> piecePos = new List<Vector3>();
    private List<Quaternion> pieceRot = new List<Quaternion>();
    private List<Vector3> randPos = new List<Vector3>();
    private List<Quaternion> randRot = new List<Quaternion>();
    public GameObject currentPiece;
    private GameObject newPiece;
    private Vector3 newRandPos;
    private Quaternion newRandRot;
    
    public void Start()
    {  
        foreach (Transform child in transform)
        {
            if(child.gameObject.tag == "PiecePos")
            {
                piecePos.Add(child.transform.position);
                pieceRot.Add(child.transform.rotation);    
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < piecePos.Count; i++)
        {
            newPiece = Instantiate(currentPiece, piecePos[i], pieceRot[i]);
            newPiece.transform.SetParent(transform);
            NetworkServer.Spawn(newPiece);
        }
    }
    
    public void Shuffle()
    {
        randPos.AddRange(piecePos);
        randRot.AddRange(pieceRot);

            foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "FoodPiece")
            {
                newRandRot = randRot[Random.Range(0, randRot.Count)];
                newRandPos = randPos[Random.Range(0, randPos.Count)];
                child.transform.position = newRandPos;
                child.transform.rotation = newRandRot;
                randPos.Remove(newRandPos);
                randRot.Remove(newRandRot);

                foreach (Transform grandchild in child) 
                {
                    Destroy(grandchild.gameObject);
                    NetworkServer.Destroy(grandchild.gameObject);
                }
            }
        }
    }
}
