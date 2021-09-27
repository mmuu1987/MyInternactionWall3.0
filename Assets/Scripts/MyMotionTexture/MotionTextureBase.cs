using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTextureBase : TextureBase
{
    public MeshRenderer MeshRenderer { get; private set; }


    private Material _curMaterial;


    private bool _isInitEnd = false;
    


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

    
}
