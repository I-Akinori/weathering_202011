using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayTriangles :MonoBehaviour
{
    private void Start()
    {
        Point A = new Point();
        Point B = new Point();
        Point C = new Point();


        Triangle T = new Triangle(A, B, C);
    }

    private void Update()
    {

    }
}
