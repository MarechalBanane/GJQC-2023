using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class CryingAnalog : MonoBehaviour
{
    public string AxisName;
    public Vector2 AxisMinMax;
    public string ParameterName;
    public Vector2 ParamMinMax;
    public MainCry Cry;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (this.Cry != null)
        {
            float value = Input.GetAxis(this.AxisName);
            value = Mathf.Clamp(value, this.AxisMinMax.x, this.AxisMinMax.y);
            value = (value - this.AxisMinMax.x) / (this.AxisMinMax.y - this.AxisMinMax.x);
            value = Mathf.Lerp(this.ParamMinMax.x, this.ParamMinMax.y, value);
            this.Cry.Instance.setParameterByName(this.ParameterName, value);
        }
    }
}
