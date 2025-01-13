using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public OpenWeather openWeather;

    public CheckPoints checkPoints;

    private void Awake()
    {
        //DontDestroyOnLoad(this);
    }

    private void Start()
    {

    }

    public void LocationSelect()
    {
        SceneManager.LoadSceneAsync("LocationSelection");
    }

    public void StartGame()
    {
        // if(openWeather.validLocations == true)
        // {
        //     SceneManager.LoadSceneAsync("Level 1");
        // }
        // else
        // {
        //     Debug.Log("No valid loactions chosen");
        // }
    }

    public void Level2()
    {
        SceneManager.LoadSceneAsync("Level 2");
    }

    public void QuitGame()
    {
        Application.Quit();

        // For editor use
        Debug.Log("Game has been quit");
    }

}
