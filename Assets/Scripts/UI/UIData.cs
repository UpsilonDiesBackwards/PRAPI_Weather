using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIData : MonoBehaviour
{
    //Diplay how many checkpoints done/ are left!
    public CheckPoints checkPoints;
    public TextMeshProUGUI checkPointCounter;
    public int numOfCheckPointsInLevel;

    public OpenWeather openWeather;

    //Location text objects
    public TextMeshProUGUI city;
    public TextMeshProUGUI state;
    public TextMeshProUGUI country;

    private void Start()
    {
        // Find the OpenWeather script in the scene
        openWeather = FindObjectOfType<OpenWeather>();

        if (openWeather == null)
        {
            Debug.LogError("OpenWeather script not found in the scene!");
        }

        //Display OpenWeather info
        city.text = openWeather._cityName;
        state.text = openWeather._stateName;
        country.text = openWeather._countryName;

    }

    private void Update()
    {
        checkPointCounter.text = "CheckPoints:" + checkPoints.curCheckPoint.ToString() + "/" + numOfCheckPointsInLevel;

    }

}
