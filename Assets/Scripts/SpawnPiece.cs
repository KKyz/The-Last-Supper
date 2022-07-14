using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnPiece : NetworkBehaviour
{
    private List<Vector3> piecePos = new List<Vector3>();
    private List<Quaternion> pieceRot = new List<Quaternion>();
    private  List<Vector3> randPos = new List<Vector3>();
    private  List<Quaternion> randRot = new List<Quaternion>();
    private GameObject newPiece;
    private Vector3 newRandPos;
    private Quaternion newRandRot;
    public int normalCount, psnCount, scrollCount;
    public float[] scrollProbability;
    public Sprite chalkBoard;
    public GameObject[] currentPiece;

    public void Start()
    {  
        RefreshPieceList();
        RpcInitPlate(normalCount, psnCount, scrollCount);    
    }

    private void RefreshPieceList()
    {
        piecePos.Clear();
        pieceRot.Clear();
        
        foreach (Transform child in transform)
        {
            if(child.CompareTag("PiecePos") || child.CompareTag("FoodPiece"))
            {
                piecePos.Add(child.transform.position);
                pieceRot.Add(child.transform.rotation);
            }

            if (child.CompareTag("PiecePos"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void RpcInitPlate(int normalPiece, int psnPiece, int scrollPiece)
    {
        if (!isServer)
        {return;}

        GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>().ShowChalk(chalkBoard);

        //In mode 0; Until i = normalPiece, keep adding pieces with "normal tag"
        //In mode 1; Until i = psnPiece, keep adding pieces with "normal tag"
        //In mode 2; Until i = scrollPiece keep adding scroll pieces
        //Code will check what the percentage of scroll pieces should be
        
        for (int i = normalPiece; i > 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, currentPiece.Length - 1);
            newPiece = Instantiate(currentPiece[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(0, null);
            newPiece.transform.SetParent(transform, true);
        }

        for (int i = psnPiece; i > 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, currentPiece.Length - 1);
            newPiece = Instantiate(currentPiece[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(1, null);
            newPiece.transform.SetParent(transform, true);
        }

        for (int i = scrollPiece - 1; i >= 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, currentPiece.Length - 1);
            newPiece = Instantiate(currentPiece[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(2, scrollProbability);
            newPiece.transform.SetParent(transform, true);
        }
    }

    /*This function runs on server side, and the pieces moved should sync w/ network transform component
    But doesn't sync to clients. Why?*/
    
    [Command(requiresAuthority = false)]
    public void Shuffle()
    {
        RefreshPieceList();
        randPos = piecePos;
        randRot = pieceRot;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("FoodPiece"))
            {
                newRandRot = randRot[Random.Range(0, randRot.Count - 1)];
                newRandPos = randPos[Random.Range(0, randPos.Count - 1)];
                child.position = newRandPos;
                child.rotation = newRandRot;
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
