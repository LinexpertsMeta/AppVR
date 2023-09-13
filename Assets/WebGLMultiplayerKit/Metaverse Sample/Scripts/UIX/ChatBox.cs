using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MetaverseSample
{
    public class ChatBox : MonoBehaviour
    {

        public string id;

        public string guest_id;

        public string host_id;

        public Image profileImage;

        public TextMeshProUGUI profileName;

        public TextMeshProUGUI txtMsg;

        public GameObject myMessagePrefab; // set in inspector. stores the user Prefab message game object

        public GameObject networkMessagePrefab;  // set in inspector. stores the network user message game object

        ArrayList messages; // list to store all messages

        public GameObject contentMessages; // set in inspector. stores the content messages game object


        [HideInInspector]
        public int countMessages; //variable for controlling the number of messages on the screen

        [HideInInspector]
        public int maxDeleteMessage; //variable for controlling the number of messages on the screen

        [HideInInspector]
        public int currentAvatar; // flag to mark the chosen avatar

        public TMP_InputField inputFieldPrivateMessage;

        public Event evento;

        // Start is called before the first frame update
        void Start()
        {
            messages = new ArrayList();
        }

        private void Update()
        {
            if (inputFieldPrivateMessage.isFocused)
            {
                PlayerManager.instance.thirdPersonController.enabled = false;
                PlayerManager.instance.hasToUpdateAnimation = true;
            }
            else
            {
                PlayerManager.instance.thirdPersonController.enabled = true;
                PlayerManager.instance.hasToUpdateAnimation = false;
            }
        }


        public void SpawnMyMessage(string _message)
        {

            Debug.Log("try to spawn my message");
            countMessages += 1;

            GameObject newMessage = Instantiate(myMessagePrefab) as GameObject;
            newMessage.name = countMessages.ToString();
            newMessage.GetComponent<Message>().txtUserName.text = PlayerManager.instance.name;
            newMessage.GetComponent<Message>().txtMsg.text = _message;
            newMessage.transform.parent = contentMessages.transform;
            newMessage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            messages.Add(newMessage);
            Debug.Log(" my message spawned");
            //if (messages.Count > 7)
            //{
            //    ArrayList deleteMessages = new ArrayList();

            //    int j = 0;

            //    foreach (GameObject msg in messages)
            //    {
            //        if (j <= maxDeleteMessage)
            //        {
            //            deleteMessages.Add(msg);
            //        }
            //        j += 1;

            //    }

            //    foreach (GameObject msg in deleteMessages)
            //    {
            //        Destroy(msg);
            //        messages.Remove(msg);
            //    }

            //}

        }

        public void SpawnNetworkMessage(string userID, string _message)
        {

            Debug.Log("try to spawn network message");
            countMessages += 1;

            GameObject newMessage = Instantiate(networkMessagePrefab) as GameObject;
            newMessage.name = countMessages.ToString();
            newMessage.GetComponent<Message>().id = countMessages;
            newMessage.GetComponent<Message>().txtUserName.text = userID;
            newMessage.GetComponent<Message>().txtMsg.text = _message;
            newMessage.transform.parent = contentMessages.transform;
            newMessage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            messages.Add(newMessage);
            Debug.Log(" network message spawned");

            //if (messages.Count > 7)
            //{
            //    int j = 0;

            //    foreach (Message msg in messages)
            //    {
            //        if (j == 0)
            //        {

            //            Destroy(GameObject.Find(msg.id.ToString()));
            //            messages.Remove(msg);

            //            break;
            //        }
            //        j += 1;

            //    }
            //}
        }

        public void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(inputFieldPrivateMessage.text))
            {
                return;
            }
            CanvasManager.instance.inputFieldPrivateMessage = inputFieldPrivateMessage;
            NetworkManager.instance.EmitPrivateMessage(txtMsg.text, id, guest_id,host_id);
        }


        public void CloseChatBox()
        {
            CanvasManager.instance.CloseChatBox(this);
        }

    }
}
