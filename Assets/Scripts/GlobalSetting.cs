using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using UnityEngine;

public static class GlobalSetting
{

    public static int ScreenWidth;

    public static int ScreenHeight;

    public static int LOG_LEVENL;

    public static string UDPIP = "";

    public static int UDPSendPort = 6001;

    public static int UDPSelfPort = 5999;

    public static int UDPReceivePort = 6000;

    /// <summary>
    /// 容纳图片，图片能在摄像机的范围的宽
    /// </summary>
    public static float ContainerWidth=20f;

    public static float Scale = 1.75f;
    /// <summary>
    /// 容纳的图片，图片能在摄像机的范围的高
    /// </summary>
    public static float ContainerHeight=5f;



    public static List<string> URLS = new List<string>();
    public static void ReadXml()
    {

        try
        {
            XmlDocument doc = new XmlDocument();
            string xmlPath = Application.streamingAssetsPath + "/Setting.xml";
            bool flag = !File.Exists(xmlPath);
            if (flag)
            {
                Debug.Log("没有找到XML文件");
            }
            doc.Load(xmlPath);
            XmlNode selectSingleNode = doc.SelectSingleNode("CommonTag");
            bool flag2 = selectSingleNode != null;
            if (flag2)
            {
                XmlNodeList nodeList = selectSingleNode.ChildNodes;
                foreach (object obj in nodeList)
                {

                    XmlNode item = (XmlNode)obj;
                    if (item.Name == "LOG_LEVENL")
                    {
                        LOG_LEVENL = int.Parse(item.InnerText);
                    }
                    else if (item.Name == "ScreenWidth")
                    {
                        ScreenWidth = int.Parse(item.InnerText);
                    }
                    else if (item.Name == "ScreenHeight")
                    {
                        ScreenHeight = int.Parse(item.InnerText);
                    }
                    else if (item.Name == "UDPip")
                    {
                        UDPIP = item.InnerText;
                    }
                    else if (item.Name == "UDPSendPort")
                    {
                        UDPSendPort = int.Parse(item.InnerText);
                    }
                    else if (item.Name == "UDPSelfPort")
                    {
                        UDPSelfPort = int.Parse(item.InnerText);
                    }
                    else if (item.Name == "UDPReceivePort")
                    {
                        UDPReceivePort = int.Parse(item.InnerText);
                    }
                    else if (item.Name == "ShowQrTime")
                    {
                        ShowQrTime = float.Parse(item.InnerText);
                    }
                    



                    if (item.Name == "URLS")
                    {
                        foreach (object obj2 in item.ChildNodes)
                        {
                            XmlElement element = (XmlElement)obj2;

                            string url = element.InnerText;
                            //Debug.Log(url);

                            GlobalSetting.URLS.Add(url);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
             Debug.LogError(e.ToString());
                throw;
        }
       
    }

    public static IEnumerator WaitEndFrame(Action action)
    {
        yield return new WaitForEndOfFrame();
        if (action != null) action();
    }

    public static IEnumerator WaitTime(float time,Action action)
    {
        yield return new  WaitForSeconds(time);
        if (action != null) action();
    }



    
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    /// <summary>
    /// 缩略图片
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newWidth"></param>
    /// <param name="newHeight"></param>
    /// <returns></returns>
    public static Texture2D Resize(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        var nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        return nTex;
    }
    /// <summary>
    /// 计算在长和宽的矩形里，半径为R的物体能塞多少个，并且不会重叠
    /// </summary>
    /// <param name="width">矩形的长</param>
    /// <param name="height">矩形的宽</param>
    /// <param name="r">小物体半径</param>
    /// <param name="k">计算次数，值越大，越密集，计算量就越大</param>
    /// <returns></returns>
    public static List<Vector2> Sample2D(float width, float height, float r, int k = 30)
    {
        return Sample2D((int)DateTime.Now.Ticks, width, height, r, k);
    }
    public static  Vector2 ShowImage(Vector2 texSize,Vector2 targetSize)
    {
        Vector2 temp = texSize;

        // temp.y -= PictureHandle.Instance.LableHeight;
        //图片的容器的宽高
        Vector2 size = new Vector2(targetSize.x, targetSize.y);
        float v2 = temp.x / temp.y;//图片的比率


        if (temp.x > temp.y)//如果图片宽大于高
        {
            if (temp.x > size.x)//如果图片宽大于容器的宽
            {
                temp.x = size.x;//以容器宽为准

                temp.y = size.x / v2;//把图片高按比例缩小

                if (temp.y > size.y)//如果图片的高还是大于容器的高
                {
                    temp.y = size.y;//则以容器的高为标准

                    temp.x = size.y * v2;//容器的高再度计算赋值

                    //一下逻辑同理
                }
            }
            else //如果图片宽小于容器的宽
            {

                if (temp.y > size.y)//如果图片的高还是大于容器的高
                {
                    temp.y = size.y;//则以容器的高为标准

                    temp.x = size.y * v2;//容器的高再度计算赋值


                }
            }
        }
        else if (temp.x <= temp.y)//如果图片的高大于宽 
        {
            if (temp.y > size.y)//如果图片高大于容器的高
            {
                temp.y = size.y;//以容器的高为准

                temp.x = size.y * v2;//重新计算图片的宽

                if (temp.x > size.x)//如果图片的宽还是大于容器的高
                {

                    temp.x = size.x;//则再次以容器的宽为标准

                    temp.y = size.x / v2;//再以容器的宽计算得到容器的高
                }
            }
            else //如果图片的高小于容器的高
            {
                //但是图片的宽大于容器的宽
                if (temp.x > size.x)
                {
                    temp.x = size.x;//以容器的宽为准
                    temp.y = size.x / v2;//再以容器的宽计算得到容器的高
                }

            }
        }


        //   Vector2 realSize =_yearsEvent.PictureIndes

        Vector2 newSize = new Vector2(temp.x, temp.y);

        return newSize;


    }

    /// <summary>
    /// 根据高度来适配
    /// </summary>
    /// <param name="texSize"></param>
    /// <param name="targetHeight"></param>
    /// <returns></returns>
    public static Vector2 ScaleImageInHeight(Vector2 texSize, float targetHeight)
    {
        Vector2 temp = texSize;

       
      
        float v2 = temp.x / temp.y;//图片的比率


     

        temp.y = targetHeight;//以容器的高为准

        temp.x = targetHeight * v2;//重新计算图片的宽

      


        //   Vector2 realSize =_yearsEvent.PictureIndes

        Vector2 newSize = new Vector2(temp.x, temp.y);

        return newSize;


    }
    /// <summary>
    /// 获得世界空间中长度在屏幕的映射，也就是屏幕长度
    /// </summary>
    public static float GetScreenSizePos(float size)
    {
       



       
        Vector2 leftUpScreenPos = Camera.main.WorldToScreenPoint(new Vector3(0f,0f,0f));


        
        Vector2 RightUpScreenPos = Camera.main.WorldToScreenPoint(new Vector3(size, 0f, 0f));





        float width = RightUpScreenPos.x - leftUpScreenPos.x;

       

       



        return width;





    }
    public static List<Vector2> Sample2D(int seed, float width, float height, float r, int k = 30)
    {
        // STEP 0

        // 维度，平面就是2维
        var n = 2;

        // 计算出合理的cell大小
        // cell是一个正方形，为了保证每个cell内部不可能出现多个点，那么cell内的任意点最远距离不能大于r
        // 因为cell内最长的距离是对角线，假设对角线长度是r，那边长就是下面的cell_size
        var cell_size = r / Math.Sqrt(n);

        // 计算出有多少行列的cell
        var cols = (int)Math.Ceiling(width / cell_size);
        var rows = (int)Math.Ceiling(height / cell_size);

        // cells记录了所有合法的点
        var cells = new List<Vector2>();

        // grids记录了每个cell内的点在cells里的索引，-1表示没有点
        var grids = new int[rows, cols];
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < cols; ++j)
            {
                grids[i, j] = -1;
            }
        }

        // STEP 1
        var random = new System.Random(seed);

        // 随机选一个起始点
        var x0 = new Vector2(random.Next((int)width), random.Next((int)height));
        var col = (int)Math.Floor(x0.x / cell_size);
        var row = (int)Math.Floor(x0.y / cell_size);

        var x0_idx = cells.Count;
        cells.Add(x0);
        grids[row, col] = x0_idx;

        var active_list = new List<int>();
        active_list.Add(x0_idx);

        // STEP 2
        while (active_list.Count > 0)
        {
            // 随机选一个待处理的点xi
            var xi_idx = active_list[random.Next(active_list.Count)]; // 区间是[0,1)，不用担心溢出。
            var xi = cells[xi_idx];
            var found = false;

            // 以xi为中点，随机找与xi距离在[r,2r)的点xk，并判断该点的合法性
            // 重复k次，如果都找不到，则把xi从active_list中去掉，认为xi附近已经没有合法点了
            for (var i = 0; i < k; ++i)
            {
                var dir = UnityEngine.Random.insideUnitCircle;
                var xk = xi + (dir.normalized * r + dir * r); // [r,2r)
                if (xk.x < 0 || xk.x >= width || xk.y < 0 || xk.y >= height)
                {
                    continue;
                }

                col = (int)Math.Floor(xk.x / cell_size);
                row = (int)Math.Floor(xk.y / cell_size);

                if (grids[row, col] != -1)
                {
                    continue;
                }

                // 要判断xk的合法性，就是要判断有附近没有点与xk的距离小于r
                // 由于cell的边长小于r，所以只测试xk所在的cell的九宫格是不够的（考虑xk正好处于cell的边缘的情况）
                // 正确做法是以xk为中心，做一个边长为2r的正方形，测试这个正方形覆盖到所有cell
                var ok = true;
                var min_r = (int)Math.Floor((xk.y - r) / cell_size);
                var max_r = (int)Math.Floor((xk.y + r) / cell_size);
                var min_c = (int)Math.Floor((xk.x - r) / cell_size);
                var max_c = (int)Math.Floor((xk.x + r) / cell_size);
                for (var or = min_r; or <= max_r; ++or)
                {
                    if (or < 0 || or >= rows)
                    {
                        continue;
                    }

                    for (var oc = min_c; oc <= max_c; ++oc)
                    {
                        if (oc < 0 || oc >= cols)
                        {
                            continue;
                        }

                        var xj_idx = grids[or, oc];
                        if (xj_idx != -1)
                        {
                            var xj = cells[xj_idx];
                            var dist = (xj - xk).magnitude;
                            if (dist < r)
                            {
                                ok = false;
                                goto end_of_distance_check;
                            }
                        }
                    }
                }

                end_of_distance_check:
                if (ok)
                {
                    var xk_idx = cells.Count;
                    cells.Add(xk);

                    grids[row, col] = xk_idx;
                    active_list.Add(xk_idx);

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                active_list.Remove(xi_idx);
            }
        }

        return cells;
    }

    // not used rigth now
    //const uint SWP_NOMOVE = 0x2;
    //const uint SWP_NOSIZE = 1;
    //const uint SWP_NOZORDER = 0x4;
    //const uint SWP_HIDEWINDOW = 0x0080;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);


    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);


    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// 最小化
    /// </summary>
    const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}  
    /// <summary>
    /// 最大化
    /// </summary>
    const int SW_SHOWMAXIMIZED = 3;//最大化  
    const int SW_SHOWRESTORE = 1;//还原  

    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;

    private static IntPtr _curIntPtr;

    public static byte[] HexStringToByteArray(string s)
    {
        if (s.Length == 0)
            throw new Exception("将16进制字符串转换成字节数组时出错，错误信息：被转换的字符串长度为0。");
        s = s.Replace(" ", "");
        byte[] buffer = new byte[s.Length / 2];
        for (int i = 0; i < s.Length; i += 2)
            buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
        return buffer;
    }

    public static void GetWinPtr()
    {
        _curIntPtr = GetForegroundWindow();
    }
    /// <summary>
    /// 设置窗口的最小化，最大化
    /// </summary>
    /// <param name="isSamell"></param>
    public static void SetWindow(bool isSamell)
    {
        

        if (!isSamell)
        {
            ShowWindow(_curIntPtr, SW_SHOWMAXIMIZED);
        }
        else
        {

            ShowWindow(_curIntPtr, SW_SHOWMINIMIZED);
        }
    }


    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetSystemMetrics(int nIndex);
    public static int SM_CXSCREEN = 0; //主屏幕分辨率宽度
    public static int SM_CYSCREEN = 1; //主屏幕分辨率高度
    public static int SM_CYCAPTION = 4; //标题栏高度
    public static int SM_CXFULLSCREEN = 16; //最大化窗口宽度（减去任务栏）
    public static int SM_CYFULLSCREEN = 17; //最大化窗口高度（减去任务栏）

    public static float ShowQrTime { get; private set; }
}
