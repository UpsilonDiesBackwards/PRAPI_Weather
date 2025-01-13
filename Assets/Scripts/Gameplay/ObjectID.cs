using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectID : MonoBehaviour
{
    public int id;
    public CheckPoints checkPoints;     
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log($"Player has reached checkpoint {id}");
            ActivateCheckPoint();
            checkPoints.curCheckPoint++;
            Destroy(gameObject);
        }
    }

    private void ActivateCheckPoint()
    {
        // Debug.Log($"CheckPoint {id} complete");
    }
}
