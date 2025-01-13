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
    
    //Location text objects
    public TextMeshProUGUI city;
    public TextMeshProUGUI state;
    public TextMeshProUGUI country;

    // Weather text objects
    public TextMeshProUGUI overview;
    public TextMeshProUGUI temperature;
    
    private void Start()
    {
        //Display OpenWeather info
        city.text = OpenWeather.Instance.cityName;
        state.text = OpenWeather.Instance.stateName;
        country.text = OpenWeather.Instance.countryName;

        overview.text = OpenWeather.Instance.currentWeather.weather[0].main;
        temperature.text = OpenWeather.Instance.currentWeather.main.temp.ToString();


        numOfCheckPointsInLevel = checkPoints.checkPoints.Length;
    }

    private void Update()
    {
        checkPointCounter.text = checkPoints.curCheckPoint.ToString() + "/" + numOfCheckPointsInLevel;
    }

}
