using System.Collections.Generic;
using UnityEngine;

public class RestaurantContents : MonoBehaviour
{
    public Material skyBox;
    public Sprite thumbnail;
    public string restaurantName;
    public int estPlayTime;
    public Menus[] menus;
    public AudioClip[] bgmClips;
    public GameObject[] playerModels;

    [HideInInspector] public List<Menus> availableMenus = new();

    [System.Serializable]
    public class Menus
    {
        public string menuName;
        public DataEnum condition;
        public int conditionVal;
        public GameObject[] meals;
    }
    
    public enum DataEnum
    {
        GamesWon, 
        GamesJoined,
        TeamGamesWon,
        GamesLost,
        None
    }
    
    public GameObject[] GetCourses(int index)
    {
        return availableMenus[index].meals;
    }
    
    public string GetNames(int index)
    {
        return menus[index].menuName;
    }
}
