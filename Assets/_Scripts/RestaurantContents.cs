using UnityEngine;

public class RestaurantContents : MonoBehaviour
{
    public Material skyBox;
    public Sprite thumbnail;
    public string restaurantName;
    public Menus[] menus;
    public PieceTypes[] pieceTypes;
    public AudioClip[] bgmClips;
    
    [System.Serializable]
    public class Menus
    {
        public string mealName;
        public GameObject[] meals;
    }
    
    public GameObject[] GetCourses(int index)
    {
        return menus[index].meals;
    }
    
    public string GetNames(int index)
    {
        return menus[index].mealName;
    }
    
    [System.Serializable]
    public class PieceTypes
    {
        public string course;
        public int normalAmount;
        public int psnAmount;
        public int scrollAmount;
    }

    public int[] GetTypeAmounts(int index)
    {
        int[] typeAmount = {pieceTypes[index].normalAmount, pieceTypes[index].psnAmount, pieceTypes[index].scrollAmount};
        return typeAmount;
    }
}
