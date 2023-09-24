using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class Anchorer : MonoBehaviour
{
    private RectTransform rectTransform;

    public void Anchor(RectTransform otherTransform)
    {
        this.rectTransform.position = otherTransform.position;
    }

    private void Start()
    {
        this.rectTransform = this.gameObject.GetComponent<RectTransform>();
    }
}
