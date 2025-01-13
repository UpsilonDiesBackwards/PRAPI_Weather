using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour {
    private static Startup instance;
    
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
}
