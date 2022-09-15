using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;

public class SpawnPiece : NetworkBehaviour
{
    public int normalCount, psnCount, scrollCount;
    public float[] scrollProbability;
    public Sprite chalkSprite;
    public string courseName;
    public AudioClip courseBGM;
    public GameObject[] pieces;
    
    private List<Vector3> piecePos = new();
    private List<Quaternion> pieceRot = new();
    private List<Vector3> randPos = new();
    private List<Quaternion> randRot = new();
    private GameObject newPiece;
    private Vector3 newRandPos;
    private Quaternion newRandRot;

    public void Start()
    {
        RefreshPieceList();
        
        if (isServer)
        {
            InitPlate(normalCount, psnCount, scrollCount);
        }
    }
    
    private void RefreshPieceList()
    {
        piecePos.Clear();
        pieceRot.Clear();
        
        foreach (Transform child in transform)
        {
            if((child.CompareTag("PiecePos") || child.CompareTag("FoodPiece")) && isServer)
            {
                piecePos.Add(child.position);
                pieceRot.Add(child.rotation);
            }

            if (child.CompareTag("PiecePos"))
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    private void InitPlate(int normalPiece, int psnPiece, int scrollPiece)
    {
        //In mode 0; Until i = normalPiece, keep adding pieces with "normal tag"
        //In mode 1; Until i = psnPiece, keep adding pieces with "poison tag"
        //In mode 2; Until i = scrollPiece keep adding scroll pieces
        //Code will check what the percentage of scroll pieces should be
        
        for (int i = normalPiece; i > 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, pieces.Length - 1);
            newPiece = Instantiate(pieces[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(0, null);
        }

        for (int i = psnPiece; i > 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, pieces.Length - 1);
            newPiece = Instantiate(pieces[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(1, null);
        }

        for (int i = scrollPiece - 1; i >= 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, pieces.Length - 1);
            newPiece = Instantiate(pieces[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(2, scrollProbability);
        }
        
        RpcUpdatePieceParent();
    }

    [ClientRpc]
    private void RpcUpdatePieceParent()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("FoodPiece"))
        {
            obj.transform.SetParent(transform, true);
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
                newRandRot = randRot[Random.Range(0, randRot.Count - 1)];
                newRandPos = randPos[Random.Range(0, randPos.Count - 1)];
                child.position = newRandPos;
                child.rotation = newRandRot;
                randPos.Remove(newRandPos);
                randRot.Remove(newRandRot);

                foreach (Transform grandchild in child)
                { 
                    if (grandchild.CompareTag("Recommend") || grandchild.CompareTag("TypeFlag"))
                    {
                        Destroy(grandchild.gameObject);
                        NetworkServer.Destroy(grandchild.gameObject);
                    }
                }
            }
        }
    }
}
