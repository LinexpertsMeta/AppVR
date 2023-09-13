using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaverseSample
{
    public class PlayerManagerVR : MonoBehaviour
    {
        public Vector3 position;
        void Update()
        {
            if (transform.position != position && Client.instance.onLogged)
            {
                Client.instance.EmitMoveAndRotate(transform);
                Debug.Log("Sending info");
            }
            position = transform.position;
        }
    }

}
