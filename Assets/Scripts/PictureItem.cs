using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  DG.Tweening;
using Random = UnityEngine.Random;

public class PictureItem : MonoBehaviour
{

    private PictureInfo _pictureInfo;

    public MeshRenderer MeshRenderer { get; private set; }

    private  float _target = 0.5f;

    public  float CurTarget = 0f;

    private bool _isFront = false;

    /// <summary>
    /// 是否是新海报
    /// </summary>
    private bool _isNew = false;

    private float _animationTime = 0.55f;

    private MaterialPropertyBlock _materialPropertyBlock;

    public int PictureId;

    private Tween _tween;

    /// <summary>
    /// 缓存用的
    /// </summary>
    private Transform _cacheTransform;

    public float MoveSpeed = -0f;

    public Vector3 TargetPos;

    public Transform QrTransform;
    /// <summary>
    /// 原本的缩放比例
    /// </summary>
    private Vector3 _orinigalScale;

    /// <summary>
    /// 初始化的时候的原本的位置
    /// </summary>
    private Vector3 _oriniglaPos;

    /// <summary>
    /// 整体的缩放倍数
    /// </summary>
    private float _scale = 1.0f;

    /// <summary>
    /// 后排需要强制整体缩放的倍数
    /// </summary>
    private float _scaleBack = 0.75f;

    /// <summary>
    /// 原本的尺寸
    /// </summary>
    private Vector2 _orinigalSize;

    /// <summary>
    /// 是否显示二维码
    /// </summary>
    private bool _isQr;


    /// <summary>
    /// 是否触发了动画点
    /// </summary>
    private bool _isTriggerPoint;

    
   

    private void Awake()
    {
        MeshRenderer = this.GetComponent<MeshRenderer>();


        
        _isTriggerPoint = false;
    }
    // Start is called before the first frame update
    void Start()
    {

        MoveSpeed = -0f;
        _isQr = false;
    }

    // Update is called once per frame
    void Update()
    {
       HandleMat();

       if (Math.Abs(MoveSpeed) > 0.0000001f)
       {
           Vector3 pos = _cacheTransform.position + new Vector3(MoveSpeed, 0f, 0f);
           _cacheTransform.position = pos;
           if (Math.Abs(pos.y - TargetPos.y) < Mathf.Epsilon)
           {
               if (!_isQr)
               {
                   if (MoveSpeed < 0)
                   {
                       if (pos.x <= TargetPos.x)
                       {
                           //触发旋转,翻转到二维码
                           Rotation(180f);
                           _isQr = true;
                           StartCoroutine(GlobalSetting.WaitTime(10f, (() =>
                           {
                               Rotation(0f);
                              
                           })));
                       }
                   }
                   else
                   {
                       if (pos.x >= TargetPos.x)
                       {
                           //触发旋转,翻转到二维码
                           Rotation(180f);
                           _isQr = true;
                            StartCoroutine(GlobalSetting.WaitTime(10f, (() =>
                           {
                               Rotation(0f);
                           })));
                       }
                   }
               }
             

              
           }

       }


       if (_cacheTransform.position.x <= -GlobalSetting.ContainerWidth/2-2 && MoveSpeed<0)
       {
           _cacheTransform.position= new Vector3(GlobalSetting.ContainerWidth / 2  + GlobalSetting.ContainerWidth/2, _cacheTransform.position.y,_cacheTransform.position.z);
           Rotation(0f);
       }
       else if (_cacheTransform.position.x >= GlobalSetting.ContainerWidth / 2 + 2 && MoveSpeed > 0)
       {
            _cacheTransform.position = new Vector3(-GlobalSetting.ContainerWidth / 2 + -GlobalSetting.ContainerWidth/2, _cacheTransform.position.y, _cacheTransform.position.z);
            Rotation(0f);
       }

       UpdateQr();

    }

    private void UpdateQr()
    {
        if (QrTransform != null)
        {
            this.QrTransform.position = this.transform.position;
            this.QrTransform.rotation = this.transform.rotation*Quaternion.Euler(0f,180f,0f);
        }
     
    }

