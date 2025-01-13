using System;
using System.Collections;
using Environment;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MenuManager : MonoBehaviour {
    [Header("Interface")]
    public GameObject mainMenuScreen;
    public GameObject locationSelectionMenuScreen;
    public GameObject levelCompleteScreen;
    public GameObject stallWarning;

    [Header("References")] 
    public PlaneController planeController;
    
    public TMP_InputField city;
    public TMP_InputField state;
    public TMP_InputField country;

    public TextMeshProUGUI errorText;
    
    public void Update() {
        if (locationSelectionMenuScreen.activeSelf) {
            OpenWeather.Instance.cityName = city.text;
            OpenWeather.Instance.stateName = state.text;
            OpenWeather.Instance.countryName = country.text;
        }

        if (levelCompleteScreen.activeSelf) {
            Cursor.lockState = CursorLockMode.None;
            GotoMainMenu("StartScreen");
        }

        if (planeController.hasCrashed) {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("StartScreen");
        }
        
        stallWarning.SetActive(planeController.isStalling);
    }

    public void GotoRandomizedLocation() {
        int index = Random.Range(0, OpenWeather.Instance.locations.Count);
        OpenWeather.Instance.cityName = OpenWeather.Instance.locations[index];

        SceneManager.LoadScene("GameScene");
        OpenWeather.Instance.GetWeatherFromAPI();
    }
    
    public void GotoSelectionScreen() {
        locationSelectionMenuScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }
    
    public void GotoMainMenuScreen() {
        mainMenuScreen.SetActive(true);
        locationSelectionMenuScreen.SetActive(false);
    }
        
    public void GotoMainMenu(string name) {
        // Reset OpenWeather location data
        OpenWeather.Instance.cityName = "";
        OpenWeather.Instance.stateName = "";
        OpenWeather.Instance.countryName = "";
        
        float delay = 5f;
        StartCoroutine(ReturnToMainMenu(delay, name));
    }

    public void LoadSelectedScene(string name) {
        if (OpenWeather.Instance.apiErrorString == "") {
            SceneManager.LoadScene(name);
        } else {
            errorText.text = OpenWeather.Instance.apiErrorString;
            Debug.Log("Location is not valid");
        }
    }
    
    public void QuitGame() {
        Application.Quit();

        // For editor use
        Debug.LogAssertion("Game has been quit");
    }

    void Start() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        OpenWeather.Instance.sunManager = FindObjectOfType<Sun>();
    }

    IEnumerator ReturnToMainMenu(float time, string sceneName) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(sceneName);
    }
}
