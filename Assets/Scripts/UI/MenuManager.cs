using System;
using System.Collections;
using Cinemachine;
using Environment;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuManager : MonoBehaviour {
    [Header("Interface")]
    public GameObject mainMenuScreen;
    public GameObject locationSelectionMenuScreen;
    public TMP_InputField apiKeyInput;
    public GameObject levelCompleteScreen;
    public GameObject stallWarning;

    public GameObject[] menuButtons;
    
    [Header("References")] 
    public PlaneController planeController;
    
    public TMP_InputField city;
    public TMP_InputField state;
    public TMP_InputField country;

    public TextMeshProUGUI errorText;

    public void Update() {
        if (SceneManager.GetActiveScene().name == "StartScreen") {
            if (menuButtons.Length != null && menuButtons.Length > 0) {
                foreach (GameObject button in menuButtons) {
                    if (OpenWeather.Instance.usingDevKey) return;
                    
                    button.GetComponent<Button>().interactable = apiKeyInput.text != "";
                }   
            }
            
            Startup.Instance.API_KEY = apiKeyInput.text;
        }
        
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
            stallWarning.GetComponent<TextMeshProUGUI>().text = "YOU CRASHED";
            GotoMainMenu("StartScreen");
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

    void Awake() {
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
