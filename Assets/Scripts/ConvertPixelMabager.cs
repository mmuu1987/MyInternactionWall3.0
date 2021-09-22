using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConvertPixelMabager : MonoBehaviour
{

    public int ScreenWidth = 1920;

    public int ScreenHeight = 1080;

    public float PictureWidth = 100f;

    public float PictureHeight = 100f;

    public float SpaceWidth = 3f;

    public float SpaceHeight = 3f;

    public int Column = 19;

    public int Row = 10;


    private List<ConvertPixel > ConvertPixels = new List<ConvertPixel>();

    /// <summary>
    /// 选择像素模式还是个数模式,像素模式是每个图片大小一致，个数模式为每个图片的宽不一致，高一致
    /// </summary>
    public bool IsSelectPixe = true;
    // Start is called before the first frame update
    void Start()
    {
        HandleData();
    }

    /// <summary>
    /// 处理数据，得出图片在屏幕的个数
    /// </summary>
    private List<Rect> HandleData()
    {
        List<Rect> rects = new List<Rect>();
        if (IsSelectPixe)
        {
            //像素模式下，需要知道屏幕分辨率和Column  row的个数


            Column += 1;//列数多一个，防止运动的时候有真空带
            Row += 1;//横数多一个，防止运动的时候有真空带

            float width = ScreenWidth * 1f / Column ;

            float height = ScreenHeight * 1f / Row;

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    Rect rect = new Rect(width* i +width/2,height * j+height/2, width,height);

                    rects.Add(rect);

                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);

                    go.transform.position = Vector3.zero;

                    ConvertPixel cp =  go.AddComponent<ConvertPixel>();

                    go.name = ((i+1) * (j+1)).ToString();

                    cp.Init();

                    ConvertPixels.Add(cp);

                    cp.SetPosSize(rect);

                }
            }
        }
        else
        {
            
        }

        return rects;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
      //  Gizmos.color = new Color(1, 0, 0, 0.5f);
        //Gizmos.DrawWireMesh(transform.position, new Vector3(1, 1, 1));
    }
}
