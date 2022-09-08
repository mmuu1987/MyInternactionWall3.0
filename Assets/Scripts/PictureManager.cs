using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;

using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PictureManager : MonoBehaviour
{

    public static PictureManager Instance;

  

    public ComputeShader ScaleImageComputeShader;

    public GameObject ListGameobjectsOld;

   

    public Material Mat;

    public List<PictureItem> OldMeshRenderers = new List<PictureItem>();

   

    public List<PictureInfo> PictureInfo = new List<PictureInfo>();

   

    public List<Texture2D> texs = new List<Texture2D>();

   

  

    private List<int> _queueList = new List<int>();

   

    private Tween _tween;

    private float _animationPoint=-10f;

    private void Awake()
    {

        Instance = this;
        GlobalSetting.GetWinPtr();
        GlobalSetting.ReadXml();

        Screen.SetResolution(5120,1440,true);
       
    }

    private void RecevieDataEvent(string obj)
    {

        Debug.Log("收到的消息是：" +obj);
        if (!string.IsNullOrEmpty(obj))
        {
            if (obj.Contains("*_*"))
            {
                string[] strs = obj.Split(new[] { "*_*" }, StringSplitOptions.None);

                bool isLeft = strs[0] != "false";

                int id = int.Parse(strs[1]) - 1;//传过来的id是图片的，从1开始，所以我们要按照数组习惯，减去1

               // ShowInfo(id, isLeft);
            }
            else
            {
                switch (obj)
                {
                    case "min":
                        GlobalSetting.SetWindow(true);
                        break;
                    case "max":
                        GlobalSetting.SetWindow(false);
                        break;

                }
            }
           

        }
    }
    public List<Transform> TargetPosList = new List<Transform>();
    // Use this for initialization
    void Start()
    {
       

        PictureItem[] mr1 = ListGameobjectsOld.GetComponentsInChildren<PictureItem>();

        OldMeshRenderers.AddRange(mr1);

      

        for (int i = 0; i < 100; i++)
        {
            PictureItem parent = OldMeshRenderers[i];

         

            if (i <= 19)
            {
                parent.TargetPos = TargetPosList[0].position;
            }
            else if ( i <= 39)
            {
                parent.TargetPos = TargetPosList[1].position;
            }
            else if (i <= 59)
            {
                parent.TargetPos = TargetPosList[2].position;
            }
            else if (i <= 79)
            {
                parent.TargetPos = TargetPosList[3].position;
            }
            else if (i <= 99)
            {
                parent.TargetPos = TargetPosList[4].position;
            }
        }
      
        //PictureItem[] mr2 = ListGameobjectsNew.GetComponentsInChildren<PictureItem>();

        //NewMeshRenderers.AddRange(mr2);

        _queueList.Add(1);

        _queueList.Add(2);

        _queueList.Add(3);

        _queueList.Add(4);

        _queueList.Add(5);

        _queueList.Add(6);

        _queueList.Add(7);

        _queueList.Add(8);

        _queueList.Add(9);

        _queueList.Add(10);


       

        Init();

        StartCoroutine(AutoDO());

        Screen.SetResolution(5120,1440,true);

        //StartCoroutine(WaitTime());
    }

  
    // Update is called once per frame
    void Update()
    {
        if (_tween != null)
        {
            foreach (PictureItem item in OldMeshRenderers)
            {
                item.DoEffect(_animationPoint);
            }
        }
    }
    public Texture2D ScaleImageUserRt(Texture2D sourceTexture2D,Vector2 targetSize)
    {

        float widthScale =  targetSize.x/ sourceTexture2D.width;

        float heightScale =   targetSize.y/ sourceTexture2D.height;

        RenderTexture rtDes = new RenderTexture((int)(sourceTexture2D.width * widthScale), (int)(sourceTexture2D.height * heightScale), 24);
        rtDes.enableRandomWrite = true;
        rtDes.Create();
        ////////////////////////////////////////
        //    Compute Shader
        ////////////////////////////////////////
        //1 找到compute shader中所要使用的KernelID
        int k = ScaleImageComputeShader.FindKernel("CSMain");

        ScaleImageComputeShader.SetTexture(k, "Source", sourceTexture2D);
        ScaleImageComputeShader.SetTexture(k, "Dst", rtDes);
        ScaleImageComputeShader.SetFloat("widthScale", widthScale);
        ScaleImageComputeShader.SetFloat("heightScale", heightScale);

        //3 运行shader  参数1=kid  参数2=线程组在x维度的数量 参数3=线程组在y维度的数量 参数4=线程组在z维度的数量
        ScaleImageComputeShader.Dispatch(k, (int)(sourceTexture2D.width * widthScale), (int)(sourceTexture2D.height * heightScale), 1);

        //cumputeShader gpu那边已经计算完毕。rtDes是gpu计算后的结果
        
       


        //RenderTexture.active = rtDes;
        RenderTexture.active = rtDes;

        Texture2D tex = new Texture2D((int)targetSize.x, (int)targetSize.y, TextureFormat.ARGB32,false);

        tex.ReadPixels(new Rect(0, 0, rtDes.width, rtDes.height), 0, 0);

        tex.Apply();
        
        Destroy(rtDes);
        //Destroy(sourceTexture2D);

        return tex;
        //后续操作，把reDes转为Texture2D  
        //删掉rtDes,SourceTexture2D，我们就得到了所要的目标，并且不产生内存垃圾
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

                tex = ScaleImageUserRt(tex, new Vector2(1024, 1024));

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

                var  tex = ScaleImageUserRt(texture2D, new Vector2(1024, 1024));



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


        //}

        //texArr.Apply(false);

        //Debug.Log("Texture2DArray.currentTextureMemory  " + Texture2DArray.currentTextureMemory/1024/1024+" M");
        //Debug.Log("Texture2DArray.desiredTextureMemory  " + Texture2DArray.desiredTextureMemory / 1024 / 1024 + " M");
        //Debug.Log("Texture2DArray.nonStreamingTextureMemory  " + Texture2DArray.nonStreamingTextureMemory / 1024 / 1024 + " M");
        //Debug.Log("Texture2DArray.totalTextureMemory  " + Texture2DArray.totalTextureMemory / 1024 / 1024 + " M");

        texArr.wrapMode = TextureWrapMode.Clamp;
        texArr.filterMode = FilterMode.Trilinear;

        Mat.SetTexture("_TexArr", texArr);

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


    private void Init()
    {
        LoadImage();

        MaterialPropertyBlock props = new MaterialPropertyBlock();

        List<Vector2> _randPos1 = GetVector2(1);

        List<Vector2> _randPos2 = GetVector2(2);

        int k= 2;
        while (true)
        {
            if (_randPos2.Count < 100)
            {
                _randPos2 = GetVector2(k);
                k++;
            }
            else break;
        }

        k++;
        while (true)
        {
            if (_randPos1.Count < 100)
            {
                _randPos1 = GetVector2(k);
                k++;
            }
            else break;
        }

        int n = 0;
        foreach (PictureItem meshRenderer in OldMeshRenderers)
        {
            meshRenderer.MeshRenderer.material = Mat;
            meshRenderer.GetComponent<PictureItem>().SetInfo(PictureInfo[n], true, props);
            n++;
            if (n >= PictureInfo.Count) n = 0;

        }


        _doubleMove.Add(SelectorList(1),null);
        _doubleMove.Add(SelectorList(2), null);
        _doubleMove.Add(SelectorList(3), null);
        _doubleMove.Add(SelectorList(4), null);
        _doubleMove.Add(SelectorList(5), null);
     

    }

    

    public void DoEffect()
    {
        _animationPoint = -10f;
        

        //重置数据
        foreach (PictureItem item in OldMeshRenderers)
        {
            item.DoEffect(-2000f);
        }

        _tween = DOTween.To(() => _animationPoint, x => _animationPoint = x, 10, 1f).SetEase(Ease.InOutQuad).OnComplete((() =>
        {
            _tween = null;
        }));
    }
    private List<Vector2> GetVector2(int seed)
    {
        List<Vector2> temp =new List<Vector2>();

        temp= GlobalSetting.Sample2D(seed,GlobalSetting.ContainerWidth*GlobalSetting.Scale, GlobalSetting.ContainerHeight-0.5f, 1f,50);

        Debug.Log("个数是 "+ temp.Count);
        return temp;
    }
   

    private Dictionary<List<PictureItem>,List<PictureItem>> _doubleMove = new Dictionary<List<PictureItem>, List<PictureItem>>();
    private List<PictureItem> SelectorList(int index)
    {
        switch (index)
        {
            case 1:
               return OldMeshRenderers.GetRange(0, 20);
                break;
            case 2:
                return OldMeshRenderers.GetRange(20, 20);
                break;
            case 3:
                return OldMeshRenderers.GetRange(40, 20);
                break;
            case 4:
                return OldMeshRenderers.GetRange(60, 20);
                break;
            case 5:
                return OldMeshRenderers.GetRange(80, 20);
                break;
            //case 6:
            //    return OldMeshRenderers.GetRange(50, 20);
            //    break;
            //case 7:
            //    return OldMeshRenderers.GetRange(60, 20);
            //    break;
            //case 8:
            //    return OldMeshRenderers.GetRange(70, 20);
            //    break;
            //case 9:
            //    return OldMeshRenderers.GetRange(80, 20);
            //    break;
            //case 10:
               // return OldMeshRenderers.GetRange(90, 20);
               // break;
        }

        return null;
    }


    private void Move()
    {
        List<float> randTimes= new List<float>();//随机时间 启动移动
        List<float> randSpeeds = new List<float>();//随机速度 启动移动的速度
        List<float> randDelayTimes = new List<float>();//随机延迟启动后排海报的时间
        for (int i = 0; i < 10; i++)
        {
            randTimes.Add(Random.Range(0, 3f));
            randSpeeds.Add(Random.Range(-0.04f, -0.05f));
            randDelayTimes.Add(Random.Range(0.5f, 2f));
        }

        int n = 0;
        foreach (KeyValuePair<List<PictureItem>, List<PictureItem>> pair in _doubleMove)
        {

            float randTime = randDelayTimes[n];
            float randSpeed = randSpeeds[n];
            float randDelayTime = randDelayTimes[n];
            
            if (n == 0)
            {
                randSpeed = Random.Range(-0.0005f, -0.001f);
            }
            else if (n == 1)
            {
                randSpeed = -Random.Range(-0.0005f, -0.001f);
            }
            else if (n == 2)
            {
                randSpeed = Random.Range(-0.0005f, -0.001f);
            }
            else if (n == 3)
            {
                randSpeed = -Random.Range(-0.0005f, -0.001f);
            }
            else if (n == 4)
            {
                randSpeed = Random.Range(-0.0005f, -0.001f);
            }

       
            StartCoroutine(Move(pair.Key, null, randDelayTime, randTime, randSpeed));
            n++;
        }

      

    }

    public IEnumerator Move(List<PictureItem> itemsFront, List<PictureItem> itemsBack,float delay,float randTime,float randSpeed)
    {
        Move(randTime, randSpeed, itemsFront);
        yield return new WaitForSeconds(delay);
        Move(randTime, randSpeed, itemsBack);
    }

    public void Move(float randTime, float randSpeed,List<PictureItem> itemsFront)
    {
        if (itemsFront == null) return;

        foreach (PictureItem item in itemsFront)
        {
            item.Move(randTime, randSpeed);
        }
    }

    private IEnumerator AutoDO()
    {
        while (true)
        {
            DoEffect();
            yield return new WaitForSeconds(3f);

            DoEffect();
            yield return new WaitForSeconds(3f);

            DoEffect();
            yield return new WaitForSeconds(3f);

            Move();
            Debug.Log(GlobalSetting.ShowQrTime);
            yield return new WaitForSeconds(GlobalSetting.ShowQrTime);

            foreach (PictureItem item in OldMeshRenderers)
            {
                item.RestCachePos();
            }
          
            yield return new WaitForSeconds(20f);
        }
    }
   
#if UNITY_EDITOR_WIN
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
    //    {
    //        Move();
    //    }

    //    if (GUI.Button(new Rect(150f, 0f, 100f, 100f), "test1"))
    //    {

    //        foreach (PictureItem item in OldMeshRenderers)
    //        {
    //            item.RestCachePos();
    //        }
    //        foreach (SpriteRenderer spriteRenderer in SpriteRenderers)
    //        {
    //            spriteRenderer.DOFade(0f, 0.65f);
    //        }

           
    //    }

    //    if (GUI.Button(new Rect(300f, 0f, 100f, 100f), "test2"))
    //    {
    //        DoEffect();
    //    }
    //}
#endif


    private void OnDestroy()
    {
       
    }
    
}


public class PictureInfo
{

    public PictureInfo(string name,int index,Vector2 size)
    {
        Name = name;
        Index = index;
        Size = size;
    }
    public string Name;

    public int Index;

    public Vector2 Size;

    public override string ToString()
    {
        string str = "\r\n";
        str += "PictureName is "+Name+"\r\n";
        str += "Index is " + Index + "\r\n";
        str += "Size is " + Size + "\r\n";
        return str;
    }
}