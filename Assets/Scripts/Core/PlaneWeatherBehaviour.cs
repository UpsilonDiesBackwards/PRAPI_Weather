using System;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using Random = UnityEngine.Random;

namespace Core {
    public class PlaneWeatherBehaviour : MonoBehaviour {
        private Rigidbody _rb;

        [Header("Turbulence")] 
        public float turbIntensity;
        private float _windSpeed;
        private float _gustStr;
        private float _windDir;
        
        [Header("Perlin Noise")]
        public float noiseScale = 0.1f;
        private float _noiseVariant;

        private Vector3 force;
        
        void OnDrawGizmos() {
            if (!Startup.Instance.debugMode) return;
            
            // Wind direction
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + force);

            // Gusts
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + _gustStr * Vector3.up);
        }

        
        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
            SmoothWind(force, 2.0f);
            
            TurbuleyWurbley();
        }
        
        public void UpdateData() {
            _windSpeed = OpenWeather.Instance.currentWeather.wind.speed;
            _gustStr   = OpenWeather.Instance.currentWeather.wind.gust;
            _windDir    = OpenWeather.Instance.currentWeather.wind.deg;

            float radians = _windDir * Mathf.Deg2Rad;
            force = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians) * _windSpeed);
        }

        void TurbuleyWurbley() {
            Vector3 gustForce = force * Random.Range(1.0f, _gustStr);

            float noiseX = Mathf.PerlinNoise(Time.time * noiseScale + _noiseVariant, 0) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0, Time.time * noiseScale + _noiseVariant) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(Time.time * noiseScale, Time.time * noiseScale) - 0.5f;
            Vector3 noiseTurb = new Vector3(noiseX, noiseY, noiseZ) * turbIntensity;
            
            // Apply turbulence
            Vector3 turbulence = gustForce + noiseTurb;
            _rb.AddForce(turbulence, ForceMode.Force);
            
            // Rotational turbulence
            Vector3 torque = new Vector3(noiseX, noiseY, noiseZ) * turbIntensity * 0.1f;
            _rb.AddTorque(torque, ForceMode.Force);
        }

        void SmoothWind(Vector3 tar, float speed) {
            force = Vector3.Lerp(force, tar, Time.deltaTime * speed);
        }
    }
}
