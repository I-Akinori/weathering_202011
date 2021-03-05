using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialControl : MonoBehaviour
{
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        this.material = GetComponent<Renderer>().material;

        int TexSize = 256;
        var screenShot = new Texture2D(
            TexSize,
            TexSize,
            TextureFormat.RGBA32,
            false);

        Color[] c = new Color[TexSize * TexSize];
        for (int i = 0; i < TexSize; i++)
        {
            for (int j = 0; j < TexSize; j++)
            {
                var d = new Vector2((1f * i) / TexSize - 0.5f, (1f * j) / TexSize - 0.5f).magnitude;
                if (i * TexSize + j == 10000) Debug.Log(d);
                c[i * TexSize + j] = new Color(0.5f, 0.5f, 0.5f, 1f - Mathf.Clamp(d * d * 4, 0, 1));
                //c[i * TexSize + j] = new Color(0.5f, 0.5f, 0.5f, i > 64 ? 0f : 1.0f);
            }
        }
        screenShot.SetPixels(c);
        screenShot.Apply();
        this.material.SetTexture("_ParallaxMap", screenShot);
        this.material.SetFloat("_Parallax", 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float r = Mathf.Sin(Time.time);
        float g = Mathf.Sin(Time.time) + Mathf.Cos(Time.time);
        float b = Mathf.Cos(Time.time);

        // Materialクラスの`Set****`メソッドを使ってシェーダに値を送信
        //this.material.SetColor("_Color", new Color(r, g, b, 1.0f));
    }
}
