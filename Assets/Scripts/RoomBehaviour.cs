using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls; // 0-North 1-South 2-East 3-West
    public GameObject[] doors;

    //test array 
    //public bool[] testStatus;

    // Start is called before the first frame update
    //void Start()
    //{
    //    UpdateRoom(testStatus);
    //}

    // status represents all 4 directions of a room 
    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i<status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
