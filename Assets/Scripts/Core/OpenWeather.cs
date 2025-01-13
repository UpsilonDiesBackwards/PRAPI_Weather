using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Core;
using Environment;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class OpenWeather : MonoBehaviour {
    private static OpenWeather instance;
    
    [SerializeField] private bool enable;

    public string baseURL = "https://api.openweathermap.org/data/2.5/weather?q=";

    // This variable will hold the city name
    public string cityName = "";
    public string stateName = "";
    public string countryName = "";
    private const int APICALLLIMIT = 1; // How many times we want to call the API. DO NOT CHANGE

    private double dateTime;

    [HideInInspector] public string finalisedURL; // Combined API URL we will use
    [HideInInspector] public string responseText; // JSON response from API in text
    [HideInInspector] public string apiErrorString; // Error String
    
    [SerializeField] public WeatherResponse currentWeather;
    
    [Header("Environment Manager References")]
    public Sun sunManager;

    [SerializeField] private PlaneWeatherBehaviour _planeWeatherBehaviour;
    
    public List<String> locations = new List<string>(new string[] {
        "New York", "London", "Tokyo", "Paris", "Sydney", 
        "Los Angeles", "Chicago", "Toronto", "Beijing", "Berlin", 
        "Rio de Janeiro", "Mexico City", "Cairo", "Dubai", "Moscow", 
        "Mumbai", "Delhi", "Istanbul", "Bangkok", "Seoul", 
        "Hong Kong", "Singapore", "Barcelona", "Rome", "Vienna", 
        "Madrid", "Buenos Aires", "Cape Town", "Jakarta", "Lagos", 
        "Shanghai", "Kuala Lumpur", "Lima", "Santiago", "Caracas", 
        "Casablanca", "Kabul", "Tehran", "Baghdad", "Karachi", 
        "Riyadh", "Doha", "Manila", "Hanoi", "Lisbon", 
        "Dublin", "Edinburgh", "Prague", "Warsaw", "Budapest", 
        "Oslo", "Stockholm", "Helsinki", "Copenhagen", "Reykjavik", 
        "Athens", "Belgrade", "Zagreb", "Bratislava", "Ljubljana", 
        "Vilnius", "Riga", "Tallinn", "Bucharest", "Sofia", 
        "Brussels", "Amsterdam", "Luxembourg", "Zurich", "Geneva", 
        "Monaco", "San Marino", "Valletta", "Andorra la Vella", "Yerevan", 
        "Tbilisi", "Baku", "Tashkent", "Astana", "Ulaanbaatar", 
        "Havana", "Kingston", "Port-au-Prince", "Panama City", "San Salvador", 
        "Guatemala City", "Belmopan", "Managua", "Tegucigalpa", "San Jose", 
        "Bogota", "Quito", "Asuncion", "Montevideo", "Brasilia", 
        "Pretoria", "Nairobi", "Accra", "Addis Ababa", "Abuja",
        "Portsmouth", "Peoria", "Champaign", "Cardiff"
    });

    public static OpenWeather Instance {
        get {
            if (instance == null) instance = FindObjectOfType<OpenWeather>();
            return instance;
        }
    }

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        _planeWeatherBehaviour = FindObjectOfType<PlaneWeatherBehaviour>().GetComponent<PlaneWeatherBehaviour>();
    }

    public void GetWeatherFromAPI() {
        // Read the API key from the config file
        string json = System.IO.File.ReadAllText("Assets/config.json");
        Config config = JsonUtility.FromJson<Config>(json);

        // Build the final URL for the API request
        StringBuilder stringBuilder = new StringBuilder(baseURL, 256);
        stringBuilder.Append(cityName + "," + countryName + "&appid=" + config.api_key + "&units=metric");

        finalisedURL = stringBuilder.ToString();
        stringBuilder.Clear();
        
        if (enable) { // If enabled, send an API request
            StartCoroutine(GetRequest(finalisedURL));
        } 
    }

    IEnumerator GetRequest(string url) {
        UnityWebRequest request = UnityWebRequest.Get(url); // Send a request to the finalised URL
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            responseText = request.downloadHandler.text;

            // Deserialize JSON response into WeatherResponse object
            currentWeather = JsonUtility.FromJson<WeatherResponse>(responseText);

            currentWeather.sys.sunrise = GetDaySecondsFromUnixTime(currentWeather.sys.sunrise);
            currentWeather.sys.sunset = GetDaySecondsFromUnixTime(currentWeather.sys.sunset);
            currentWeather.dt = GetDaySecondsFromUnixTime(currentWeather.dt);

            _planeWeatherBehaviour.UpdateData();
            
            sunManager.UpdateSunPosition(currentWeather.sys.sunrise, currentWeather.sys.sunset, 
                currentWeather.dt);
        }
        else {
            Debug.Log("OpenWeather API Error: " + request.error);
            apiErrorString = request.error;
        }
    }

    private int GetDaySecondsFromUnixTime(int unixTime) {
        return unixTime % 86400; // 86400 is how many seconds there are in a day
    }
}
