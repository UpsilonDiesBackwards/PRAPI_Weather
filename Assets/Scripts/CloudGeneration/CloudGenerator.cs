using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private int cloudRadius;
    private int CUTOFF = -2;
    private int sizeModX;
    private int sizeModZ;
    //[SerializeField] Dictionary<GameObject, int> subClouds;

    [SerializeField] GameObject subCloudPrefab;

    public void Generate(GameObject startCloud,  int radius, int maxSize)
    {
        cloudRadius = radius;
        sizeModX = UnityEngine.Random.Range(1, maxSize+1);
        sizeModZ = UnityEngine.Random.Range(1, maxSize+1);
        //Debug.Log(sizeModX);
        //Debug.Log(sizeModZ);

        //subClouds.Add(startCloud, cloudRadius);

        //Temp function because idk how to make it fill everything in a circle at int values
        //for (int i = 0; i < 10; i++)
        //{
        //    Vector3 spawnPos = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
        //    GameObject smolCloud = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    smolCloud.name = "Outer Cloud";
        //    smolCloud.transform.SetParent(gameObject.transform);
        //    smolCloud.transform.position = gameObject.transform.position + spawnPos;
        //}

        Debug.Log(OpenWeather.Instance.currentWeather.weather[0].description.ToUpper());
        
        if (OpenWeather.Instance.currentWeather.weather[0].main.ToUpper() == "CLOUDS"           ||
            OpenWeather.Instance.currentWeather.weather[0].main.ToUpper() == "SCATTERED CLOUDS" ||
            OpenWeather.Instance.currentWeather.weather[0].main.ToUpper() == "BROKEN CLOUDS"    ||
            OpenWeather.Instance.currentWeather.weather[0].main.ToUpper() == "OVERCAST CLOUDS"
           ) {

            //Temp function 2
            for (int x = -(sizeModX * cloudRadius); x <= sizeModX * cloudRadius; x++) {
                for (int y = CUTOFF; y <= cloudRadius; y++) {
                    for (int z = -(sizeModZ * cloudRadius); z <= sizeModZ * cloudRadius; z++) {
                        if (Math.Sqrt((x * x) / sizeModX + (y * y) + (z * z) / sizeModZ) <= cloudRadius &&
                            !(x == 0 && y == 0 && z == 0)) {
                            Vector3 spawnPos = new Vector3(x, y, z);
                            GameObject smolCloud = Instantiate(subCloudPrefab, spawnPos, Quaternion.identity);
                            smolCloud.name = "Outer Cloud";
                            smolCloud.transform.SetParent(gameObject.transform);
                            smolCloud.transform.position = gameObject.transform.position + spawnPos;
                        }
                    }
                }
            }

            GameObject[] otherClouds = GameObject.FindGameObjectsWithTag("Cloud");

            foreach (GameObject cloud in otherClouds) {
                Debug.Log(cloud.name);

                if (cloud != null && cloud.name != "Cloud") {
                    CollisionHandler colHandler = cloud.GetComponent<CollisionHandler>();
                    //colHandler.CheckCollisions();
                }
            }

            //Temp function 3
            //for (int i = 0; i < cloudRadius; i++)
            //{
            //    foreach (var subCloud in subClouds)
            //    {
            //        //Debug.Log(subCloud);
            //        //Debug.Log(cloudRadius-i);
            //        if (subCloud.Value == (cloudRadius - i))
            //        {
            //            CreateSubClouds(subCloud.Key, subCloud.Value);
            //        }
            //    }
            //}

            Debug.Log("Cloud generated");
        }
    }

    private void CreateSubClouds(GameObject startObj, int decayValue) //Loops through all neighbouring positions and generates sub clouds
    {
        int newDecay;
        bool isOverlapping = false;

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                newDecay = decayValue - UnityEngine.Random.Range(0, 0);

                if (newDecay > 0 && !(x == 0 && z == 0))
                {
                    Vector3 spawnPos = new Vector3(x, 0, z);
                    GameObject smolCloud = Instantiate(subCloudPrefab, spawnPos, Quaternion.identity);
                    smolCloud.name = "Outer Cloud";
                    smolCloud.transform.SetParent(gameObject.transform);
                    smolCloud.transform.position = startObj.transform.position + spawnPos;

                    GameObject[] otherClouds = GameObject.FindGameObjectsWithTag("Cloud");
                    foreach (GameObject cloud in otherClouds)
                    {
                        if (Vector3.Distance(cloud.transform.position, smolCloud.transform.position) < 1)
                        {
                            isOverlapping = true;
                            break;
                        }
                    }

                    if (isOverlapping == true)
                    {
                        //Deletes overlapping sub-cloud voxels to improve performance
                        GameObject.Destroy(smolCloud);
                        isOverlapping = false;
                    }
                    else
                    {
                        smolCloud.tag = "Cloud"; //Adds tag once it is clear the cloud isn't overlapping, as setting the tag before the loop makes the sub-cloud delete itself
                        //subClouds.Add(smolCloud, newDecay);
                    }
                }
            }
        }
    }
}
