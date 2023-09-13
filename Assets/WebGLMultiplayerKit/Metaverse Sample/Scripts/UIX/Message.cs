using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace MetaverseSample{

public class Message : MonoBehaviour
{

    public int id;

    public TextMeshProUGUI txtUserName;
	
	public TextMeshProUGUI txtMsg;

        public RectTransform image;

        private void Start()
        {
            image.localScale = new Vector3(image.localScale.x,txtMsg.rectTransform.localScale.y, image.localScale.z);
        }

    }
}
