using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIhandler : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {

    }

    public void OnClick()
    { // 必ず public にする
        Debug.Log("clicked");
    }
}