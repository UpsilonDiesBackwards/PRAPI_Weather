using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class RadarController : MonoBehaviour
{
    public Transform plane;              // Reference to the plane's transform
    public float radarRange = 100f;      // Range of the radar
    public GameObject radarMarkerPrefab; // Prefab for radar markers
    private GameObject radarMarker;      // Instance of the radar marker

    void Start()
    {
        // Instantiate the radar marker
        radarMarker = Instantiate(radarMarkerPrefab, transform);
    }

    void Update()
    {
        // Update radar position and orientation
        UpdateRadarPosition();
        UpdateRadarMarker();
    }

    void UpdateRadarPosition()
    {
        // Position the radar above the plane
        Vector3 radarPosition = plane.position + Vector3.up * 10f; // Adjust height as needed
        transform.position = radarPosition;

        // Rotate the radar to match the plane's yaw
        transform.rotation = Quaternion.Euler(0, plane.eulerAngles.y, 0);
    }

    void UpdateRadarMarker()
    {
        // Calculate the distance from the plane to the radar marker
        float distance = Vector3.Distance(plane.position, radarMarker.transform.position);

        // If the distance is within radar range, update the marker position
        if (distance < radarRange)
        {
            radarMarker.transform.position = plane.position + (plane.forward * distance);
        }
        else
        {
            // If out of range, move the marker to the edge of the radar
            radarMarker.transform.position = plane.position + (plane.forward * radarRange);
        }
    }
}
