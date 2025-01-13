using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    //Variables
    public GameObject[] checkPoints;
    public int curCheckPoint;

    public int chechPointsInLevel;

    public GameObject nextLevelScreen;

    public void Start()
    {
        //Assign an id to each checkpoint
        for(int i = 0; i < checkPoints.Length; i++)
        {
            if (checkPoints[i].TryGetComponent(out ObjectID objectID))
            {
                objectID.id = i;
            }
            else
            {
                Debug.LogWarning($"GameObject {checkPoints[i].name} is missing an ID");
            }
        }

        chechPointsInLevel = checkPoints.Length;
    }

    private void Update()
    {
        if(curCheckPoint >= chechPointsInLevel)
        {
            Cursor.lockState = CursorLockMode.None;
            nextLevelScreen.SetActive(true);
        }
    }
    
    // Cleans up array list NOT WORKING CURRENTLY!!!
    public void RemoveCompetedCheckPoints()
    {
        checkPoints = checkPoints.Where(i => i != null).ToArray();
    }


}
