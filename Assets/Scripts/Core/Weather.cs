using System;

namespace Core {
    [Serializable]
    public struct Main {
        public float temp;
        public float feels_like;
        public float temp_min;
        public float temp_max;
        public float pressure;
        public int humidity;
        public int sea_level;
        public int grnd_level;
        
    }

    [Serializable]
    public struct Weather {
        public int id;
        public string main;
        public string description;
    }

    [Serializable]
    public struct Visibility {
        public int visibility;
    }

    [Serializable]
    public struct Wind {
        public float speed;
        public float deg;
        public float gust;
    }

    [Serializable]
    public struct Rain {
        public float rain;
    }

    [Serializable]
    public struct Clouds {
        public int all;
    }
    
    [Serializable]
    public struct DT {
        public string dt;
    }
    
    [Serializable]
    public struct Sys {
        public string country;
        public string sunrise;
        public string sunset;
    }
    
    [Serializable]
    public struct WeatherResponse {
        public Main main;
        public Weather[] weather;
        public int visibility;
        public Wind wind;
        public Rain rain;
        public Clouds clouds;
        public string dt;
        public Sys sys;
    }
}
