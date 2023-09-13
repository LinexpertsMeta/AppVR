using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public string id;
    public string name;
    public string publicAddress;
    public string model;
    public string posX;
    public string posY;
    public string posZ;
    public string rotation;

    public PlayerInfo(string name, string publicAddress, string model, string posX, string posY, string posZ)
    {
        this.name = name;
        this.publicAddress = publicAddress;
        this.model = model;
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;
    }
    public PlayerInfo(string posX, string posY, string posZ, string rotation)
    {
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;
    }

    public string InfoPlayer()
    {
        return name + ":" + publicAddress + ":" + model + ":" + posX + ":" + posY + ":" + posZ;
    }
}
