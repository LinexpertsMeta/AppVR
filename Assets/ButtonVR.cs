using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonVR : MonoBehaviour
{
    public GameObject bloomObject;    

    private void OnMouseEnter()
    {
        bloomObject.SetActive(true);        
    }

    private void OnMouseExit()
    {
        bloomObject.SetActive(false);        
    }
}