    private void Rotation(float angel)
    {
        _cacheTransform.DORotateQuaternion( Quaternion.Euler(0f, angel, 0f), 0.55f);
    }
    private void HandleMat()
    {
        if (_tween != null)
        {
            //if(_pictureId==1)
            // Debug.Log("  CurTarget " + CurTarget);
            _materialPropertyBlock.SetFloat("_Flag", CurTarget);
            _materialPropertyBlock.SetInt("_Index",PictureId);
            _materialPropertyBlock.SetFloat("_Width", _orinigalSize.x);
            _materialPropertyBlock.SetFloat("_Height", _orinigalSize.y);
            //_materialPropertyBlock.SetTexture("_MainTex", PictureManager.Instance.texs[PictureId]);
            MeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
    public void Move(float delay,float speed)
    {
      

         _tween =DOTween.To(() => MoveSpeed, x => MoveSpeed = x, speed, 2f).SetDelay(delay).OnComplete((() =>
         {
             _tween = null;
         }));


    }

    /// <summary>
    /// 做特效画面
    /// </summary>
    public void DoEffect(float point)
    {

        if (point <= -1000f)//重置开关
        {
            _isTriggerPoint = false;
            return;
        }
       

        if (!_isTriggerPoint)
        {
            if (point >= this._cacheTransform.position.x)
            {
                _isTriggerPoint = true;
                this.transform.DOLocalMove(new Vector3(0f, 0.35f, 0f), 0.25f).OnComplete((() =>
                {
                    this.transform.DOLocalMove(new Vector3(0f, -0.35f, 0f), 0.25f).OnComplete((() =>
                    {
                        this.transform.DOLocalMove(Vector3.zero, 0.25f);
                    }));
                }));

                this.transform.DOScale(Vector3.one * 0.15f, 0.25f).SetEase(Ease.InOutQuad).OnComplete((() =>
                {
                    this.transform.DOScale(_orinigalScale * _scale, 0.25f).SetEase(Ease.InOutQuad);
                }));
            }
        }

    }
    /// <summary>
    /// 放大显示海报，如果是条件符合的海报，则返回true
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isLeft"></param>
    /// <returns></returns>
    public bool ShowInfo(Vector3 target)
    {
        //if (id != PictureId)
        //{
        //    if (!_isShowEnd)
        //    {
        //         RestPos();
        //    }
        //    return false;

        //}

        this.transform.parent = null;

       

       

        _tween = DOTween.To(() => CurTarget, x => CurTarget = x, 0, _animationTime).OnComplete((() =>
        {
            _tween = null;
        }));

        if (!_isNew)
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
       
        Vector3 moveTarget = target;


        this.transform.DOScale(_orinigalScale * 2f, 1f);
        this.transform.DOMove(moveTarget, 01f).OnComplete((() =>
        {
        }));

       

        return true;

    }

    public void RestCachePos()
    {
        MoveSpeed = 0f;
        _cacheTransform.transform.DOMove(_oriniglaPos, 0.55f).SetEase(Ease.InOutQuad);
        Rotation(0);
        _isQr = false;
    }
    /// <summary>
    /// 回到原来的地方
    /// </summary>
    public void RestPos()
    {
        this.transform.DOKill();
        this.transform.parent = _cacheTransform;
        this.transform.DOLocalMove(Vector3.zero, 01f);
        this.transform.DOScale(_orinigalScale * _scale, 1f);
        this.transform.DOLocalRotate(Vector3.zero, 1f);
       


    }

    private Vector2 ShowImage(Vector2 texSize)
    {
        Vector2 temp = texSize;

        // temp.y -= PictureHandle.Instance.LableHeight;
        //图片的容器的宽高
        Vector2 size = new Vector2(1024f, 1024f);
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
    public void SetInfo(PictureInfo info,bool isFront,MaterialPropertyBlock prop)
    {

       // this.transform.position = new Vector3(pos.x-9.5f, pos.y-2.4125f, this.transform.position.z);

        _cacheTransform = new GameObject(info.Name).transform;

        _cacheTransform.position = this.transform.position;

        _oriniglaPos = this.transform.position;

        _cacheTransform.parent = this.transform.parent;

        this.transform.parent = _cacheTransform;

        
        _materialPropertyBlock = prop;
        //默认初始化的时候前面的就是新海报
        _isFront = isFront;
        _isNew = isFront;
        PictureId = info.Index;
        _scaleBack = 0.75f;

        _scale = 1f;//图片随机缩放

         _orinigalSize = ShowImage(info.Size);

        float scaleWidth = _orinigalSize.x / 1024f;

        float scaleHeight = _orinigalSize.y / 1024f;
        _orinigalScale = new Vector3(scaleWidth, scaleHeight, 1f);

        if (_isFront)
        {
            CurTarget = 0f;
            _target = 0f;
            this.transform.localScale = _orinigalScale * _scale;
        }
        else
        {
            CurTarget = 1f;
            _target = 1f;
            this.transform.localScale = _orinigalScale * _scaleBack* _scale;

        }

        //Debug.Log(n);
        _materialPropertyBlock.SetInt("_Index", PictureId);
        _materialPropertyBlock.SetFloat("_Flag", _target);
        _materialPropertyBlock.SetFloat("_Width", _orinigalSize.x);
        _materialPropertyBlock.SetFloat("_Height", _orinigalSize.y);
        //_materialPropertyBlock.SetTexture("_MainTex", PictureManager.Instance.texs[PictureId]);
        MeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        //props.SetColor("_Color", Random.ColorHSV());

        _pictureInfo = info;

       

        this.name = PictureId.ToString();

       

       
       
       

    }

}
