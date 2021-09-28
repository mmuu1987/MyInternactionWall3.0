using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;


public class MotionlManager : MonoBehaviour
{




    /// <summary>
    /// 图片的高
    /// </summary>
    [Tooltip("图片的高,应用在高度模式")]
    public float PictureHeight = 100f;
    /// <summary>
    /// 两个图片的间隔距离的宽
    /// </summary>
    public float SpaceWidth = 3f;
    /// <summary>
    /// 两个图片的间隔距离的高
    /// </summary>
    public float SpaceHeight = 3f;

    public int Column = 50;

    public int Row = 30;

    public Material Material;

    public GameObject PrefabGameObject;


    public List<Texture2D> texs = new List<Texture2D>();

    public List<PictureInfo> PictureInfo = new List<PictureInfo>();


    private List<MyMotionTexture> ConvertPixels = new List<MyMotionTexture>();


    public List<Circular> Circulars= new List<Circular>();


    /// 选择像素模式还是个数模式,像素模式是每个图片大小一致，个数模式为每个图片的宽不一致，高一致
    /// </summary>
    [Tooltip("选择像素模式还是个数模式,高度模式是每个图片大小一致，个数模式为每个图片的宽不一致，高一致")]
    public bool IsSelectPixe = true;
    // Start is called before the first frame update

    /// <summary>
    /// 存储每行右边图片所到达的最大分辨率
    /// </summary>
    private Dictionary<int, float> _maxScreenPos = new Dictionary<int, float>();
    void Start()
    {
        Debug.Log("窗口分辨率为：" + Screen.width + "   " + Screen.height);
        LoadImage();

        HandleData();
    }

    /// <summary>
    /// 根据图片高度来设置图片位置序列
    /// </summary>
    public void SetPos(float targetHeight)
    {
        List<PictureInfo> infos = PictureInfo;

        MaterialPropertyBlock props = new MaterialPropertyBlock();

        List<Vector2> screenPosList = new List<Vector2>();

        Vector2 pos = new Vector2(0f, targetHeight / 2f);
        int objNameIndex = 0;
        float previousWidth = 0;//上一个图片的宽
        int index = 0;

        int rows = 0;
        int column = 0;
        while (true)
        {
            if (index >= PictureInfo.Count)
            {
                index = 0;
               
            }

            PictureInfo pictureInfo = PictureInfo[index];

            Vector2 size = GlobalSetting.ScaleImageInHeight(pictureInfo.Size, targetHeight);

            pictureInfo.Size = size;

            if (pos.x > Screen.width)//如果超出屏幕，则高度加多一个targetHeight，并且重置X轴
            {

                rows++;
                _maxScreenPos.Add(rows, pos.x + previousWidth / 2);
                //Debug.Log("row is" + rows + "    pos is " + pos.x + previousWidth / 2 + "    column is " + column);

                pos.y += targetHeight;
                pos.x = 0;
                previousWidth = 0;
                column = 1;


            }

            if (pos.y >= Screen.height) break;

            pos = pos + new Vector2(size.x / 2 + previousWidth / 2, 0);//得到当前图片的位置
            column++;
           // Debug.Log("column  is " + column);

            previousWidth = size.x;//赋予当前图片的看宽度给下个用 

            screenPosList.Add(pos);

            Rect rect = new Rect(pos, size);

            MyMotionTexture cp = Instantiate(PrefabGameObject).GetComponent<MyMotionTexture>();



            cp.gameObject.transform.position = Vector3.zero;



            cp.gameObject.name = objNameIndex.ToString();

            objNameIndex++;

            cp.Init(Material);

            ConvertPixels.Add(cp);

            cp.SetPosSize(rect, new Vector2(SpaceWidth, SpaceHeight));

            cp.SetInfo(pictureInfo, props, rows+1, column+1);

            cp.CircularList = Circulars;
            //if (i == Column - 1)
            //{
            //    Debug.Log(i + "   " + width * (i + 1));
            //    _maxScreenPos.Add(j, width * (i + 1));
            //}

            index++;
        }


        SetMaxScreenPos();
        
      
    }

