using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public void CheckCollisions()
    {
        //Collider[] overlapCheck = Physics.OverlapBox(gameObject.transform.position, transform.localScale/3, Quaternion.identity);
        //Debug.Log(overlapCheck);
        //if (overlapCheck.Count() > 0){
        //    Destroy(gameObject);
        //}

        GameObject[] otherClouds = GameObject.FindGameObjectsWithTag("Cloud");
        foreach (GameObject cloud in otherClouds)
        {
            if (Vector3.Distance(cloud.transform.position, transform.position) < 1 && cloud != gameObject && cloud)
            {
                Destroy(gameObject);
            }
        }
    }
}
