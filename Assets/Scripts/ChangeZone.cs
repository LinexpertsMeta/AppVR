//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MetaverseSample
//{
//    public class ChangeZone : MonoBehaviour
//    {
//        public string zoneName;
//        private void OnTriggerEnter(Collider other)
//        {
//            if (other.CompareTag("Player"))
//            {
//                //if (zoneName == "Central")
//                //{
//                //    List<ShareScreen> screens = ScreensManager.instance.screens;
//                //    foreach (ShareScreen s in screens)
//                //    {
//                //        if (s.zoneToTransmit == NetworkManager.instance.currentZone)
//                //        {
//                //            s.StopSharing();
//                //            NetworkManager.instance.EmitShareScreenOFF();
//                //        }
//                //    }
//                //}
//                NetworkManager.instance.EmitChangeZone(zoneName);
//            }
//        }
//    }
//}
