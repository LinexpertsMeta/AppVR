using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AvatarSelector : MonoBehaviour
{
    public static AvatarSelector instance;



    public List<GameObject> avatarsModelsHeads, avatarsModelsHairs, avatarsModelsBodys;

    public int indexSelectorHead, indexSelectorHair, indexSelectorBody, indexSelectorColor;

    public TextMeshProUGUI text;

    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        indexSelectorBody = 0;
        indexSelectorHair = 0;
        indexSelectorHead = 0;
        indexSelectorColor = 0;
        //avatarsModelsBodys[indexSelectorBody].SetActive(true);
        //SelectHair(indexSelectorHair);
        //SelectHead(indexSelectorHead);
    }


    public void SelectHair(int i)
    {
        int number = i;
        avatarsModelsBodys[indexSelectorBody].GetComponent<ShowAspects>().SetModelHair(number);
        indexSelectorHair = number;
    }

    public void SelectHead(int i)
    {
        int number = i;
        avatarsModelsBodys[indexSelectorBody].GetComponent<ShowAspects>().SetModelHead(number);
        indexSelectorHead = number;
        SelectColor(indexSelectorColor);
    }

    public void SelectColor(int i)
    {
        int number = i;        
        avatarsModelsBodys[indexSelectorBody].GetComponent<ShowAspects>().
            SetModelColor(indexSelectorHead,number,indexSelectorBody);
        indexSelectorColor = number;
    }

    public void SelectBody(int i)
    {
        int number = i;
        avatarsModelsBodys[indexSelectorBody].SetActive(false);
        avatarsModelsBodys[number].SetActive(true);
        avatarsModelsBodys[number].GetComponent<ShowAspects>().SetModelHead(indexSelectorHead);
        avatarsModelsBodys[number].GetComponent<ShowAspects>().SetModelHair(indexSelectorHair);
        indexSelectorBody = number;
    }
}
