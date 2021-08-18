using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyUIElement : MonoBehaviour
{
    public UnityEvent onClick;
    public UnityEvent onUp;

    public void Trigger() {
        onClick.Invoke();
    }

    public void OnUp() {
        onUp.Invoke();
    }
}
