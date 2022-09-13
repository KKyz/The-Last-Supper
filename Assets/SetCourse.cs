using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCourse : MonoBehaviour
{
    private Dropdown restaurantDropdown, menuDropdown;
    private List<string> restaurantNames, menuNames;
    private MealManager mealManager;//

    public void Start()
    {
        mealManager = GameObject.Find("StateManager").GetComponent<MealManager>();
        restaurantDropdown = transform.Find("Restaurants").GetComponent<Dropdown>();
        menuDropdown = transform.Find("Menus").GetComponent<Dropdown>();
    }

    public void UpdateDropdowns()
    {
        restaurantNames.Clear();
        menuNames.Clear();
        restaurantDropdown.options.Clear();
        menuDropdown.options.Clear();
        
        //Populate restaurant dropdown with elements from MealManager's list of restaurants

        //Populate restaurant dropdown with elements from MealManager's list of restaurants
    }
}
