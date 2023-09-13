using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaverseSample
{
    public class SeatsManager : MonoBehaviour
    {
        //public static SeatsManager instance;

        ////[SerializeField] private List<ChairController> chairs;
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

        //private void Start()
        //{
        //    for(int i = 0; i < chairs.Count; i++)
        //    {
        //        chairs[i].ID = i;
        //    }
        //}


        //public void ToggleChairByID(bool state, int ID)
        //{
        //    for(int i =0; i<chairs.Count;i++)
        //    {
        //        if(chairs[i].GetInstanceID() == ID)
        //        {
        //            chairs[i].isFilled = state;
        //            if(state == true)
        //            {
        //                NetworkManager.instance.EmitSitPlayer(i);
        //            }
        //            else
        //            {
        //                NetworkManager.instance.EmitGetUpPlayer(i);
        //            }
        //        }
        //    }
        //}

        //public void ToggleChairByArray(bool state, int i)
        //{
        //    chairs[i].isFilled = state;
        //}

    }

}
