using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Startup : MonoBehaviour {
    private static Startup instance;

    public string API_KEY;

    public bool debugMode;
    
    public static Startup Instance {
        get {
            if (instance == null)  { instance = FindObjectOfType<Startup>(); }
            return instance;
        }
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(instance);
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.RightShift)) {
            debugMode = !debugMode;
        }
    }
}
