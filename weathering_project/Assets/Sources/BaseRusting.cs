using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRusting : MonoBehaviour
{
    private Material material;
    private static int resolution = 256;
    private Texture2D RustyBoad;
    private int[,] MainRust = new int[resolution, resolution];
    private int[,] SubRust = new int[resolution, resolution];
    public float timeOut = 15.0f;
    public float p = 0.000001f;
    private float timeElapsed;
    private int Count = 0;
    private int MainN = 0;
    private int SubN = 0;

    // Use this for initialization
    void Start () {
        // 初期化時にマテリアルを保持しておく（ダイレクトにアクセスしてももちろんOK）
        this.material = GetComponent<Renderer>().material;
        
        RustyBoad = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);

        Color col = new Color(1.0f, 0.0f, 1.0f, 1.0f);   // Red/Green/Blue/Alphaの順に色指定.
        for (int y = 0; y < RustyBoad.height; y++)
        {
            for (int x = 0; x < RustyBoad.width; x++)
            {
                RustyBoad.SetPixel(x, y, col);
            }
        }
        RustyBoad.Apply();

        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
            {
                MainRust[i,j] = 0;
                SubRust[i, j] = 0;
            }
                
}

    // Update is called once per frame
    void Update () {
        timeElapsed += Time.deltaTime;

        if (true)//timeElapsed >= timeOut)
        {
            int[,] MainBuf = new int[resolution, resolution];
            int[,] SubBuf = new int[resolution, resolution];
            for (int i = 0; i < resolution; i++)
                for (int j = 0; j < resolution; j++)
                {
                    SubBuf[i,j] = SubRust[i, j];
                    MainBuf[i, j] = MainRust[i, j]; ;
                }

            for (int j = 0; j < resolution; j++)
                for (int i = 0; i < resolution; i++)
                {
                    // 主錆処理
                    if (MainBuf[i, j] == 0 || (SubBuf[i, j] > 0 && Random.value > 1.0f - p * 10f) )
                    {
                        if (Random.value > 1.0f - p)
                        {
                            MainRust[i, j] = 1;
                            MainN++ ;
                        }
                            
                    }
                    else
                    {
                        switch (Mathf.Floor(Random.value * 8.0f))
                        {
                            case 0:
                                if (i > 0 && j > 0)
                                {
                                    MainRust[i - 1, j - 1]++;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if (j - 1 + k < resolution)
                                            MainRust[i - 1, j - 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 1:
                                if (j > 0)
                                {
                                    MainRust[i, j - 1]++;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if (j - 1 + k < resolution)
                                            MainRust[i, j - 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 2:
                                if (i < resolution - 1 && j > 0)
                                {
                                    MainRust[i + 1, j - 1]++;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if (j - 1 + k < resolution)
                                            MainRust[i + 1, j - 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 3:
                                if (i > 0)
                                {
                                    MainRust[i - 1, j]++;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if (j + k < resolution)
                                            MainRust[i - 1, j + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 4:
                                if (i < resolution - 1)
                                {
                                    MainRust[i + 1, j]++;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if (j + k < resolution)
                                            MainRust[i + 1, j + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 5:
                                if (i > 0 && j < resolution - 1)
                                {
                                    MainRust[i - 1, j + 1]++;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if (j + 1 + k < resolution)
                                            MainRust[i - 1, j + 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 6:
                                if (j < resolution - 1)
                                {
                                    MainRust[i, j + 1]++;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if (j + 1 + k < resolution)
                                            MainRust[i, j + 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 7:
                            case 8:
                                if (i < resolution - 1 && j < resolution - 1)
                                {
                                    MainRust[i + 1, j + 1]++;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if (j + 1 + k < resolution)
                                            MainRust[i + 1, j + 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                    // 副錆処理
                    if (MainBuf[i, j] > 0 || SubBuf[i, j] > 0)
                    {
                        //Debug.Log(i + ", " + j + " /" + MainRust[i, j] + " /" + SubRust[i, j]);
                        SubN++;
                        switch (Mathf.Floor(Random.value * 8.0f))
                        {
                            case 0:
                                if (i > 0 && j > 0)
                                {
                                    SubRust[i - 1, j - 1]++;
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (j - 1 + k < resolution)
                                            SubRust[i - 1, j - 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 1:
                                if (j > 0)
                                {
                                    SubRust[i, j - 1]++;
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (j - 1 + k < resolution)
                                            SubRust[i, j - 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 2:
                                if (i < resolution - 1 && j > 0)
                                {
                                    SubRust[i + 1, j - 1]++;
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (j - 1 + k < resolution)
                                            SubRust[i + 1, j - 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 3:
                                if (i > 0)
                                {
                                    SubRust[i - 1, j]++;
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (j + k < resolution)
                                            SubRust[i - 1, j + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 4:
                                if (i < resolution - 1)
                                {
                                    SubRust[i + 1, j]++;
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (j + k < resolution)
                                            SubRust[i + 1, j + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 5:
                                if (i > 0 && j < resolution - 1)
                                {
                                    SubRust[i - 1, j + 1]++;
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (j + 1 + k < resolution)
                                            SubRust[i - 1, j + 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 6:
                                if (j < resolution - 1)
                                {
                                    SubRust[i, j + 1]++;
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (j + 1 + k < resolution)
                                            SubRust[i, j + 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;

                            case 7:
                            case 8:
                                if (i < resolution - 1 && j < resolution - 1)
                                {
                                    SubRust[i + 1, j + 1]++;
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (j + 1 + k < resolution)
                                            SubRust[i + 1, j + 1 + k]++;
                                        else
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                }
            for (int y = 0; y < RustyBoad.height; y++)
            {
                for (int x = 0; x < RustyBoad.width; x++)
                {
                    Color col = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                    if (MainRust[x, y] < 5)
                    {
                        if (SubRust[x, y] < 20)
                        {
                            col = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        }
                        else if (SubRust[x, y] < 40)
                            col = new Color(255.0f / 255.0f, 242.0f / 255.0f, 204.0f / 255.0f, 1.0f);
                        else if (SubRust[x, y] < 70)
                            col = new Color(255.0f / 255.0f, 188.0f / 255.0f, 0.0f, 1.0f);
                        else
                            col = new Color(191.0f / 255.0f, 144.0f / 255.0f, 0.0f, 1.0f);
                    }
                    else if (MainRust[x, y] < 25)
                        col = new Color(153.0f / 255.0f, 102.0f / 255.0f, 0.0f, 1.0f);
                    else if (MainRust[x, y] < 40)
                        col = new Color(102.0f / 255.0f, 51.0f / 255.0f, 0.0f, 1.0f);
                    else
                        col = new Color(50.0f / 255.0f, 25.0f / 255.0f, 0.0f, 1.0f);

                    RustyBoad.SetPixel(x, y, col);
                }
            }
            RustyBoad.SetPixel(127, 127, Color.red);
            RustyBoad.Apply();

            Debug.Log(++Count + ": " + MainRust[127, 127]);
            Debug.Log(MainN + " Main rust apeeared.");
            Debug.Log(SubN + " Sub rust apeeared.");
            MainN = 0; SubN = 0;

            // 経過時間に応じて値が変わるよう適当な値を入れる
            GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture = RustyBoad;

            timeElapsed = 0.0f;
        }

        // Materialクラスの`Set****`メソッドを使ってシェーダに値を送信
        //this.material.SetFloatArray("_Texture", RustyBoad);
        //this.material.SetInt("_Resolution", resolution);
    }
}
