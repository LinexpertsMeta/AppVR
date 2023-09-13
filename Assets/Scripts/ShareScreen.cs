using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareScreen : MonoBehaviour
{

    //public static ShareScreen instance;
    Texture2D newTexture;
    byte[] bytesArray;
    [SerializeField] private Material material;
    public bool isSharing;
    public bool startedSharing;
    public string zoneToTransmit;
    public Collider entrance;
    public Texture offTexture;

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(this);
    //    }
    //}

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        newTexture = new Texture2D(1, 1);
    }


    public void OnUpdateTextureScreen(string texture)
    {
        isSharing = true;
        bytesArray = Convert.FromBase64String(texture);        
        newTexture.LoadImage(bytesArray);
        GetComponent<Renderer>().material.mainTexture = newTexture;
        if (MetaverseSample.PlayerManager.instance.isSharing)
        {
            entrance.isTrigger = false;
        }
    }

    public void StopSharing()
    {
        isSharing = false;
        GetComponent<Renderer>().material = material;
        GetComponent<Renderer>().material.mainTexture = offTexture;
        entrance.isTrigger = true;
    }
}
