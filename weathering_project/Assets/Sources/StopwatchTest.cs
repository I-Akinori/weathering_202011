using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopwatchTest : MonoBehaviour
{
    private System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw3 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw4 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw5 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw6 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw7 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw8 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw9 = new System.Diagnostics.Stopwatch();
    // Start is called before the first frame update
    void Start()
    {
        sw5.Start();
        sw1.Start();
        int x;
        for (int i = 0; i < 1000; i++)
        {
            sw2.Start();
            x = 1 + 1;
            sw2.Stop();
        }

        sw1.Stop();

        sw3.Start();
        int y;
        for (int i = 0; i < 1000; i++)
        {
            sw4.Start();
            x = 1 + 1;
            sw4.Stop();
        }

        sw3.Stop();
        sw5.Stop();

        Debug.Log("Test: " + (100 * sw2.Elapsed.TotalMilliseconds / sw1.Elapsed.TotalMilliseconds) + " %");
        Debug.Log("Test2: " + (100 * (sw1.Elapsed.TotalMilliseconds + sw3.Elapsed.TotalMilliseconds) / sw5.Elapsed.TotalMilliseconds) + " %");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
