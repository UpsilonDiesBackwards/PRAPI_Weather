using System;
using System.Collections;
using System.Net;
using System.Text;
using Core;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class OpenWeather : MonoBehaviour {
    [SerializeField] private bool enable;
    
    public string baseURL = // Base URL of the API call
        "https://api.openweathermap.org/data/2.5/weather?q=";
    
    // Determines the location we want to get the weather of
    public string _cityName = "";
    public string _stateName = "";
    public string _countryName = "";
    private const int APICALLLIMIT = 1; // How many times we want to call the API. DO NOT CHANGE

    private double dateTime;
    
    [HideInInspector] public string finalisedURL; // Combined API URL we will use
    [HideInInspector] public string responseText; // JSON response from API in text

    [SerializeField] public WeatherResponse currentWeather;
    
    private void Start() {
        GetWeatherFromAPI();
    }

    public void GetWeatherFromAPI() {
        // Read assets/config.json for API key
        string json = System.IO.File.ReadAllText("Assets/config.json");
        Config config = JsonUtility.FromJson<Config>(json);
        
        // Append the city name, country name, and api key to the baseURL
        StringBuilder stringBuilder = new(baseURL, 256);
        stringBuilder.Append(_cityName + "," + _countryName + "&appid=" + config.api_key + "&units=metric");
        
        finalisedURL = stringBuilder.ToString();

        if (enable) { // If enabled, send an API request call
            StartCoroutine(GetRequest(finalisedURL));
        }
    }

    IEnumerator GetRequest(string url) {
        UnityWebRequest request = UnityWebRequest.Get(url); // Send a web address to the finalised URL
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) { // If the request is successful ...
            responseText = request.downloadHandler.text; 

            currentWeather = JsonUtility.FromJson<WeatherResponse>(responseText); // ... then set currentweather to the result

            currentWeather.sys.sunrise = GetDaySecondsFromUnixTime(currentWeather.sys.sunrise);
            currentWeather.sys.sunset = GetDaySecondsFromUnixTime(currentWeather.sys.sunset);
            currentWeather.dt = GetDaySecondsFromUnixTime(currentWeather.dt);
        }
        else {
            Debug.Log("OpenWeather API Error: " + request.error);
        }
    }

    private int GetDaySecondsFromUnixTime(int unixTime) {
        return unixTime % 86400; // 86400 is how many seconds there are in a day
    }
}
