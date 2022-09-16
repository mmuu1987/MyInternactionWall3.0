using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

public class Tex2DArrayTest : MonoBehaviour
{

    public List<Texture2D> Texs = new List<Texture2D>();

    [MenuItem("GameObject/Create Texture2DArry")]
    static void CreateTexture2DArry()
    {
        // Create a simple material asset  

        string[] files = Directory.GetFiles(Application.dataPath + "/Pictures");


        List<Texture2D> texTemps = new List<Texture2D>();
        List<Vector2>  sizes = new List<Vector2>();

        foreach (string file in files)
        {
            if (file.Contains(".meta")) continue;


            string filePath = file.Replace(Application.dataPath, "");

            filePath = filePath.Replace("\\", "/");
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/" + filePath);
            sizes.Add(new Vector2(tex.width,tex.height));
            tex = GlobalSetting.Resize(tex, 512, 512);

            if (tex != null)
            {
                Debug.Log(tex.name);
                texTemps.Add(tex);
            }
        }


        Debug.Log(texTemps.Count);

        Texture2DArray TexAry = SetTexToGpu(texTemps.ToArray());


        CraetSizeTex(sizes);


        AssetDatabase.CreateAsset(TexAry, "Assets/Mytex2darry.asset");

       

      
    }

    [MenuItem("GameObject/Load Texture2DArry")]
    public static void LoadTexture2dArry()
    {

        Texture2DArray tex = AssetDatabase.LoadAssetAtPath<Texture2DArray>("Assets/Mytex2darry.asset");

        

        Debug.Log(tex.depth);

    }


    public VisualEffect VfX;
    void Start()
    { 

        //AssetDatabase.CreateAsset(texArr, "Assets/RogueX/Prefab/texArray.asset");
    }

    public static Texture2DArray SetTexToGpu(Texture2D[] texs)
    {
        if (texs == null || texs.Length == 0)
        {

            return null;
        }

        if (SystemInfo.copyTextureSupport == CopyTextureSupport.None ||
            !SystemInfo.supports2DArrayTextures)
        {

            return null;
        }

        Texture2DArray texArr = new Texture2DArray(texs[0].width, texs[0].width, texs.Length, texs[0].format, false, false);

        // 结论 //
        // Graphics.CopyTexture耗时(单位:Tick): 5914, 8092, 6807, 5706, 5993, 5865, 6104, 5780 //
        // Texture2DArray.SetPixels耗时(单位:Tick): 253608, 255041, 225135, 256947, 260036, 295523, 250641, 266044 //
        // Graphics.CopyTexture 明显快于 Texture2DArray.SetPixels 方法 //
        // Texture2DArray.SetPixels 方法的耗时大约是 Graphics.CopyTexture 的50倍左右 //
        // Texture2DArray.SetPixels 耗时的原因是需要把像素数据从cpu传到gpu, 原文: Call Apply to actually upload the changed pixels to the graphics card //
        // 而Graphics.CopyTexture只在gpu端进行操作, 原文: operates on GPU-side data exclusively //

        //using (Timer timer = new Timer(Timer.ETimerLogType.Tick))
        //{

        for (int i = 0; i < texs.Length; i++)
        {
            // 以下两行都可以 //
            //Graphics.CopyTexture(textures[i], 0, texArr, i);
            Graphics.CopyTexture(texs[i], 0, 0, texArr, i, 0);
        }


        //}

        texArr.wrapMode = TextureWrapMode.Clamp;
        texArr.filterMode = FilterMode.Bilinear;


        return texArr;

    }

    public static void CraetSizeTex(List<Vector2> texs)
    {
        Texture2D tex = new Texture2D(texs.Count, 1,TextureFormat.RGBA32,false);

        for (int i = 0; i < texs.Count; i++)
        {

            Vector2 size = GlobalSetting.ScaleImageSize(new Vector2(texs[i].x, texs[i].y), new Vector2(512f, 512f));

            size = size / 512f;

            tex.SetPixel(i,1,new Color(size.x,size.y,0));


        }

        AssetDatabase.CreateAsset(tex, "Assets/SizeTex.asset");


    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 0, 200, 100), "Change Texture"))
        {


        }
    }

}

public class PicturesInfo: ScriptableObject
{
    public int ID;

    public Vector2 Size;
}