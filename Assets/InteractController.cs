using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaverseSample
{
    public class InteractController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        ////[SerializeField] private ChairController chair;

        //private void Start()
        //{
        //    chair = GetComponent<ChairController>();
        //}
        //public void SittingChair(GameObject other)
        //{
        //    player = other;
        //    if (!chair.isFilled)
        //    {
        //        Debug.Log("Trying to interact");
        //        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        //        playerManager.thirdPersonController.enabled = false;
        //        chair.OnSitCharacter(player);
        //    }
        //}

        //public void OutChair()
        //{
        //    chair.onGetUpCharacter();
        //    chair = null;
        //    PlayerManager playerManager = player.GetComponent<PlayerManager>();
        //    playerManager.thirdPersonController.enabled = true;
        //    playerManager.onSitChair = false;
        //    player.transform.position += player.transform.forward * -1;
        //}
    }
}
