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
            
            Turbulence();
        } else {
            Debug.Log("OpenWeather API Error: " + request.error);
        }
    }

    private void Turbulence() { // ignore for now
        float thermal = 0.5f * ((currentWeather.main.temp - currentWeather.main.temp_min) /
                             (currentWeather.main.temp_max - currentWeather.main.temp_min)) * 100f;

        float jetStream = 0.5f * (currentWeather.wind.speed / 30) * 100;
        
        int weather = 0;
        if (currentWeather.weather[0].id >= 200 && currentWeather.weather[0].id < 300) { // Thunderstorms or severe weather
            weather = 40;
        } else if (currentWeather.weather[0].id >= 300 && currentWeather.weather[0].id < 600) { // Rain or snow
            weather = 20;
        } else if (currentWeather.weather[0].id >= 801 && currentWeather.weather[0].id <= 804) { // Clouds
            weather = 10;
        } else if (currentWeather.weather[0].id == 800) {
            weather = 0;
        }
        
        float weightThermal = 0.3f;
        float weightJetStream = 0.2f;
        float weightWeather = 0.5f;


        var turbulenceProbability = Mathf.Min(100, thermal + weather + jetStream);
        
        float totalWeight = weightThermal + weightJetStream + weightWeather;
        float weightedSum = (thermal * weightThermal) + (jetStream * weightJetStream) + (weather * weightWeather);
        
        var turbulenceIntensity = Mathf.Clamp01(weightedSum / (100f * totalWeight));
        
        Debug.Log("Thermal: " + thermal);
        Debug.Log("Jet Stream: " + jetStream);
        Debug.Log("Weather: " + weather);

        Debug.Log("Probability: " + turbulenceProbability);
        Debug.Log("Intensity: " + turbulenceIntensity);
    }
}
