using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{

    public static Doors instance;

    public List<GameObject> doors;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void ToggleDoor(int i, bool state)
    {
        Debug.Log("Cerro puerta");
        doors[i].SetActive(state);
    }


}
