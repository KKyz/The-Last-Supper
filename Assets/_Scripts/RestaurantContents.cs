using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantContents : MonoBehaviour
{
    public AudioClip[] bgmClips;
    public Material skyBox;
    public Sprite thumbnail;
    public string restaurantName;
    public Menus[] menus;
    
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
}
