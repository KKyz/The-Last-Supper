using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;

public class SpawnPiece : NetworkBehaviour
{
    //[HideInInspector] 
    public readonly SyncList<int> pieceTypes = new();
    public Sprite chalkSprite;
    public string courseName;
    public GameObject[] pieces;
    public PieceTypes pieceType;
    
    private List<Vector3> piecePos = new();
    private List<Quaternion> pieceRot = new();
    private List<Vector3> randPos = new();
    private List<Quaternion> randRot = new();
    private GameObject newPiece;
    private Vector3 newRandPos;
    private Quaternion newRandRot;
    private GameManager gameManager;
    
    [System.Serializable]
    public class PieceTypes
    {
        public int normalAmount;
        public int psnAmount;
        public int scrollAmount;
    }

    public void Start()
    {
        RefreshPieceList();
        
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (isServer)
        {
            InitPlate();
        }
    }
    
    public void RefreshPieceList() 
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
    
    private void InitPlate()
    {
        //In mode 0; Until i = normalPiece, keep adding pieces with "normal tag"
        //In mode 1; Until i = psnPiece, keep adding pieces with "poison tag"
        //In mode 2; Until i = scrollPiece keep adding scroll pieces
        //Code will check what the percentage of scroll pieces should be
        
        pieceTypes.Clear();

        int[] types = {pieceType.normalAmount, pieceType.psnAmount, pieceType.scrollAmount};

        foreach (int amount in types)
        {
            pieceTypes.Add(amount); 
        }

        for (int i = pieceTypes[0]; i > 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, pieces.Length - 1);
            newPiece = Instantiate(pieces[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(0, 0, null);
        }

        for (int i = pieceTypes[1]; i > 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, pieces.Length - 1);
            newPiece = Instantiate(pieces[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(1, 0, null);
        }

        for (int i = pieceTypes[2] - 1; i >= 0; i--)
        {
            int j = Random.Range(i, piecePos.Count - 1);
            int k = Random.Range(0, pieces.Length - 1);
            newPiece = Instantiate(pieces[k], piecePos[j], pieceRot[j]);
            NetworkServer.Spawn(newPiece);
            piecePos.RemoveAt(j);
            pieceRot.RemoveAt(j);
            newPiece.GetComponent<FoodPiece>().SetType(2, gameManager.scrollProb, gameManager.availableScrolls.ToArray());
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
            }
        }
        
        RpcDestroyFlags();
    }

    [ClientRpc]
    private void RpcDestroyFlags()
    {
        foreach (Transform piece in transform)
        {
            foreach (Transform grandchild in piece)
            { 
                if (grandchild.CompareTag("Recommend") || grandchild.CompareTag("TypeFlag"))
                {
                    Destroy(grandchild.gameObject);
                }
            }
        }
    }
}
