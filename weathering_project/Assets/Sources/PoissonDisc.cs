using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDisc : MonoBehaviour
{
    static private float Pi = 3.1415926535f;
    public GameObject originObject;
    public Point[] SamplePoints = new Point[4096];
    static public Point[] PoissonPoints = new Point[4096 * 4];
    public static int PointN;
    public static void test() {
        //Debug.Log("TEST!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

    // Use this for initialization
    void Awake()
    {
        Debug.Log("PoissonDisc has Started.");
        float rad = 0.2f;
        float size = 10.0f;
        bool plot = false;
        bool plotted = false;
        int trial = 20;

        Vector2[] Points = new Vector2[4096 * 4];
        var Confirmed = new List<int>();
        var Possible = new List<int>();

        PointN = 0;

        Points[0] = new Vector2( size / 2,  size / 2);
        Points[1] = new Vector2(-size / 2,  size / 2);
        Points[2] = new Vector2(-size / 2, -size / 2);
        Points[3] = new Vector2( size / 2, -size / 2);
        Confirmed.Add(0); Possible.Add(0);
        Confirmed.Add(1); Possible.Add(1);
        Confirmed.Add(2); Possible.Add(2);
        Confirmed.Add(3); Possible.Add(3);

        PointN = 4;

        while (Possible.Count > 0)                                                  // 可能性のあるリストがある限りループ
        {
            for (int i = 0; i < trial; i++) {
            var Pivot = Points[Possible[Possible.Count - 1]];                       // 基準点は可能性のあるリストの最後尾
                var r = Random.Range(rad, rad * 2);                                     // 基準点からの距離は rad ~ 2rad
                var s = Random.Range(0.0f, 360.0f);                                     // 方向
                Points[PointN] = new Vector2(Mathf.Cos(s), Mathf.Sin(s)) * r + Pivot;   // とりあえず登録（不適合なら上書きされる）
                plotted = false;

                if (Points[PointN].x >  size / 2) Points[PointN].x =  size / 2;
                if (Points[PointN].x < -size / 2) Points[PointN].x = -size / 2;
                if (Points[PointN].y >  size / 2) Points[PointN].y =  size / 2;
                if (Points[PointN].y < -size / 2) Points[PointN].y = -size / 2;
                
                if (Mathf.Abs(Points[PointN].x) > size / 2 || Mathf.Abs(Points[PointN].y) > size / 2)   // 塗料の外なら計算し直し, ただし最終ループではcontinueしない
                {
                    if (i == trial - 1)
                    {
                                                                                                // 可能性リストからpivotを削除
                        Possible.RemoveAt(Possible.Count - 1);
                    }
                    continue;
                }
                
                for (int j = 0; j < Confirmed.Count; j++)                                       // 頂点候補と確定頂点の距離比較
                {
                    if (Vector2.Distance(Points[PointN], Points[Confirmed[j]]) < rad )
                        break;
                    if (j == Confirmed.Count - 1) plot = true;                                      // 最後まで近い頂点が見つからなければ plot
                }

                if (plot)                                                       // plot: 確定リスト，可能性リストに追加，頂点数を更新(上書を防止)
                {
                    Confirmed.Add(PointN); Possible.Add(PointN);
                    PointN++;
                    plot = false;
                    plotted = true;
                }

                if (plotted == false && i == trial - 1)                            // 10回試してもplotされなかったら周囲に新たな頂点が生成される可能性なし
                {             // 可能性リストからpivotを削除
                    Possible.RemoveAt(Possible.Count - 1);
                }
            }
        }

        Point[] TestPoints = new Point[4096 * 4];

        // CubeプレハブをGameObject型で取得
        GameObject obj = (GameObject)Resources.Load("Sphere");
        // Cubeプレハブを元に、インスタンスを生成、
        for (int i = 0; i < PointN; i++)
        {
            PoissonPoints[i] = new Point(Points[Confirmed[i]].x, Points[Confirmed[i]].y);
            //TestPoints[i].Draw();
            //SamplePoints[i].SetPosition(Points[Confirmed[i]].x, Points[Confirmed[i]].y);
            //Instantiate(originObject, new Vector3(Points[Confirmed[i]].x, Points[Confirmed[i]].y, 0.0f), Quaternion.identity);
        }
    }

}