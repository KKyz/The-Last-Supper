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
            if(child.CompareTag("PiecePos"))
            {
                piecePos.Add(child.transform.position);
                pieceRot.Add(child.transform.rotation);    
                Destroy(child.gameObject);
            }
        }
    }
    
    private void RpcInitPlate(int normalPiece, int psnPiece, int scrollPiece)
    {
        if (!isServer)
        {return;}

        //Until i = normalPiece, keep adding pieces with "normal tag"
        //Until i = psnPiece, keep adding pieces with "normal tag"
        //Until i = scrollPiece keep adding scroll pieces
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
            newPiece.transform.SetParent(transform);
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
            newPiece.transform.SetParent(transform);
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
            newPiece.transform.SetParent(transform);
        }
    }

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
