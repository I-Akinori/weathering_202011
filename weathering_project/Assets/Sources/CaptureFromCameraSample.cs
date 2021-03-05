using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaptureFromCameraSample : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    public void OnClick()
    {
        CaptureScreenShot(Application.streamingAssetsPath + "/tex3.png");
    }

    // カメラのスクリーンショットを保存する
    private void CaptureScreenShot(string filePath)
    {
        var rt = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 24);
        var prev = _camera.targetTexture;
        _camera.targetTexture = rt;
        _camera.Render();
        _camera.targetTexture = prev;
        RenderTexture.active = rt;

        var screenShot = new Texture2D(
            128,
            128,
            TextureFormat.RGB24,
            false);

        Color[] c = new Color[128 * 128];
        for (int i = 0; i < 128; i++) {
            for (int j = 0; j < 128; j++)
            {
                c[i * 128 + j] = new Color(0, 0, 0);
            }
        }
        screenShot.SetPixels(c);
        screenShot.Apply();

        var bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        File.WriteAllBytes(filePath, bytes);
    }
}