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
        RefreshPieceList();
        RpcInitPlate();    
    }

    private void RefreshPieceList()
    {
        piecePos.Clear();
        pieceRot.Clear();
        
        foreach (Transform child in transform)
        {
            if(child.CompareTag("PiecePos"))
            {
                piecePos.Add(child.transform.position);
                pieceRot.Add(child.transform.rotation);    
                Destroy(child.gameObject);
            }
        }
    }
    
    private void RpcInitPlate()
    {
        if (!isServer)
        {return;}
        
        //Debug.Log("PlateCreated");
        for (int i = 0; i < piecePos.Count; i++)
        {
            newPiece = Instantiate(currentPiece, piecePos[i], pieceRot[i]);
            NetworkServer.Spawn(newPiece);
            newPiece.transform.SetParent(transform);
        }
    }

    public void Shuffle()
    {
        RefreshPieceList();
        randPos = piecePos;
        randRot = pieceRot;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("FoodPiece"))
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