    public void SetMaxScreenPos()
    {
        foreach (MyMotionTexture convertPixel in ConvertPixels)
        {
            foreach (KeyValuePair<int, float> keyValuePair in _maxScreenPos)
            {
                if (keyValuePair.Key == convertPixel.Row)
                {
                    convertPixel.MaxScreenPos = keyValuePair.Value;
                    Debug.Log("MaxScreenPos is" + convertPixel.MaxScreenPos);
                    break;
                }
            }
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

            int index = 0;

            MaterialPropertyBlock props = new MaterialPropertyBlock();

            Debug.Log("计算得出每个矩形的长是：" + width + "像素 " + "   高是 " + height + "像素");

        
            for (int j = 0; j < Row; j++)
            {
                for (int i = 0; i < Column; i++)
                {
                    if (index >= PictureInfo.Count) index = 0;

                   
                    global::PictureInfo info = PictureInfo[index];

                    info.Size = new Vector2(SpaceWidth,SpaceHeight);

                    Rect rect = new Rect(width * i + width / 2, height * j + height / 2, width, height);

                    rects.Add(rect);

                    MyMotionTexture cp = Instantiate(PrefabGameObject).GetComponent<MyMotionTexture>();

                    cp.gameObject.transform.position = Vector3.zero;

                    cp.gameObject.name = ((i + 1) * (j + 1)).ToString();

                    cp.Init(Material);

                    ConvertPixels.Add(cp);

                    cp.SetPosSize(rect, new Vector2(SpaceWidth, SpaceHeight));


                   

                    cp.SetInfo(PictureInfo[index],props,i+1,j+1);

                    if (i == Column - 1)
                    {
                        Debug.Log(j+"   " + width * (i + 1));
                        _maxScreenPos.Add(j, width * (i+1));
                    }

                    index++;
                }
            }
        }
        else
        {
            SetPos(PictureHeight);
        }

        return rects;
    }

    private void LoadImage()
    {
        if (texs.Count == 0)
        {
            string[] files = Directory.GetFiles(Application.streamingAssetsPath + "/Pictures");

            texs = new List<Texture2D>();
            foreach (string file in files)
            {
                if (file.Contains(".meta")) continue;

                DirectoryInfo info = new DirectoryInfo(file);
                int index = 0;
                try
                {
                    string temp = info.Name;

                    index = int.Parse(temp.Substring(0, 3)) - 1;//序号对应上，图片的是1开始，代码的是0开始
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    throw;
                }


                byte[] bytes = File.ReadAllBytes(file);

                Texture2D tex = new Texture2D(1024, 1024);

                tex.LoadImage(bytes);

                tex.Apply();

                Vector2 size = new Vector2(tex.width, tex.height);

                PictureInfo.Add(new PictureInfo(info.Name, index, size));

                tex = GlobalSetting.Resize(tex, 512, 512);

                texs.Add(tex);


            }

            SetTexToGpu(texs.ToArray());
        }
        else
        {
            List<Texture2D> temps = new List<Texture2D>();
            foreach (Texture2D texture2D in texs)
            {

                try
                {
                    string temp = texture2D.name;

                    int index = int.Parse(temp.Substring(0, 3)) - 1;//序号对应上，图片的是1开始，代码的是0开始

                    Vector2 size = new Vector2(texture2D.width, texture2D.height);

                    PictureInfo.Add(new PictureInfo(texture2D.name, index, size));
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    throw;
                }

                var tex = GlobalSetting.Resize(texture2D, 512, 512);




                temps.Add(tex);
            }

            SetTexToGpu(temps.ToArray());
        }





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

        for (int i = 0; i < texs.Length; i++)
        {
            // 以下两行都可以 //
            //Graphics.CopyTexture(textures[i], 0, texArr, i);
            Graphics.CopyTexture(texs[i], 0, 0, texArr, i, 0);
        }

        texArr.wrapMode = TextureWrapMode.Clamp;
        texArr.filterMode = FilterMode.Trilinear;

        Material.SetTexture("_TexArr", texArr);

        foreach (Texture2D texture2D in texs)
        {
            Destroy(texture2D);
        }

        foreach (PictureInfo info in PictureInfo)
        {
            //Debug.Log(info.ToString());
        }

        Resources.UnloadUnusedAssets();
    }

    // Update is called once per frame
    void Update()
    {

    }


}
public enum MoveType
{
    None,
    /// <summary>
    /// 从右边向左运动
    /// </summary>
    ForLeft,//
    /// <summary>
    /// 从屏幕外飞到屏幕内组成文字
    /// </summary>
    Label,// 
    /// <summary>
    /// 重置图片到屏幕初始位置
    /// </summary>
    RestPosition,//
    /// <summary>
    /// 图片分开左右两边，分别流向屏幕外边
    /// </summary>
    ScreenOut//
}