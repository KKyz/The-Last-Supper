using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCourse : MonoBehaviour
{
    private Dropdown restaurantDropdown, menuDropdown;
    private MealManager mealManager;

    [HideInInspector] public GameObject selectedRestaurant, selectedMenu;
    public List<GameObject> restaurants = new();
    public List<GameObject> menus = new();

    void Start()
    {
        restaurantDropdown = transform.Find("Restaurants").GetComponent<Dropdown>();
        menuDropdown = transform.Find("Courses").GetComponent<Dropdown>();
        mealManager = GameObject.Find("StateManager").GetComponent<MealManager>();
        restaurantDropdown.ClearOptions();
        menuDropdown.ClearOptions();


        foreach (GameObject restaurant in mealManager.restaurants)
        {
            restaurants.Add(restaurant);
        }
    }

    public void AddRestaurantNames()
    {
        List<GameObject> restaurants = new();
        foreach (GameObject restaurant in mealManager.restaurants)
        {
            restaurants.Add(restaurant);
        }

        //restaurantDropdown.AddOptions(restaurants);
    }
}
