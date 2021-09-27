using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Circular : MonoBehaviour
{
    /// <summary>
    /// 圆形的图片
    /// </summary>
    private RawImage tex;
    /// <summary>
    /// 碰撞的半径
    /// </summary>
    public float Radius;



    /// <summary>
    /// 第二半径减去第一半径的差值
    /// </summary>
    private float _differenceValue;
    /// <summary>
    /// 第二半径减去第一半径的差值
    /// </summary>
    public float DifferenceValue
    {
        get { return _differenceValue; }

    }

    private Transform _cacheTransform;

    public Transform CacheTransform
    {
        get { return _cacheTransform; }

    }

    void Awake()
    {


        _cacheTransform = this.transform;

        tex = this.GetComponent<RawImage>();

        Vector2 size = this.transform.localScale;

       this.transform.localScale = Vector3.zero;
        Radius = 0f;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        ZoomSize();
    }
    /// <summary>
    /// 放大球体
    /// </summary>
    void ZoomSize()
    {
        Radius = Mathf.Lerp(Radius, 3, Time.deltaTime * 5f);
        Vector3 size = this.transform.localScale;

        float x = Mathf.Lerp(size.x, 5, Time.deltaTime * 5f);
        this.transform.localScale= new Vector3(x,size.y,size.z);
    }
}
