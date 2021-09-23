using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 把quad 的在3d空间的size转为屏幕尺寸,目前用transform.localscale代表像素尺寸
/// 默认Z轴的位置为0
/// </summary>
public class ConvertPixel : MonoBehaviour
{

    private Transform _cacheTransform;

    private bool _isInitEnd = false;

    private Material _curMaterial;

    

    private MaterialPropertyBlock _materialPropertyBlock;

    /// <summary>
    /// 初始化的时候的原本的位置
    /// </summary>
    private Vector3 _oriniglaPos;

    private PictureInfo _pictureInfo;


    public MeshRenderer MeshRenderer { get; private set; }

    public int PictureId;

   
    /// <summary>
    /// 原本的缩放比例
    /// </summary>
    private Vector3 _orinigalSize;

    /// <summary>
    /// 该面片所代表的的屏幕尺寸
    /// </summary>
    public Vector2 ScreenSize { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
      //  Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Material mat)
    {
        if (_isInitEnd) return;
        _isInitEnd = true;
        _cacheTransform = this.transform;



        if (Math.Abs(_cacheTransform.localScale.z - 1f) > Mathf.Epsilon)
        {
            throw new UnityException("目前仅支持z缩放为1");
        }

        MeshRenderer = this.GetComponent<MeshRenderer>();

        _curMaterial = mat;
        MeshRenderer.material = mat;
       
      
    }

    
    /// <summary>
    /// 获取该面片的屏幕尺寸,和屏幕位置
    /// </summary>
    public Rect GetScreenSizePos()
    {
        Vector3 worldPos = _cacheTransform.position;

        Vector3 worldSize = _cacheTransform.localScale;

        //转化到屏幕空间的Rect
        Vector2 screenCenterPos = Camera.main.WorldToScreenPoint(worldPos);



        Vector2 leftUpPosWorld = worldPos + new Vector3(-worldSize.x / 2, worldSize.y / 2);
        Vector2 leftUpScreenPos = Camera.main.WorldToScreenPoint(leftUpPosWorld);


        Vector2 RightUpPosWorld = worldPos + new Vector3(worldSize.x / 2, worldSize.y / 2);
        Vector2 RightUpScreenPos = Camera.main.WorldToScreenPoint(RightUpPosWorld);


        Vector2 RightDownPosWorld = worldPos + new Vector3(worldSize.x / 2, -worldSize.y / 2);
        Vector2 RightDownScreenPos = Camera.main.WorldToScreenPoint(RightDownPosWorld);



        float width = RightUpScreenPos.x - leftUpScreenPos.x;

        float height = RightUpScreenPos.y - RightDownScreenPos.y;

        Rect screenRect = new Rect(screenCenterPos,new Vector2(width,height));



        return screenRect;





    }
    /// <summary>
    /// 根据屏幕位置设置quad的3D位置
    /// </summary>
    /// <param name="screenPos"></param>
    public void SetScreenPos(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(Camera.main.transform.position.z)));

        _cacheTransform.transform.position = worldPos;
    }
    /// <summary>
    /// 根据像素尺寸，设置quad的3D大小
    /// </summary>
    /// <param name="size"></param>

    public void SetScreenSize(Vector2 size)
    {
        //先计算屏幕 0 0点在世界坐标的位置
        Vector2 zeroWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(0f,0f, Mathf.Abs(Camera.main.transform.position.z)));

        //Debug.Log("zeroWorldPos is " +zeroWorldPos);
        //再计算这个尺寸在世界的位置
        Vector2 sizeWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(size.x, size.y, Mathf.Abs(Camera.main.transform.position.z)));

        float worldScaleX = Mathf.Abs( sizeWorldPos.x - zeroWorldPos.x);

        float worldScaleY = Mathf.Abs(sizeWorldPos.y - zeroWorldPos.y);

        this._cacheTransform.localScale = new Vector3(worldScaleX,worldScaleY,1);//默认缩放为1


        ScreenSize = size;
    }

  
    /// <summary>
    /// 设置quad的屏幕位置和尺寸
    /// </summary>
    /// <param name="rect"></param>
    public void SetPosSize(Rect rect,Vector2 space)
    {
        SetScreenPos(rect.position);



        Vector2 size;

        if (rect.size.x > space.x && rect.size.y > space.y)
            size = new Vector2(rect.size.x - space.x, rect.size.y - space.y);
        else size = rect.size;

        SetScreenSize(size);
    }

    public void SetInfo(PictureInfo info,  MaterialPropertyBlock prop )
    {
        _cacheTransform = new GameObject(info.Name).transform;

        _cacheTransform.position = this.transform.position;

        _oriniglaPos = this.transform.position;

        _cacheTransform.parent = this.transform.parent;

        this.transform.parent = _cacheTransform;


        _materialPropertyBlock = prop;
        //默认初始化的时候前面的就是新海报
       
       
        PictureId = info.Index;




        _orinigalSize = this.transform.localScale;

     

        //Debug.Log(n);
        _materialPropertyBlock.SetInt("_Index", PictureId);
        _materialPropertyBlock.SetFloat("_Flag", 0);
        _materialPropertyBlock.SetFloat("_Width", _orinigalSize.x);
        _materialPropertyBlock.SetFloat("_Height", _orinigalSize.y);
       
        MeshRenderer.SetPropertyBlock(_materialPropertyBlock);
      

        _pictureInfo = info;



        this.name = PictureId.ToString();
    }

#if UNITY_EDITOR
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
    //    {
    //       // Debug.Log(GetScreenSize().ToString());

    //       SetScreenPos(new Vector2(1920f,0f));

    //       SetScreenSize(new Vector2(100, 1080));
    //    }
    //}

#endif
}
