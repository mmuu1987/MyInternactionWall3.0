using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责表现初始化数据
/// </summary>
public class MotionTextureBase : TextureBase
{
    public MeshRenderer MeshRenderer { get; private set; }


    private Material _curMaterial;


    private bool _isInitEnd = false;

    /// <summary>
    /// 所在的列
    /// </summary>
    public int Column;

    /// <summary>
    /// 所在的横
    /// </summary>
    public int Row;

    /// <summary>
    /// 初始化的时候的原本的位置
    /// </summary>
    protected Vector3 _oriniglaPos;

    protected PictureInfo PictureInfo;

    /// <summary>
    /// 图片所在的行
    /// </summary>
    private int _indexColumn;
    /// <summary>
    /// 图片所在的列
    /// </summary>
    private int _indexRow;

    /// <summary>
    /// 图片所在横的最远图片的位置，分辨率为单位
    /// </summary>
    private float _maxScreenPos;


    public int PictureId;


    /// <summary>
    /// 原本的缩放比例
    /// </summary>
    protected Vector3 OrinigalSize;


    protected MaterialPropertyBlock MaterialPropertyBlock;
    /// <summary>
    /// 缩放系数
    /// </summary>
    protected Vector3 Scale = Vector3.zero;

    // protected MaterialPropertyBlock _materialPropertyBlock;
    public virtual void Init(Material mat)
    {
        if (_isInitEnd) return;
        _isInitEnd = true;
        CacheTransform = this.transform;



        if (Math.Abs(CacheTransform.localScale.z - 1f) > Mathf.Epsilon)
        {
            throw new UnityException("目前仅支持z缩放为1");
        }

        MeshRenderer = this.GetComponent<MeshRenderer>();

        _curMaterial = mat;
        MeshRenderer.material = mat;


    }

    public virtual void SetInfo(PictureInfo info, MaterialPropertyBlock prop, int row, int column)
    {
        Column = column;

        Row = row;
      
        _oriniglaPos = this.transform.position;

        MaterialPropertyBlock = prop;
        //默认初始化的时候前面的就是新海报  

        PictureId = info.Index;

        OrinigalSize = this.transform.localScale;

        Scale = Vector3.zero;

        //Debug.Log(n);
        MaterialPropertyBlock.SetInt("_Index", PictureId);
        MaterialPropertyBlock.SetFloat("_Flag", 0);
        MaterialPropertyBlock.SetFloat("_Width", OrinigalSize.x);
        MaterialPropertyBlock.SetFloat("_Height", OrinigalSize.y);

        MeshRenderer.SetPropertyBlock(MaterialPropertyBlock);

        PictureInfo = info;

      //  this.name = (Row * column).ToString();
    }

    
}
