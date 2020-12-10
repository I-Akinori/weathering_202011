using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateTriangle4 : MonoBehaviour
{
    private GameObject obj; // デバッグ用のポインタ

    private Transform _camTransform;
    private float timeOut = 0.1f;
    private int count = 0;
    private bool firstloop = true;
    public static System.Diagnostics.Stopwatch sw0 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw3 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw4 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw5 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw6 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw7 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw8 = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sw9 = new System.Diagnostics.Stopwatch();

    [SerializeField]
    private GameObject m_object = null;
    [SerializeField]
    private Material _material;
    [SerializeField]
    private Material _submat;
    [SerializeField]
    private Material _normal;
    [SerializeField]
    private Material _basemat;
    private float _speed = 0.15f;
    private float _Aspeed = 1f;
    private float _Pspeed = 1f;
    private float _Rspeed = 2f;
    private float _Dspeed = 0.3f;
    private bool _simulating = true;
    private Vector3 _gravity = new Vector3(0.0f, -1.0f, 0.0f);
    private float _peelangle = Mathf.PI * 2;

    [SerializeField]
    private uint _seed = 1000;

    private Mesh _mesh;
    private Mesh _Bmesh;

    private static int resolution = 64;                            // テクスチャ生成お試し

    // (1) 頂点座標（この配列のインデックスが頂点インデックス）
    private List<Vertex> _vertices = new List<Vertex>();

    private Vector3[] _positions = new Vector3[]{                                                                    // position, normal, triangles, uvsは配列　ほかはリスト
    
    };
    private Vector3[] _Bpositions;

    // (2) ポリゴンを形成する頂点インデックスを順番に指定する
    private List<TPolygon> _tpolygons = new List<TPolygon>();
    private List<HalfEdge> _halfedges = new List<HalfEdge>();

    private int[] _triangles = new int[] { 0, 1, 2 };
    private int[] _Btriangles = new int[] { };

    private List<Vector3> _BposList = new List<Vector3>();
    private List<Vector3> _BnorList = new List<Vector3>();
    private List<Vector3> _BuvList = new List<Vector3>();
    private List<Vector3Int> _BtriList = new List<Vector3Int>();
    private List<int> _edgeList = new List<int>();

    // (3) 法線
    private Vector3[] _normals = new Vector3[]{
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1)
    };
    private Vector3[] _Bnormals;

    // uv座標
    private Vector2[] _uvs = new Vector2[]{
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
    };
    private Vector2[] _Buvs;

    // uv座標
    private Color[] _colors = new Color[]{
        Color.white,
        Color.blue,
        Color.red,
    };
    private Color[] _Bcolors;

    private float Dist2BetweenPointAndLine(Vector3 P, Vector3 Q, Vector3 R)
    {
        Vector3 u = P - Q;
        Vector3 v = R - Q;
        float uu = Vector3.Dot(u, u);
        float vv = Vector3.Dot(v, v);
        float uv = Vector3.Dot(u, v);
        return (vv - uv * uv / uu);
    }

    // ##############################################################################################################################################################
    // ##############################################################################################################################################################
    private void Awake()
    {
        obj = (GameObject)Resources.Load("Point");

        sw0.Start();
        _camTransform = GameObject.Find("Main Camera").transform;
        // 各リストの初期化

        _mesh = new Mesh();
        _Bmesh = new Mesh();


        // Objファイル読込み start

        for (int i = 0; i < LoadText.Positions.Count; i++)
        {
            _vertices.Add(new Vertex(LoadText.Positions[i], LoadText.VNormals[i], LoadText.ArrangedUVs[i]));
            _BposList.Add(LoadText.Positions[i]);
            _BnorList.Add(LoadText.VNormals[i]);
            _BuvList.Add(LoadText.ArrangedUVs[i]);
        }
        for (int i = 0; i < LoadText.Polygons.Count; i++)
        {
            _BtriList.Add(LoadText.Polygons[i]);
            _halfedges.Add(new HalfEdge(_vertices[LoadText.Polygons[i].x]));
            _halfedges.Add(new HalfEdge(_vertices[LoadText.Polygons[i].y]));
            _halfedges.Add(new HalfEdge(_vertices[LoadText.Polygons[i].z]));

            _halfedges[_halfedges.Count - 3].Next = _halfedges[_halfedges.Count - 2];
            _halfedges[_halfedges.Count - 2].Prev = _halfedges[_halfedges.Count - 3];
            _halfedges[_halfedges.Count - 2].Next = _halfedges[_halfedges.Count - 1];
            _halfedges[_halfedges.Count - 1].Prev = _halfedges[_halfedges.Count - 2];
            _halfedges[_halfedges.Count - 1].Next = _halfedges[_halfedges.Count - 3];
            _halfedges[_halfedges.Count - 3].Prev = _halfedges[_halfedges.Count - 1];

            _tpolygons.Add(new TPolygon(_halfedges[_halfedges.Count - 3]));
            _halfedges[_halfedges.Count - 3].Face = _tpolygons[_tpolygons.Count - 1];
            _halfedges[_halfedges.Count - 2].Face = _tpolygons[_tpolygons.Count - 1];
            _halfedges[_halfedges.Count - 1].Face = _tpolygons[_tpolygons.Count - 1];
        }


        // Objファイル読込み end
        /*
        for (int i = _tpolygons.Count - 1; i >= 0; i--)
        {

            TPolygon TP = _tpolygons[i];

            if (TP.V1 < 3 || TP.V2 < 3 || TP.V3 < 3)        // 初期３点を含むポリゴンの削除 ドロネー分割のみ必要
            {
                // _tpolygons.RemoveAt(i);
            }
        }
        Debug.Log("After Count: " + _tpolygons.Count);
        */
        SetHalfEdge();
        SetPairHalfEdge();

        for (int i = 0; i < _halfedges.Count; i++)
        {
            HalfEdge HEi = _halfedges[i];

            if (HEi.Pair == null) // 簡易曲率計算 & かける揚力計算
            {
                HEi.LiftingForce = 0.12f * HEi.Face.ContractionForce * Dist2BetweenPointAndLine(HEi.Prev.Vert.Pos, HEi.Vert.Pos, HEi.Next.Vert.Pos) * 2.0f;
            }
            else
            {
                bool convex = Vector3.Dot(HEi.Prev.Vert.Nor - HEi.Pair.Prev.Vert.Nor, HEi.Prev.Vert.Pos - HEi.Pair.Prev.Vert.Pos) > 0;
                Vector3 mid = (HEi.Vert.Pos + HEi.Next.Vert.Pos) / 2.0f;
                //convex = Vector3.Dot(_vertices[HEi.OppositeVertex].Pos + _vertices[_halfedges[HEi.Pair].OppositeVertex].Pos - 2.0f * mid, _vertices[HEi.End1].Nor + _vertices[HEi.End2].Nor) < 0;

                float rad = Vector3.Dot(HEi.Prev.Vert.Nor.normalized, HEi.Pair.Prev.Vert.Nor.normalized);
                float d = Mathf.Clamp(rad, -1.0f, 1.0f);
                HEi.Curvature = (convex ? Mathf.PI + Mathf.Acos(d) : Mathf.PI - Mathf.Acos(d)) - _peelangle;
                HEi.LiftingForce = 0.12f * HEi.Face.ContractionForce * Dist2BetweenPointAndLine(HEi.Prev.Vert.Pos, HEi.Vert.Pos, HEi.Next.Vert.Pos)
                //    * (convex ? 10 : 0);
                * HEi.Curvature;

            }
        }

        //for (int i = 0; i < _tpolygons.Count; i++)
        //    _tpolygons[i].ReloadTriangles(_tpolygons);

        foreach (Vertex V in _vertices)
            V.ReloadNearbyVertices();
        for (int i = 0; i < _tpolygons.Count; i++)
            _tpolygons[i].ReloadNormal();

        sw0.Stop();
    }

    private void SetHalfEdge() // 頂点ごとの半稜線のセッティング
    {
        Debug.Log("Set Vertex's HalfEdge.");
        for (int i = 0; i < _vertices.Count; i++)
        {
            for (int j = 0; j < _halfedges.Count; j++)
            {
                if (_vertices[i] == _halfedges[j].Vert)
                {
                    _vertices[i].HE = _halfedges[j];
                    break;
                }
            }
            if (_vertices[i].HE == null) Debug.Log("Error2: " + i);
        }
    }
    private void SetPairHalfEdge() // 半稜線のペアセッティング
    {
        Debug.Log("Set Pair.");
        for (int i = 0; i < _halfedges.Count; i++)
        {
            if (_halfedges[i].Pair != null) continue;
            for (int j = 0; j < _halfedges.Count; j++)
            {
                if (j == i) continue;
                if (_halfedges[i].Next.Vert == _halfedges[j].Vert && _halfedges[j].Next.Vert == _halfedges[i].Vert)
                {
                    _halfedges[i].Pair = _halfedges[j];
                    _halfedges[j].Pair = _halfedges[i];
                    break;
                }
            }
        }
    }
    private void ReloadVertexIndex()
    {
        for (int i = 0; i < _vertices.Count; i++)
        {
            _vertices[i].Idx = i;
        }
    }
    private void ReloadHalfEdgeIndex()
    {
        for (int i = 0; i < _halfedges.Count; i++)
        {
            _halfedges[i].Idx = i;
        }
    }
    private void ReloadFaceIndex()
    {
        for (int i = 0; i < _tpolygons.Count; i++)
        {
            _tpolygons[i].Idx = i;
        }
    }
    // ##############################################################################################################################################################
    // ##############################################################################################################################################################
    private void Update()
    {
        sw1.Start();
        // while (count < 1)
        if (count < 550 || true)
        { // 計算時間測定用ループ

            float slider = SimulationSpeedSlider.SliderValue;
            //slider = 1.0f;
            // インタラクティブ
            if (Input.GetMouseButton(0) && !CamToggle._cam)
            {
                Vector2 touchScreenPosition = Input.mousePosition;

                touchScreenPosition.x = Mathf.Clamp(touchScreenPosition.x, 0.0f, Screen.width);
                touchScreenPosition.y = Mathf.Clamp(touchScreenPosition.y, 0.0f, Screen.height);

                Camera gameCamera = Camera.main;
                Ray touchPointToRay = gameCamera.ScreenPointToRay(touchScreenPosition);
                Vector3 v;
                for (int i = 0; i < _vertices.Count; i++)
                {
                    v = _vertices[i].Pos - touchPointToRay.origin;
                    if (Vector3.Dot(v, v) - Mathf.Pow(Vector3.Dot(v, touchPointToRay.direction), 2) / Vector3.Dot(touchPointToRay.direction, touchPointToRay.direction) < 0.02f * Mathf.Pow(1.5f, DrawingRangeSlider.DrawingRange))
                    {
                        //if (Input.GetKey(KeyCode.D)) // de-wetharing
                        if (!WeaToggle._wea)
                            _vertices[i].Bond += (100f - _vertices[i].Bond) * 0.05f;
                        else
                            _vertices[i].Bond += -_vertices[i].Bond * 0.05f;
                    }
                }

                Debug.Log("Extent: " + DrawingRangeSlider.DrawingRange);
            }

            if (Input.GetKeyDown(KeyCode.P)) // simulation　ストップ/ スタート
                _simulating = !_simulating;

            sw3.Start();

            for (int i = 0; i < _halfedges.Count; i++)
            {
                HalfEdge HEi = _halfedges[i];

                if (HEi.Pair == null) // 簡易曲率計算 & かける揚力計算
                {
                    HEi.LiftingForce = 0.12f * HEi.Face.ContractionForce * Dist2BetweenPointAndLine(HEi.Prev.Vert.Pos, HEi.Vert.Pos, HEi.Next.Vert.Pos) * 2.0f;
                }
                else
                {
                    bool convex = Vector3.Dot(HEi.Prev.Vert.Nor - HEi.Pair.Prev.Vert.Nor, HEi.Prev.Vert.Pos - HEi.Pair.Prev.Vert.Pos) > 0;
                    Vector3 mid = (HEi.Vert.Pos + HEi.Next.Vert.Pos) / 2.0f;
                    //convex = Vector3.Dot(_vertices[HEi.OppositeVertex].Pos + _vertices[_halfedges[HEi.Pair].OppositeVertex].Pos - 2.0f * mid, _vertices[HEi.End1].Nor + _vertices[HEi.End2].Nor) < 0;

                    float rad = Vector3.Dot(HEi.Prev.Vert.Nor.normalized, HEi.Pair.Prev.Vert.Nor.normalized);
                    float d = Mathf.Clamp(rad, -1.0f, 1.0f);
                    HEi.Curvature = (convex ? Mathf.PI + Mathf.Acos(d) : Mathf.PI - Mathf.Acos(d)) - _peelangle;
                    HEi.LiftingForce = 0.12f * HEi.Face.ContractionForce * Dist2BetweenPointAndLine(HEi.Prev.Vert.Pos, HEi.Vert.Pos, HEi.Next.Vert.Pos)
                    * HEi.Curvature;
                }
            }

            _peelangle *= Mathf.Pow(0.999f, _Aspeed * _speed * slider * 10f);

            //Debug.Log("Speed2: " + _speed);
            //Debug.Log("PeelAngle2: " + _peelangle);

            // 揚力計算
            for (int i = 0; i < _vertices.Count; i++)
                _vertices[i].LiftingForce = 0.0f;
            for (int i = 0; i < _halfedges.Count; i++)
            {
                if (!_halfedges[i].Face.Peeled)
                    _halfedges[i].Prev.Vert.LiftingForce += _halfedges[i].LiftingForce;
            }
            sw3.Stop();

            sw4.Start();
            float[] RustBuf = new float[_vertices.Count];
            for (int i = 0; i < _vertices.Count; i++)
                RustBuf[i] = _vertices[i].Rust;
            sw4.Stop();

            for (int i = 0; i < _vertices.Count && _simulating; i++)                                // 頂点ごとの操作
            {
                sw2.Start(); // Dust 時間計測 start
                _vertices[i].Dust += Mathf.Max(0, 1.0f - (_vertices[i].Pos.y + 5.0f) / 10.0f) * _Dspeed * _speed * slider
                    * (0.5f - 0.5f * Vector3.Dot(_gravity, _vertices[i].Nor)) * (1 - _vertices[i].HE.Curvature / (2 * Mathf.PI)) * 5.0f;
                sw2.Stop();  // Dust 時間計測 end


                if (_vertices[i].Status > 4) continue;                                                 // 子頂点は除く

                sw3.Start(); // Separate 時間計測 start
                Vertex Vi = _vertices[i];
                if (Vi.Status == 0)
                {
                    Vi.Bond += -_Pspeed * _speed * slider * (1 - Mathf.Pow(0.9f, Vi.PeelScale + 1.0f));

                    if (Vi.Bond < Vi.LiftingForce)                                                  // 分離処理
                    {
                        Vi.Pos += Vi.Nor * 0.1f;                                                    // 頂点移動処理 2020/12/05
                        //Debug.Log("Separated!");
                        Vi.PeelScale++;
                        Vi.Status = 1;

                        for (int j = 0; j < _halfedges.Count; j++)                                  // 剥離規模パラメタの更新
                        {
                            HalfEdge HEj = _halfedges[j];
                            if (HEj.Vert == Vi && HEj.Next.Vert.PeelScale < Vi.PeelScale) HEj.Next.Vert.PeelScale = Vi.PeelScale;
                            if (HEj.Next.Vert == Vi && HEj.Vert.PeelScale < Vi.PeelScale) HEj.Vert.PeelScale = Vi.PeelScale;
                        }
                    }
                }
                else
                {
                }

                sw3.Stop(); // separate 時間計測 end

                if (!firstloop && _vertices[i].Status > 0 && i < _Bcolors.Length)
                    _Bcolors[i].r += (1.0f - _Bcolors[i].r) * 0.2f * _speed * slider;                              // 基板の錆 2020/05/09

                sw4.Start(); // rust 時間計測 start
                if (Vi.Rust >= 10.0f && Vi.NVS.Count > 0)                                                                    // Rust拡散　探索をもっと容易にする余地あり
                {
                    Vi.Rust = Vi.Rust > 100f ? Vi.Rust : Vi.Rust + 0.03f * 15.0f * _Rspeed * _speed * slider;

                    Vector3 projectedgrav = Vector3.Normalize(_gravity - Vector3.Dot(Vi.Nor, _gravity) * Vi.Nor);
                    Vertex MaxVert = Vi.NVS[0];
                    float MaxValue = 0.0f;
                    float dotproduct = 0.0f;
                    float thre = 0.7f;
                    float ratio = 0.001f;

                    HalfEdge tmpHE = Vi.HE;
                    for (int j = 0; j < Vi.NVS.Count; j++)
                    {
                        if (Vi.NVS[j].Status > 4) continue; // 子頂点なら伝搬しない
                        dotproduct = Vector3.Dot(Vector3.Normalize(Vi.NVS[j].Pos - Vi.Pos), projectedgrav);
                        if (dotproduct > thre)
                        {
                            tmpHE.Prev.Vert.Rust += (dotproduct - thre) * RustBuf[i] * 0.15f * ratio * _Rspeed * _speed * slider;
                        }

                        if (dotproduct > MaxValue)
                        {
                            MaxVert = Vi.NVS[j];
                            MaxValue = dotproduct;
                        }
                    }

                    if (MaxValue > thre)
                    {
                        MaxVert.Rust += MaxValue * RustBuf[i] * 0.15f * _Rspeed * _speed * slider;
                    }

                }
                sw4.Stop(); // rust 時間計測 end
            }

            Array.Resize(ref _triangles, _tpolygons.Count * 3);

            sw5.Start();

            for (int i = 0; i < _tpolygons.Count; i++)  // 三角ポリゴンごとの処理
            {
                TPolygon TPi = _tpolygons[i];
                //HalfEdge TPiHE = TPi.HE;
                //Vertex V1 = TPiHE.Vert;
                //Vertex V2 = TPiHE.Next.Vert;
                //Vertex V3 = TPiHE.Prev.Vert;

                TPi.ContractionForce += _speed * slider * 1f;
                /*
                if (V1.Status > 0 && V2.Status > 0 && V3.Status > 0 && TPi.Peeled == false) // 剥離の処理 ひび割れ処理追加のため無視 (2019/12/18)
                {
                    TPi.Peeled = true;

                    ////////////////////////////////////////////////////////// 頂点移動

                    V1.Pos += V1.Nor * 0.05f;  // teapot4 0.05f, other 0.1f
                    V2.Pos += V2.Nor * 0.05f;
                    V3.Pos += V3.Nor * 0.05f;

                    TPi.ReloadNormal();

                    //////////////////////////////// 最も下にある頂点はRust上昇 
                    int target = 1;
                    float tmp;
                    float value = 0.0f;
                    Vector3 G = (V1.Pos + V2.Pos + V3.Pos) / 3.0f;
                    if ((tmp = Vector3.Dot(V1.Pos - G, _gravity)) > value)
                    {
                        target = 1;
                        value = tmp;
                    }
                    if ((tmp = Vector3.Dot(V2.Pos - G, _gravity)) > value)
                    {
                        target = 2;
                        value = tmp;
                    }
                    if ((tmp = Vector3.Dot(V3.Pos - G, _gravity)) > value)
                    {
                        target = 3;
                        // value = tmp;
                    }

                    switch (target)
                    {
                        case 1:
                            V1.Rust = V1.Rust > 100f ? V1.Rust : V1.Rust + 10.00f;
                            break;
                        case 2:
                            V2.Rust = V2.Rust > 100f ? V2.Rust : V2.Rust + 10.00f;
                            break;
                        case 3:
                            V3.Rust = V3.Rust > 100f ? V3.Rust : V3.Rust + 10.00f;
                            break;
                    }
                    //////////////////////////

                    for (int j = 0; j < _tpolygons.Count; j++)
                    {
                        if (i == j) continue;

                        TPolygon TPj = _tpolygons[j];
                        Vertex U1 = TPj.HE.Vert;
                        Vertex U2 = TPj.HE.Next.Vert;
                        Vertex U3 = TPj.HE.Prev.Vert;
                        if (V1 == U1 ||                                                // 頂点1を共有するなら
                            V2 == U1 ||
                            V3 == U1 ||
                            V1 == U2 ||                                                // 頂点2を共有するなら
                            V2 == U2 ||
                            V3 == U2 ||
                            V1 == U3 ||                                                // 頂点3を共有するなら
                            V2 == U3 ||
                            V3 == U3
                         ) TPj.ReloadNormal();
                    }
                }*/
            }


            for (int i = 0; i < _halfedges.Count; i++)  // ハーフエッジごとの処理
            {
                HalfEdge HEi = _halfedges[i];
                HalfEdge HEp = HEi.Pair == null ? HEi : HEi.Pair;
                TPolygon HEiP = HEi.Face;
                TPolygon HEpP = HEp.Face;

                float HEiPCF = HEiP.ContractionForce;
                float HEpPCF = HEpP.ContractionForce;
                float HEiBF = HEi.BindingForce;

                if (HEi.Next.Vert != HEp.Vert && HEi != HEp && HEi.Connected) {
                    // プレハブを元にオブジェクトを生成する
                     Instantiate(obj, HEi.Vert.Pos, Quaternion.identity);
                     Instantiate(obj, HEi.Next.Vert.Pos, Quaternion.identity);
                     Instantiate(obj, HEi.Pair.Vert.Pos, Quaternion.identity);
                    Debug.Log("HEi.Vert: (" + HEi.Vert.Pos.x + ", " + HEi.Vert.Pos.y + ", " + +HEi.Vert.Pos.z + ")");
                    Debug.Log("HEi.Next.Vert: (" + HEi.Next.Vert.Pos.x + ", " + HEi.Next.Vert.Pos.y + ", " + +HEi.Next.Vert.Pos.z + ")");
                    Debug.Log("HEi.Pair.Vert: (" + HEi.Pair.Vert.Pos.x + ", " + HEi.Pair.Vert.Pos.y + ", " + +HEi.Pair.Vert.Pos.z + ")");
                }

                if ((HEiP.ContractionForce + HEpP.ContractionForce > HEi.BindingForce * 5f                                 // 張力と接合力の比較
                        || (HEiP.ContractionForce + HEpP.ContractionForce > HEi.BindingForce * 0.1f                      // 割れやすい
                            && ((HEi.Vert.Status == 2 && Vector3.Dot(HEi.Vert.Direction, Vector3.Normalize(HEi.Next.Vert.Pos - HEi.Vert.Pos)) > 0.5f)
                                || HEi.Next.Vert.Status == 2 && Vector3.Dot(HEi.Next.Vert.Direction, Vector3.Normalize(HEi.Vert.Pos - HEi.Next.Vert.Pos)) > 0.5f))
                    )                                                                                         // ↑ エッジの端点がひびの端点だったら
                    && HEi.Vert.Status > 0 && HEi.Next.Vert.Status > 0 && HEi != HEp && HEi.Connected)                 // 亀裂の処理 1回のみ
                {
                    if (HEi.Vert.Status == 1) HEi.Vert.Direction = Vector3.Normalize(HEi.Vert.Pos - HEi.Next.Vert.Pos);
                    if (HEi.Next.Vert.Status == 1) HEi.Vert.Direction = Vector3.Normalize(HEi.Next.Vert.Pos - HEi.Vert.Pos);

                    HEi.Connected = false;
                    HEp.Connected = false;

                    float rp = -15.0f;
                    //int rp = 4;
                    HEiP.ContractionForce += rp * (0.7f + 0.3f * UnityEngine.Random.value);                                                                   // 張力緩和
                    if (HEiP.HE.Next.Pair != null) HEiP.HE.Next.Pair.Face.ContractionForce += rp * (0.7f + 0.3f * UnityEngine.Random.value);
                    if (HEiP.HE.Prev.Pair != null) HEiP.HE.Prev.Pair.Face.ContractionForce += rp * (0.7f + 0.3f * UnityEngine.Random.value);
                    if (HEiP.HE.Pair != null) HEiP.HE.Pair.Face.ContractionForce += rp * (0.7f + 0.3f * UnityEngine.Random.value);

                    HEpP.ContractionForce += rp * (0.7f + 0.3f * UnityEngine.Random.value);
                    if (HEpP.HE.Next.Pair != null) HEpP.HE.Next.Pair.Face.ContractionForce += rp * (0.7f + 0.3f * UnityEngine.Random.value);
                    if (HEpP.HE.Prev.Pair != null) HEpP.HE.Prev.Pair.Face.ContractionForce += rp * (0.7f + 0.3f * UnityEngine.Random.value);
                    if (HEpP.HE.Pair != null) HEpP.HE.Pair.Face.ContractionForce += rp * (0.7f + 0.3f * UnityEngine.Random.value); // ContractionForce += -;

                    if (HEi.Vert.Status < 4) HEi.Vert.Status++;
                    if (HEp.Vert.Status < 4) HEp.Vert.Status++;

                    if (HEi.Vert.Status > 2) // Vertから見てこれが二本目以上の断裂 → 頂点を分裂させて移動
                    {
                        // 頂点を一つ追加，子頂点のNVSは親のみにする
                        _vertices.Add(new Vertex(HEi.Vert)); //_vertices[_vertices.Count - 1].Status = 5;     // 頂点を1個追加
                        //_vertices[_vertices.Count - 1].NVS.Clear(); //_vertices[_vertices.Count - 1].NVS.Add(HEi.Vert);       // 子頂点のNVSは親のみにする

                        // 新しい頂点に繋げるべき稜線を探す
                        HalfEdge tmpHE = HEi;
                        HalfEdge tearedHER;
                        Vector3 Rdelta = new Vector3(0, 0, 0);
                        Vector3 Ldelta = new Vector3(0, 0, 0);
                        List<HalfEdge> RightGroup = new List<HalfEdge>();
                        List<HalfEdge> LeftGroup = new List<HalfEdge>();
                        
                        // HEi.Vertの周り
                        // 反時計回り探索 断裂した稜線も含む
                        tmpHE = HEi;

                        do
                        {
                            if (tmpHE.Pair == null) // 塗膜の縁の場合はこれ以上探さないし，縁に面する稜線は動かさない
                            {
                                Rdelta *= 0f;
                                RightGroup.Clear();
                                break;
                            }
                            Rdelta += tmpHE.Next.Vert.Pos - tmpHE.Vert.Pos;
                            if (tmpHE != HEi) RightGroup.Add(tmpHE);

                            tmpHE = tmpHE.Pair.Next;
                        } while (tmpHE.Connected);
                        tearedHER = tmpHE; // 反時計回り探索の最終地点を記録
                        if (tearedHER.Pair != null) RightGroup.Add(tearedHER);

                        //Debug.Log("debug: " + debug);

                        // 時計回り探索 さっき断裂した半稜線は含むが，断裂していた半稜線は含まない
                        LeftGroup.Add(HEi);
                        tmpHE = HEi.Prev;

                        while (tmpHE.Connected)
                        {

                            if (tmpHE.Pair == null) // 塗膜の縁の場合はこれ以上探さないし，縁に面する稜線は動かさない
                            {
                                Ldelta *= 0f;
                                LeftGroup.Clear();
                                break;
                            }

                            tmpHE = tmpHE.Pair;
                            Ldelta += tmpHE.Next.Vert.Pos - tmpHE.Vert.Pos;
                            LeftGroup.Add(tmpHE);

                            tmpHE = tmpHE.Prev;
                        }
                        // tmpHEには断裂した半稜線（VertがHEi.Vertでないもの）が記録されている
                        // tmpHE.VertとHEi.Pair.Vertは無条件で両方のNVSになる

                        //Debug.Log("Left: " + LeftGroup.Count + ", Right:" + RightGroup.Count);

                        if (RightGroup.Count == 0)
                        {
                            
                            foreach (HalfEdge HEL in LeftGroup)
                            {
                                HEL.Vert = _vertices[_vertices.Count - 1];
                                if (HEL.Pair != null)_vertices[_vertices.Count - 1].NVS.Add(HEL.Pair.Vert); // HEi.Pair.Vertを含む
                                if (HEL != HEi && HEL.Pair != null) HEi.Vert.NVS.Remove(HEL.Pair.Vert);    // 元の頂点のNVSからHEi.Pair.Vertは除かなくてよい
                                if (HEL.Connected && HEL.Next.Vert != HEL.Pair.Vert)
                                {
                                    Debug.Log("ErrorIL Status: " + HEL.Next.Vert.Status + " & " + HEL.Pair.Vert.Status);
                                    Debug.Log("ErrorIL Posision: " + HEL.Next.Vert.Pos + " & " + HEL.Pair.Vert.Pos);
                                }
                            }

                            _vertices[_vertices.Count - 1].NVS.Add(tmpHE.Vert); // これだけは独立で追加

                            _vertices[_vertices.Count - 1].Pos += Ldelta.normalized * 0.02f;
                        }
                        else {
                            
                            foreach (HalfEdge HER in RightGroup)
                            {
                                HER.Vert = _vertices[_vertices.Count - 1];
                                if (HER.Pair != null) _vertices[_vertices.Count - 1].NVS.Add(HER.Pair.Vert);  // tearedHER.Pair.Vertを含む
                                if (HER != tearedHER && HER.Pair != null) HEi.Vert.NVS.Remove(HER.Pair.Vert);    // 元の頂点のNVSからtearedHER.Pair.Vertは除かなくてよい
                                if (HER.Connected && HER.Next.Vert != HER.Pair.Vert)
                                {
                                    Debug.Log("ErrorIR Status: " + HER.Next.Vert.Status + " & " + HER.Pair.Vert.Status);
                                    Debug.Log("ErrorIR Posision: " + HER.Next.Vert.Pos + " & " + HER.Pair.Vert.Pos);
                                }
                            }

                            if (HEi.Pair != null) _vertices[_vertices.Count - 1].NVS.Add(HEi.Pair.Vert); // これだけは独立で追加

                            _vertices[_vertices.Count - 1].Pos += Rdelta.normalized * 0.02f;
                            HEi.Vert.Pos += Ldelta.normalized * 0.02f;
                        }
                    }

                    if (HEp.Vert.Status > 2) // HEpに対しても同じことをする
                    {
                        // 頂点を一つ追加，子頂点のNVSは親のみにする
                        _vertices.Add(new Vertex(HEp.Vert)); //_vertices[_vertices.Count - 1].Status = 5;     // 頂点を1個追加
                        //_vertices[_vertices.Count - 1].NVS.Clear(); //_vertices[_vertices.Count - 1].NVS.Add(HEp.Vert);       // 子頂点のNVSは親のみにする

                        // 新しい頂点に繋げるべき稜線を探す
                        HalfEdge tmpHE = HEp;
                        HalfEdge tearedHER;
                        Vector3 Rdelta = new Vector3(0, 0, 0);
                        Vector3 Ldelta = new Vector3(0, 0, 0);
                        List<HalfEdge> RightGroup = new List<HalfEdge>();
                        List<HalfEdge> LeftGroup = new List<HalfEdge>();

                        // HEp.Vertの周り
                        // 反時計回り探索 断裂した稜線も含む
                        tmpHE = HEp;

                        do
                        {
                            if (tmpHE.Pair == null) // 塗膜の縁の場合はこれ以上探さないし，縁に面する稜線は動かさない
                            {
                                Rdelta *= 0f;
                                RightGroup.Clear();
                                break;
                            }
                            Rdelta += tmpHE.Next.Vert.Pos - tmpHE.Vert.Pos;
                            if (tmpHE != HEp) RightGroup.Add(tmpHE);

                            tmpHE = tmpHE.Pair.Next;
                        } while (tmpHE.Connected);
                        tearedHER = tmpHE; // 反時計回り探索の最終地点を記録
                        if (tearedHER.Pair != null) RightGroup.Add(tearedHER);

                        //Debug.Log("debug: " + debug);

                        // 時計回り探索 さっき断裂した半稜線は含むが，断裂していた半稜線は含まない
                        LeftGroup.Add(HEp);
                        tmpHE = HEp.Prev;

                        while (tmpHE.Connected)
                        {

                            if (tmpHE.Pair == null) // 塗膜の縁の場合はこれ以上探さないし，縁に面する稜線は動かさない
                            {
                                Ldelta *= 0f;
                                LeftGroup.Clear();
                                break;
                            }

                            tmpHE = tmpHE.Pair;
                            Ldelta += tmpHE.Next.Vert.Pos - tmpHE.Vert.Pos;
                            LeftGroup.Add(tmpHE);

                            tmpHE = tmpHE.Prev;
                        }
                        // tmpHEには断裂した半稜線（VertがHEp.Vertでないもの）が記録されている
                        // tmpHE.VertとHEp.Pair.Vertは無条件で両方のNVSになる

                        //Debug.Log("Left: " + LeftGroup.Count + ", Right:" + RightGroup.Count);

                        if (RightGroup.Count == 0)
                        {

                            foreach (HalfEdge HEL in LeftGroup)
                            {
                                HEL.Vert = _vertices[_vertices.Count - 1];
                                if (HEL.Pair != null) _vertices[_vertices.Count - 1].NVS.Add(HEL.Pair.Vert); // HEp.Pair.Vertを含む
                                if (HEL != HEp && HEL.Pair != null) HEp.Vert.NVS.Remove(HEL.Pair.Vert);    // 元の頂点のNVSからHEp.Pair.Vertは除かなくてよい
                                if (HEL.Connected && HEL.Next.Vert != HEL.Pair.Vert)
                                {
                                    Debug.Log("ErrorIL Status: " + HEL.Next.Vert.Status + " & " + HEL.Pair.Vert.Status);
                                    Debug.Log("ErrorIL Posision: " + HEL.Next.Vert.Pos + " & " + HEL.Pair.Vert.Pos);
                                }
                            }

                            _vertices[_vertices.Count - 1].NVS.Add(tmpHE.Vert); // これだけは独立で追加

                            _vertices[_vertices.Count - 1].Pos += Ldelta.normalized * 0.02f;
                        }
                        else
                        {

                            foreach (HalfEdge HER in RightGroup)
                            {
                                HER.Vert = _vertices[_vertices.Count - 1];
                                if (HER.Pair != null) _vertices[_vertices.Count - 1].NVS.Add(HER.Pair.Vert);  // tearedHER.Pair.Vertを含む
                                if (HER != tearedHER && HER.Pair != null) HEp.Vert.NVS.Remove(HER.Pair.Vert);    // 元の頂点のNVSからtearedHER.Pair.Vertは除かなくてよい
                                if (HER.Connected && HER.Next.Vert != HER.Pair.Vert)
                                {
                                    Debug.Log("ErrorIR Status: " + HER.Next.Vert.Status + " & " + HER.Pair.Vert.Status);
                                    Debug.Log("ErrorIR Posision: " + HER.Next.Vert.Pos + " & " + HER.Pair.Vert.Pos);
                                }
                            }

                            if (HEp.Pair != null) _vertices[_vertices.Count - 1].NVS.Add(HEp.Pair.Vert); // これだけは独立で追加

                            _vertices[_vertices.Count - 1].Pos += Rdelta.normalized * 0.02f;
                            HEp.Vert.Pos += Ldelta.normalized * 0.02f;
                        }
                    }

                    //HEi.Vert.Pos += Vector3.Normalize(HEi.Prev.Vert.Pos - HEi.Vert.Pos) * 0.02f;

                    /*
                    _vertices.Add(new Vertex(HEi.Vert)); _vertices[_vertices.Count - 1].Status = 5;     // 頂点を4個追加
                    _vertices[_vertices.Count - 1].NVS.Clear(); _vertices[_vertices.Count - 1].NVS.Add(HEi.Vert);       // 子頂点のNVSは親のみにする      
                    _vertices.Add(new Vertex(HEi.Vert)); _vertices[_vertices.Count - 1].Status = 5;
                    _vertices[_vertices.Count - 1].NVS.Clear(); _vertices[_vertices.Count - 1].NVS.Add(HEi.Vert);
                    _vertices.Add(new Vertex(HEp.Vert)); _vertices[_vertices.Count - 1].Status = 5;
                    _vertices[_vertices.Count - 1].NVS.Clear(); _vertices[_vertices.Count - 1].NVS.Add(HEp.Vert);
                    _vertices.Add(new Vertex(HEp.Vert)); _vertices[_vertices.Count - 1].Status = 5;
                    _vertices[_vertices.Count - 1].NVS.Clear(); _vertices[_vertices.Count - 1].NVS.Add(HEp.Vert);

                    HEi.Vert = _vertices[_vertices.Count - 4];      // ひび周辺では元の頂点は参照されなくなる
                    HEp.Vert = _vertices[_vertices.Count - 1];
                    HEi.Next.Vert = _vertices[_vertices.Count - 2];
                    HEp.Next.Vert = _vertices[_vertices.Count - 3];

                    HEi.Vert.Pos += Vector3.Normalize(HEi.Prev.Vert.Pos - HEi.Vert.Pos) * 0.02f;
                    HEp.Vert.Pos += Vector3.Normalize(HEp.Prev.Vert.Pos - HEp.Vert.Pos) * 0.02f;
                    */

                    // 剥離処理：　剥落判定＆頂点移動
                    if (!HEi.Next.Connected && !HEi.Prev.Connected && false)  // すべての稜線が断裂 → 剥落
                    {
                        if (HEiP.Peeled == false)
                        {
                            HEiP.Peeled = true;
                            Vertex LowestV = HEi.Vert;
                            float height = Vector3.Dot(HEi.Vert.Pos, _gravity);
                            if (height < Vector3.Dot(HEi.Next.Vert.Pos, _gravity))
                            {
                                LowestV = HEi.Next.Vert;
                                height = Vector3.Dot(HEi.Next.Vert.Pos, _gravity);
                            }
                            if (height < Vector3.Dot(HEi.Prev.Vert.Pos, _gravity))
                            {
                                LowestV = HEi.Prev.Vert;
                            }
                            if (LowestV.Status > 4)                         // 最下点が子供
                            {
                                Vertex Parent = LowestV.NVS[0];
                                Parent.Rust += 100f;
                                for (int j = 0; j < Parent.NVS.Count; j++)
                                {
                                    if (Parent.NVS[j].Status > 4)
                                        Parent.NVS[j].Rust += 100f;
                                }
                            }
                            else
                            {
                                LowestV.Rust += 100f;
                                for (int j = 0; j < LowestV.NVS.Count; j++)
                                {
                                    if (LowestV.NVS[j].Status > 4)
                                        LowestV.NVS[j].Rust += 100f;
                                }
                            }
                        }
                    }                                                           // 頂点移動処理（分離後移動方式に変更したため無効化 2020/12/05）
                    else if (!HEi.Next.Connected)                                                 // 2本の稜線が断裂： nextが断裂してない
                    {
                        // HEi.Prev.Vert.Pos += HEi.Prev.Vert.Nor * 0.1f;
                    }
                    else if (!HEi.Prev.Connected)                                // 2本の稜線が断裂： next next が断裂してない
                    {
                        // HEi.Next.Vert.Pos += HEi.Next.Vert.Nor * 0.1f;
                    }

                    if (!HEp.Next.Connected && !HEp.Prev.Connected && false)
                    {

                        if (HEpP.Peeled == false)
                        {
                            HEpP.Peeled = true;
                            Vertex LowestV = HEp.Vert;
                            float height = Vector3.Dot(HEp.Vert.Pos, _gravity);
                            if (height < Vector3.Dot(HEp.Next.Vert.Pos, _gravity))
                            {
                                LowestV = HEp.Next.Vert;
                                height = Vector3.Dot(HEp.Next.Vert.Pos, _gravity);
                            }
                            if (height < Vector3.Dot(HEp.Prev.Vert.Pos, _gravity))
                            {
                                LowestV = HEp.Prev.Vert;
                            }
                            if (LowestV.Status > 4)
                            {
                                Vertex Parent = LowestV.NVS[0];
                                Parent.Rust += 100f;
                                for (int j = 0; j < Parent.NVS.Count; j++)
                                {
                                    if (Parent.NVS[j].Status > 4)
                                        Parent.NVS[j].Rust += 100f;
                                }
                            }
                            else
                            {
                                LowestV.Rust += 100f;
                                for (int j = 0; j < LowestV.NVS.Count; j++)
                                {
                                    if (LowestV.NVS[j].Status > 4)
                                        LowestV.NVS[j].Rust += 100f;
                                }
                            }

                        }
                    }
                    else if (!HEp.Next.Connected)
                    {
                        //HEp.Prev.Vert.Pos += HEp.Prev.Vert.Nor * 0.02f;
                    }
                    else if (!HEp.Prev.Connected)
                    {
                        //HEp.Next.Vert.Pos += HEp.Next.Vert.Nor * 0.02f;
                    }

                    HEiP.ReloadNormal();
                    HEpP.ReloadNormal();

                }
            }
            sw5.Stop();

            sw7.Start();
            for (int i = 0; i < _tpolygons.Count; i++)      // ポリゴンの最終処理：ポリゴン座標をメッシュに代入
            {
                if (_tpolygons[i].Peeled
                    // || (_vertices[_tpolygons[i].V1].Separated && _vertices[_tpolygons[i].V2].Separated)
                    // || (_vertices[_tpolygons[i].V2].Separated && _vertices[_tpolygons[i].V3].Separated)
                    // || (_vertices[_tpolygons[i].V3].Separated && _vertices[_tpolygons[i].V1].Separated)
                    )
                {
                    //Debug.Log("Peeled!");
                    _triangles[i * 3] = _tpolygons[i].HE.Vert.Idx;
                    _triangles[i * 3 + 1] = _tpolygons[i].HE.Vert.Idx;
                    _triangles[i * 3 + 2] = _tpolygons[i].HE.Vert.Idx;

                    // 剥落なし

                    //_triangles[i * 3] = _vertices.IndexOf(_tpolygons[i].HE.Vert);
                    //_triangles[i * 3 + 1] = _vertices.IndexOf(_tpolygons[i].HE.Next.Vert);
                    //_triangles[i * 3 + 2] = _vertices.IndexOf(_tpolygons[i].HE.Prev.Vert);

                }
                else
                {
                    _triangles[i * 3] = _tpolygons[i].HE.Vert.Idx;
                    _triangles[i * 3 + 1] = _tpolygons[i].HE.Next.Vert.Idx;
                    _triangles[i * 3 + 2] = _tpolygons[i].HE.Prev.Vert.Idx;
                }
            }

            Array.Resize(ref _positions, _vertices.Count);
            Array.Resize(ref _normals, _vertices.Count);
            Array.Resize(ref _uvs, _vertices.Count);
            Array.Resize(ref _colors, _vertices.Count);

            // 位置計算
            for (int i = 0; i < _vertices.Count && _simulating; i++)
                _positions[i] = _vertices[i].Pos;

            // 法線計算
            int[] Nface = new int[_vertices.Count];
            Vector3[] NormalSum = new Vector3[_vertices.Count];
            for (int i = 0; i < _vertices.Count; i++) Nface[i] = 0;
            for (int i = 0; i < _tpolygons.Count; i++)
            {
                TPolygon TP = _tpolygons[i];
                NormalSum[TP.HE.Vert.Idx] += TP.Normal; Nface[TP.HE.Vert.Idx]++;
                NormalSum[TP.HE.Next.Vert.Idx] += TP.Normal; Nface[TP.HE.Next.Vert.Idx]++;
                NormalSum[TP.HE.Prev.Vert.Idx] += TP.Normal; Nface[TP.HE.Prev.Vert.Idx]++;
            }
            for (int i = 0; i < _vertices.Count; i++)
            {
                // _vertices[i].Nor = NormalSum[i] / Nface[i];
                _normals[i] = _vertices[i].Nor;

            }

            // 収縮力計算 （デバッグ用）
            // Debug.Log("Contract: " + _tpolygons[100].ContractionForce);
            int[] Cface = new int[_vertices.Count];
            float[] ContractSum = new float[_vertices.Count];
            for (int i = 0; i < _vertices.Count; i++) Cface[i] = 0;
            for (int i = 0; i < 0 * _tpolygons.Count; i++)
            {
                TPolygon TP = _tpolygons[i];
                ContractSum[TP.HE.Vert.Idx] += TP.ContractionForce; Cface[TP.HE.Vert.Idx]++;
                ContractSum[TP.HE.Next.Vert.Idx] += TP.ContractionForce; Cface[TP.HE.Next.Vert.Idx]++;
                ContractSum[TP.HE.Prev.Vert.Idx] += TP.ContractionForce; Cface[TP.HE.Prev.Vert.Idx]++;
            }
            for (int i = 0; i < _vertices.Count; i++)
            {
                //_vertices[i].Rust = ContractSum[i] / Cface[i];

            }

            /*        if (Input.GetMouseButtonDown(0))
                    {
                        Input.mousePosition;
                        _camTransform.transform.eulerAngles.x;
                        _camTransform.transform.eulerAngles.y;

                        _presentCamPos = _camTransform.position;
                    }*/

            //Debug.Log(PoissonDisc.PointN);
            Array.Resize(ref _uvs, _positions.Length);
            if (Input.GetKeyDown(KeyCode.V))
                VisToggle._vis = !VisToggle._vis;

            for (int i = 0; i < _positions.Length; i++)
                if (VisToggle._vis)
                {
                    float BL = 50f - (_vertices[i].Bond - _vertices[i].LiftingForce);
                    _uvs[i] = new Vector2(BL > 49f ? 0.98f : (BL < 1f ? 0.02f : BL / 50.0f), 0.5f);//_uvs[i] = new Vector2(_vertices[i].Bond - _vertices[i].LiftingForce > 10f ? 0.0f : (_vertices[i].Bond - _vertices[i].LiftingForce < 0.0f ? 1.0f : 1.0f - (_vertices[i].Bond - _vertices[i].LiftingForce) / 10.0f), 0.5f);//_uvs[i] = new Vector2(_vertices[i].Bond > 30f ? 0.0f : (_vertices[i].Bond < 0.0f ? 1.0f : 1.0f - _vertices[i].Bond / 30.0f), 0.5f);

                }
                else if (Input.GetKey(KeyCode.N))
                    _uvs[i] = new Vector2(_vertices[i].Nor.x * 0.5f + 0.5f, _vertices[i].Nor.y * 0.5f + 0.5f);
                else if (Input.GetKey(KeyCode.B))
                    _uvs[i] = new Vector2(_vertices[i].Bond * 0.005f < 0.01f ? 0.99f : (_vertices[i].Bond * 0.005f > 0.99f ? 0.01f : 1.0f - _vertices[i].Bond * 0.005f), 0.5f);
                else if (Input.GetKey(KeyCode.L))
                    _uvs[i] = new Vector2(_vertices[i].LiftingForce * 0.03f < 0.01f ? 0.01f : (_vertices[i].LiftingForce * 0.03f > 0.99f ? 0.99f : _vertices[i].LiftingForce * 0.03f), 0.5f);
                else if (Input.GetKey(KeyCode.C))
                    _uvs[i] = new Vector2(Mathf.Clamp(_vertices[i].Rust / 200.0f, 0.01f, 0.99f), 0.5f);
                else
                    _uvs[i] = _vertices[i].Tex;
            sw7.Stop();

            sw6.Start();
            for (int i = 0; i < _vertices.Count; i++)
            {
                // 錆
                _colors[i] = new Color(132.0f / 255.0f, 51.0f / 255.0f, 0.0f, 1.0f) + new Color(123.0f / 255.0f, 204.0f / 255.0f, 1.0f, 0.0f) * Mathf.Exp(-(_vertices[i].Rust > 10.0f ? _vertices[i].Rust - 10.0f : 0.0f) / 100.0f);
                // 塵
                _colors[i] = _colors[i] * (0.0f + 1.0f * Mathf.Exp(-(_vertices[i].Dust) / 100.0f));

                // _colors[i] = _vertices[i].Col;
                //_colors[i] = Color.white;
            }
            sw6.Stop();

            count++;
        } // 計算時間測定 ループ end
        sw1.Stop();

        if (firstloop)
        {
            Debug.Log("Start: " + sw0.Elapsed);
            Debug.Log("Total: " + sw1.Elapsed + " / " + (100 * sw1.Elapsed.TotalMilliseconds / sw1.Elapsed.TotalMilliseconds) + " %");
            Debug.Log("Dust: " + sw2.Elapsed + " / " + (100 * sw2.Elapsed.TotalMilliseconds / sw1.Elapsed.TotalMilliseconds) + " %");
            Debug.Log("Separate: " + sw3.Elapsed + " / " + (100 * sw3.Elapsed.TotalMilliseconds / sw1.Elapsed.TotalMilliseconds) + " %");
            Debug.Log("Rust: " + sw4.Elapsed + " / " + (100 * sw4.Elapsed.TotalMilliseconds / sw1.Elapsed.TotalMilliseconds) + " %");
            Debug.Log("Deform: " + sw5.Elapsed + " / " + (100 * sw5.Elapsed.TotalMilliseconds / sw1.Elapsed.TotalMilliseconds) + " %");
            Debug.Log("Color: " + sw6.Elapsed + " / " + (100 * sw6.Elapsed.TotalMilliseconds / sw1.Elapsed.TotalMilliseconds) + " %");
            Debug.Log("Mesh: " + sw7.Elapsed + " / " + (100 * sw7.Elapsed.TotalMilliseconds / sw1.Elapsed.TotalMilliseconds) + " %");
            Debug.Log("Other: " + (100f - 100 * (sw2.Elapsed.TotalMilliseconds + sw3.Elapsed.TotalMilliseconds + sw4.Elapsed.TotalMilliseconds +
                                                    sw5.Elapsed.TotalMilliseconds + sw6.Elapsed.TotalMilliseconds + sw7.Elapsed.TotalMilliseconds) / sw1.Elapsed.TotalMilliseconds) + " %");
            Debug.Log("Vertices: " + _vertices.Count);

            Array.Resize(ref _Bpositions, _BposList.Count);
            Array.Resize(ref _Bnormals, _BposList.Count);
            Array.Resize(ref _Buvs, _BposList.Count);
            Array.Resize(ref _Bcolors, _BposList.Count);

            for (int i = 0; i < _BposList.Count; i++)
            {
                _Bpositions[i] = _BposList[i] - _BnorList[i] * 0.03f;
                _Bnormals[i] = _BnorList[i];
                //_Buvs[i] = _BuvList[i];
                _Buvs[i] = new Vector2(_Bpositions[i].x * 0.1f + 0.5f, _Bpositions[i].y * 0.1f + 0.5f);
                _Bcolors[i] = Color.black;
            }

            Array.Resize(ref _Btriangles, _BtriList.Count * 3);

            for (int i = 0; i < _BtriList.Count; i++)      // ポリゴンの最終処理：ポリゴン座標をメッシュに代入
            {
                _Btriangles[i * 3] = _BtriList[i].x;
                _Btriangles[i * 3 + 1] = _BtriList[i].y;
                _Btriangles[i * 3 + 2] = _BtriList[i].z;
            }

            _Bmesh.vertices = _Bpositions;
            _Bmesh.triangles = _Btriangles;
            _Bmesh.normals = _Bnormals;
            _Bmesh.uv = _Buvs;
            _Bmesh.colors = _Bcolors;

            _Bmesh.RecalculateBounds();

            firstloop = false;
        }

        _Bmesh.colors = _Bcolors;
        //Debug.Log(_Bcolors[100].r);
        _Bmesh.RecalculateBounds();


        // (4) Meshに頂点情報を代入
        _mesh.vertices = _positions;
        _mesh.triangles = _triangles;
        _mesh.normals = _normals;
        _mesh.uv = _uvs;
        _mesh.colors = _colors;

        _mesh.RecalculateBounds();


        // (5) 描画
        if (VisToggle._vis)
            Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _submat, 0);
        else if (Input.GetKey(KeyCode.N))
            Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _normal, 0);
        else if (Input.GetKey(KeyCode.L))
            Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _submat, 0);
        else if (Input.GetKey(KeyCode.B))
            Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _submat, 0);
        else if (Input.GetKey(KeyCode.C))
            Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _submat, 0);
        else if (Input.GetKey(KeyCode.T))
            ;
        else
            Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _material, 0);

        Graphics.DrawMesh(_Bmesh, Vector3.zero, Quaternion.identity, _basemat, 0);
    }
}