using System;
using System.Collections;
using System.Net;
using System.Text;
using Core;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;  // Add this to use TextMesh Pro InputField

public class OpenWeather : MonoBehaviour
{
    [SerializeField] private bool enable;

    public string baseURL = "https://api.openweathermap.org/data/2.5/weather?q=";

    // This variable will hold the city name
    public string _cityName = "";
    public string _stateName = "";
    public string _countryName = "";
    private const int APICALLLIMIT = 1; // How many times we want to call the API. DO NOT CHANGE

    private double dateTime;

    [HideInInspector] public string finalisedURL; // Combined API URL we will use
    [HideInInspector] public string responseText; // JSON response from API in text

    [SerializeField] public WeatherResponse currentWeather;

    // UI access to the area and change in runtime
    [SerializeField] private TMP_InputField cityInputField;
    [SerializeField] private TMP_InputField stateInputField;
    [SerializeField] private TMP_InputField countryInputField;

    //Display error messages
    public TextMeshProUGUI errorMessage;
    //Stop next scene loading with invalid location
    public bool validLocations = false;


    private void Start()
    {
        // Make sure TMP_InputField is assigned
        if (cityInputField != null)
        {
            // Add listener to the TMP_InputField onEndEdit event
            cityInputField.onEndEdit.AddListener(UpdateCityName);
        }
        if (stateInputField != null)
        {
            // Add listener to the TMP_InputField onEndEdit event
            stateInputField.onEndEdit.AddListener(UpdateStateName);
        }
        if (countryInputField != null)
        {
            // Add listener to the TMP_InputField onEndEdit event
            countryInputField.onEndEdit.AddListener(UpdateCountryName);
        }

        // Optionally, if you want to fetch weather immediately when a city name is set
        if (!string.IsNullOrEmpty(_cityName))
        {
            GetWeatherFromAPI();
        }


        DontDestroyOnLoad(gameObject);
    }

    // Update the string of the city name 
    private void UpdateCityName(string cityInput)
    {
        // Update _cityName with what the user typed in the InputField
        _cityName = cityInput;

        // API request right after updating the city name
        GetWeatherFromAPI();
    }
    private void UpdateStateName(string stateInput)
    {
        // Update _stateName with what the user typed in the InputField
        _stateName = stateInput;

        // API request right after updating the state name
        GetWeatherFromAPI();
    }
    private void UpdateCountryName(string countryInput)
    {
        // Update _stateName with what the user typed in the InputField
        _countryName = countryInput;

        // API request right after updating the country name
        GetWeatherFromAPI();
    }

    public void GetWeatherFromAPI()
    {
        StringBuilder errorBuilder = new StringBuilder();

        if (string.IsNullOrWhiteSpace(_cityName))
        {
            errorBuilder.AppendLine("City name is empty. Please enter a valid city.");
        }
        if (string.IsNullOrWhiteSpace(_stateName))
        {
            errorBuilder.AppendLine("State name is empty. Please enter a valid state.");
        }
        if (string.IsNullOrWhiteSpace(_countryName))
        {
            errorBuilder.AppendLine("Country name is empty. Please enter a valid country.");
        }

        if (errorBuilder.Length > 0)
        {
            errorMessage.text = errorBuilder.ToString();
            Debug.LogWarning(errorBuilder.ToString());
            return;
        }
        else
        {   
            validLocations = true;
        }



        // Read the API key from the config file
        string json = System.IO.File.ReadAllText("Assets/config.json");
        Config config = JsonUtility.FromJson<Config>(json);

        // Build the final URL for the API request
        StringBuilder stringBuilder = new StringBuilder(baseURL, 256);
        stringBuilder.Append(_cityName + "," + _countryName + "&appid=" + config.api_key + "&units=metric");

        finalisedURL = stringBuilder.ToString();

        if (enable)
        { // If enabled, send an API request
            StartCoroutine(GetRequest(finalisedURL));
        }
    }

    IEnumerator GetRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url); // Send a request to the finalised URL
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            responseText = request.downloadHandler.text;

            // Deserialize JSON response into WeatherResponse object
            currentWeather = JsonUtility.FromJson<WeatherResponse>(responseText);

            currentWeather.sys.sunrise = GetDaySecondsFromUnixTime(currentWeather.sys.sunrise);
            currentWeather.sys.sunset = GetDaySecondsFromUnixTime(currentWeather.sys.sunset);
            currentWeather.dt = GetDaySecondsFromUnixTime(currentWeather.dt);
        }
        else
        {
            Debug.Log("OpenWeather API Error: " + request.error);
        }


    }

    private int GetDaySecondsFromUnixTime(int unixTime)
    {
        return unixTime % 86400; // 86400 is how many seconds there are in a day
    }
}
