using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnterEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent enterEvent;

    public void StartEvent()
    {
        enterEvent.Invoke();
    }    
}
