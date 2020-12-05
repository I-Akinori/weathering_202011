using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamToggle : MonoBehaviour
{
    public static bool _cam = true;
    public Toggle vistog;

    public void ChangeCam()
    {
        _cam = !_cam;
        Debug.Log(_cam);
        //vistog.isOn = _vis;
    }
}
