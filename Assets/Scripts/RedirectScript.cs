using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedirectScript : MonoBehaviour
{
    [SerializeField] private string webName;
    [SerializeField] private GameObject overlayScreen;

    private void OnMouseDown()
    {
        overlayScreen.SetActive(true);
    }

    public void SendToPage()
    {
        Application.OpenURL(webName);
    }
}
