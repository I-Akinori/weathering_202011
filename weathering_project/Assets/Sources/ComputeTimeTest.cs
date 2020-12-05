using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleClass
{
    private SampleClass nextC;
    private int nextI;
    static private int num = 0;
    private int idx;

    public SampleClass() {
        idx = num++;
    }
    public int Num
    {
        set { num = value; }
        get { return num; }

    }
    public int Idx
    {
        set { idx = value; }
        get { return idx; }
    }
    public SampleClass NextC
    {
        set { nextC = value; }
        get { return nextC; }
    }
    public int NextI
    {
        set { nextI = value; }
        get { return nextI; }
    }
}
public class ComputeTimeTest : MonoBehaviour
{
    private float start_time = 0.0f;
    private float end_time = 0.0f;
    private int count = 0;
    private int execute = 10 * 1;
    private int elements = 100 * 100;
    private List<SampleClass> SCList = new List<SampleClass>();
    private SampleClass SC_0;
    private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    private void SampleMethod()
    {
        SampleClass tmpSC = SC_0;
        int Index = 0;
        do
        {
            //tmpSC = tmpSC.NextC;
            //Index = SCList.IndexOf(tmpSC);
            //Index = tmpSC.Idx;
            tmpSC = SCList[tmpSC.NextI];
            Index = tmpSC.NextI;

        } while (tmpSC != SC_0);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < elements; i++)
        {
            SCList.Add(new SampleClass());
        }
        for (int i = 0; i < elements - 1; i++)
        {
            SCList[i].NextC = SCList[i + 1];
            SCList[i].NextI = i + 1;
        }
        SCList[elements - 1].NextC = SCList[0];
        SCList[elements - 1].NextI = 0;
        SC_0 = SCList[0];

        sw.Start();
        while (count < execute)
        {
            SampleMethod();
            count++;
        }
        sw.Stop();
        Debug.Log(sw.Elapsed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
