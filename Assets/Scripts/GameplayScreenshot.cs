
using UnityEngine;
using System.IO;
using System;

public class GameplayScreenshot : MonoBehaviour
{
    public Camera captureCamera;
    public int width = 1920;
    public int height = 1080;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            CaptureScreenshot();
        }
    }

    public void CaptureScreenshot()
    {
        if (captureCamera == null)
        {
            Debug.LogError("No hay ninguna cámara asignada.");
            return;
        }

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        RenderTexture previousTexture = captureCamera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        float previousAspect = captureCamera.aspect;

        captureCamera.targetTexture = renderTexture;
        captureCamera.aspect = (float)width / height;
        captureCamera.Render();

        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        captureCamera.targetTexture = previousTexture;
        captureCamera.aspect = previousAspect;
        RenderTexture.active = previousActive;

        byte[] bytes = screenshot.EncodeToPNG();

        string folder = Path.Combine(Application.persistentDataPath, "Screenshots");
        Directory.CreateDirectory(folder);

        string fileName = "Gameplay_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string path = Path.Combine(folder, fileName);

        File.WriteAllBytes(path, bytes);

        renderTexture.Release();
        Destroy(renderTexture);
        Destroy(screenshot);

        Debug.Log("Captura guardada en: " + path);
    }
}
