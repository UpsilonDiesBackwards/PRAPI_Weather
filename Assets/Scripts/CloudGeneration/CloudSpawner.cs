using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] int cloudCount;
    [SerializeField] int cloudRadius;
    [SerializeField] int maxScale;
    [SerializeField] GameObject cloudPrefab;
    
    private int maxRange;
    private GameObject plane;

    // Start is called before the first frame update
    void Start()
    {
        maxRange = (int) gameObject.transform.localScale.x/2; //Cloud area should be kept square, as the max range is dependent on only one axis
        plane = GameObject.Find("Plane");
    }

    void FixedUpdate(){
        float planeDistance = Vector3.Distance(gameObject.transform.position - new Vector3(0, gameObject.transform.position.y) ,plane.transform.position - new Vector3(0, plane.transform.position.y));

        if (gameObject.transform.childCount == 0 && planeDistance < maxRange * 3){
            //Generates clouds if the plane is within range
            StartCoroutine(SpawnClouds());
        }
        else if (gameObject.transform.childCount > 0 && planeDistance >= maxRange * 3){
            StartCoroutine(DestroyClouds());
        }
    }

    private IEnumerator SpawnClouds(){
        for (int i = 0; i < cloudCount; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-maxRange, maxRange), gameObject.transform.position.y, Random.Range(-maxRange, maxRange));
            GameObject cloudObj = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);
            Cloud cloud = cloudObj.GetComponent<Cloud>();
            cloud.name = "Cloud";
            cloud.transform.SetParent(gameObject.transform);
            cloud.Generate(cloudObj, cloudRadius, maxScale);
        }

        return null;
    }

    private IEnumerator DestroyClouds(){
        while (gameObject.transform.childCount > 0){
            Destroy(gameObject.transform.GetChild(0).gameObject);
            yield return null;
        }
    }
}
