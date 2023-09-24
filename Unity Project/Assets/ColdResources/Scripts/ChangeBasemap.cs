using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeBasemap : MonoBehaviour
{

    public void ChangeMaterial(Texture basemap)
    {
        this.GetComponent<Renderer>().material.mainTexture = basemap;
    }
}