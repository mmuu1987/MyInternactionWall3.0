using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Tex2DArrayTest : MonoBehaviour
{


    public List<MeshRenderer> MeshRenderers = new List<MeshRenderer>();

    

    public PictureManager ScaleImage;

    void Start()
    {


        //AssetDatabase.CreateAsset(texArr, "Assets/RogueX/Prefab/texArray.asset");
    }

    public void SetTexToGpu(Texture2D[] texs)
    {
        if (texs == null || texs.Length == 0)
        {
            enabled = false;
            return;
        }

        if (SystemInfo.copyTextureSupport == CopyTextureSupport.None ||
            !SystemInfo.supports2DArrayTextures)
        {
            enabled = false;
            return;
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



     
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 0, 200, 100), "Change Texture"))
        {

            MaterialPropertyBlock props = new MaterialPropertyBlock();
            int n = 0;
            foreach (MeshRenderer meshRenderer in MeshRenderers)
            {
                n++;
                meshRenderer.material = ScaleImage.Mat;
                props.SetInt("_Index",n);

                if (n >= 100) n = 0;

                meshRenderer.SetPropertyBlock(props);
            }
        }
    }

}
