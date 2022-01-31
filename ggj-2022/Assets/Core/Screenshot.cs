using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Screenshot
{
#if UNITY_EDITOR
  [MenuItem("Utilities/Take Screenshot")]
  public static void SaveOpaque()
  {
    Save(TextureFormat.RGB24);
  }

  [MenuItem("Utilities/Take Screenshot Transparent")]
  public static void SaveTransparent()
  {
    Save(TextureFormat.ARGB32);
  }

  private static void Save(TextureFormat format)
  {
    string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
    string[] projectPath = Application.dataPath.Split('/');
    string projectName = projectPath[projectPath.Length - 2];
    path += string.Format("/{0}-shot-{1}.png", projectName, System.DateTime.Now.ToString("MM.dd.HH.mm.ss"));

    RenderTexture tex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    tex.Create();

    Texture2D savedTex = new Texture2D(tex.width, tex.height, format, false);

    Camera.main.targetTexture = tex;
    Camera.main.Render();

    RenderTexture.active = tex;
    savedTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
    savedTex.Apply();

    byte[] imageBytes = ImageConversion.EncodeToPNG(savedTex);
    System.IO.File.WriteAllBytes(path, imageBytes);

    RenderTexture.active = null;
    Camera.main.targetTexture = null;

    tex.Release();
    Texture2D.Destroy(savedTex);
    RenderTexture.Destroy(tex);
  }
#endif
}