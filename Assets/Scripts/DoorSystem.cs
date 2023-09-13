using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaverseSample
{
    public class DoorSystem : MonoBehaviour
    {
        public GameObject door;
        public List<GameObject> Arrows;
        public bool isOpen;

        public void OnMouseDown()
        {
            ToggleDoor();
            Debug.Log("Abrio puerta");
        }

        public void ToggleDoor()
        {
            //0 abierto
            //1 cerrado
            if (isOpen)
            {
                Arrows[1].SetActive(true);
                Arrows[0].SetActive(false);
            }
            else
            {
                Arrows[0].SetActive(true);
                Arrows[1].SetActive(false);

            }
            isOpen = !isOpen;
            NetworkManager.instance.EmitToggleGameObject(door);
        }
    }
}
