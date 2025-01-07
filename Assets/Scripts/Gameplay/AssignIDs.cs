using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignIDs : MonoBehaviour
{

    private void Start()
    {
        ObjectID[] objects = FindObjectsOfType<ObjectID>();

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].id = i;
        }
    }
}
