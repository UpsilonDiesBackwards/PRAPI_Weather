using Core;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Environment {
    public class Sun : MonoBehaviour {
        public Transform sun;
        public float radius = 100f;
        
        public void UpdateSunPosition(float sunrise, float sunset, float currentTime) {
            float dayDur = sunset - sunrise;
            float timeSinceSunrise = Mathf.Clamp(currentTime - sunrise, 0, dayDur);

            float timeNormalized = timeSinceSunrise / dayDur;
            
            float angle = timeNormalized * 180f;
            float radians = angle * Mathf.Deg2Rad;

            float altitudeRadius = radians * 0.5f;
            
            Vector3 sunPosition;
            sunPosition.x = Mathf.Cos(radians) * radius;
            sunPosition.y = Mathf.Sin(radians) * radius;
            sunPosition.z = Mathf.Cos(radians) * altitudeRadius;

            sun.position = sunPosition;
            
            sun.LookAt(Vector3.zero);
        }
    }
}
