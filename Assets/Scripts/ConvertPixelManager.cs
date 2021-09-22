using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConvertPixelManager : MonoBehaviour
{

    

    public float PictureWidth = 100f;

    public float PictureHeight = 100f;

    public float SpaceWidth = 3f;

    public float SpaceHeight = 3f;

    public int Column = 50;

    public int Row = 30;

    public Material Material;

    public GameObject PrefabGameObject;

    private List<ConvertPixel> ConvertPixels = new List<ConvertPixel>();

    /// <summary>
    /// 选择像素模式还是个数模式,像素模式是每个图片大小一致，个数模式为每个图片的宽不一致，高一致
    /// </summary>
    public bool IsSelectPixe = true;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("窗口分辨率为：" +Screen.width +"   " +Screen.height);
        HandleData();
    }

   
    public void SetPos()
    {
       List<PictureInfo> infos = new List<PictureInfo>();

       List<Vector2> screenPosList= new List<Vector2>();

       Vector2 pos= Vector2.zero;
       foreach (PictureInfo pictureInfo in infos)
       {
           Vector2 temp = Vector2.zero;

           Vector2 size = GlobalSetting.ScaleImageInHeight(pictureInfo.Size, 256);

           temp = pos + new Vector2(size.x / 2, size.y / 2);

           screenPosList.Add(temp);

           pos = temp;
       }
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

            float width = Screen.width * 1f / Column;

            float height = Screen.height * 1f / Row;


            Debug.Log("计算得出每个矩形的长是：" +width+"像素 " + "   高是 " +height + "像素");
            for (int j = 0; j < Row; j++)
            {
                for (int i = 0; i < Column; i++)
                {

                    Rect rect = new Rect(width * i + width / 2, height * j + height / 2, width, height);

                    rects.Add(rect);

                    ConvertPixel cp = Instantiate(PrefabGameObject).GetComponent<ConvertPixel>();

                    // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);

                    cp.gameObject.transform.position = Vector3.zero;



                    cp.gameObject.name = ((i + 1) * (j + 1)).ToString();

                    cp.Init(Material);

                    ConvertPixels.Add(cp);

                    cp.SetPosSize(rect, new Vector2(SpaceWidth, SpaceHeight));

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


}
