using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace MetaverseSample{
public class UserOptions : MonoBehaviour
{

    public TextMeshProUGUI userName;

    public string id;

    public string userPublicAddress;

    public TMP_InputField if_myPublicAdrr;

    public TMP_InputField if_balance;

	public TMP_InputField if_amount;
    
    public TMP_InputField if_publicAdrrTo;

    public TMP_InputField if_transHash;

    public void OpenPrivateChat()
    {
            Debug.Log(id);
            ChatBox[] chats = GameObject.FindObjectsByType<ChatBox>(FindObjectsSortMode.None);
            foreach (ChatBox chat in CanvasManager.instance.chatBoxs)
            {
                if (((chat.host_id == NetworkManager.instance.local_player_id && chat.guest_id == id) 
                    ||(chat.host_id == id && chat.guest_id == NetworkManager.instance.local_player_id)))
                {
                    Debug.Log("chat.host_id:" + chat.host_id);
                    Debug.Log("chat.guest_id:" + chat.guest_id);
                    chat.gameObject.SetActive(true);
                    return;
                }
            }
            NetworkManager.instance.EmitOpenChatBox(id);

    }

    public void OpenTransactionPanel()
    {


        CanvasManager.instance.transactionPanel.SetActive(true);

        if_myPublicAdrr.text = CanvasManager.instance.myPublicAdrr;
        if_publicAdrrTo.text = userPublicAddress;
        if_balance.text =  CanvasManager.instance.balance;
         




    }

    public void CloseTransactionPanel()
    {

        CanvasManager.instance.transactionPanel.SetActive(false);

    }
}
}
