using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

/// <summary>
/// 2d屏幕转3D空间管理
/// </summary>
public class ScreenToWorldManager : MonoBehaviour
{

    /// <summary>
    /// 屏幕中的点
    /// </summary>
    public Vector2 ScreenPos;

    public Vector2 Size;

    public Image TestImage;

    public VisualEffect VisualEffect;

    private void Init()
    {
       // _screenPos = new Vector2(1920f,600f);


    }

    private Rect3D ScreenToWorld(float camDis,Vector2 size)
    {

        Vector3 centerScreen = new Vector3(ScreenPos.x, ScreenPos.y, camDis);
        Vector3 center = Camera.main.ScreenToWorldPoint(centerScreen);

        Debug.Log("中心点的世界位置： "+center);

        Vector3 leftTopScreen = new Vector3(ScreenPos.x - size.x/2f,ScreenPos.y+size.y/2f,camDis);
        Vector3 leftTop = Camera.main.ScreenToWorldPoint(leftTopScreen);

        Vector3 leftDownScreen = new Vector3(ScreenPos.x - size.x / 2f, ScreenPos.y - size.y / 2f, camDis);
        Vector3 leftDown = Camera.main.ScreenToWorldPoint(leftDownScreen);

        Vector3 rightDownScreen = new Vector3(ScreenPos.x + size.x / 2f, ScreenPos.y - size.y / 2f, camDis);
        Vector3 rightDown = Camera.main.ScreenToWorldPoint(rightDownScreen);

        Vector3 rightUpScreen = new Vector3(ScreenPos.x + size.x / 2f, ScreenPos.y + size.y / 2f, camDis);
        Vector3 rightUp = Camera.main.ScreenToWorldPoint(rightUpScreen);

        float worldWidth =  rightDown.x - leftDown.x;

        float worldHeight = leftTop.y - leftDown.y;

        Rect3D rect3D = new Rect3D();

        rect3D.CenterPos = center;

        rect3D.WorldWidth = worldWidth;
        rect3D.WorldHeight = worldHeight;

        Debug.Log("世界位置的尺寸是： " + worldWidth  +"   "+ worldHeight);

        return rect3D;

    }
    private List<GameObject> _gos = new List<GameObject>();
#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
        {
            VisualEffect.enabled = false;

            foreach (GameObject go in _gos)
            {
                Destroy(go);
            }

            _gos.Clear();

            TestImage.rectTransform.anchoredPosition = ScreenPos;
            TestImage.rectTransform.sizeDelta = Size;

            Rect3D rect3D = ScreenToWorld(610f, Size);

            GameObject center = GameObject.CreatePrimitive(PrimitiveType.Cube);
            center.name = "Center";
            center.transform.position = rect3D.CenterPos;

            GameObject leftTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftTop.name = "leftTop";
            leftTop.transform.position = rect3D.CenterPos+new Vector3(-rect3D.WorldWidth/2,rect3D.WorldHeight/2,0f); ;//
            leftTop.transform.localScale = Vector3.one * 10f;

            GameObject leftDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftDown.name = "leftDown";
            leftDown.transform.position = rect3D.CenterPos + new Vector3(-rect3D.WorldWidth / 2, -rect3D.WorldHeight / 2, 0f);
            leftDown.transform.localScale = Vector3.one * 10f;

            GameObject rightDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightDown.name = "rightDown";
            rightDown.transform.position = rect3D.CenterPos + new Vector3(rect3D.WorldWidth / 2, -rect3D.WorldHeight / 2, 0f);
            rightDown.transform.localScale = Vector3.one * 10f;

            GameObject rightUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightUp.name = "rightUp";
            rightUp.transform.position = rect3D.CenterPos + new Vector3(rect3D.WorldWidth / 2, rect3D.WorldHeight / 2, 0f);
            rightUp.transform.localScale = Vector3.one * 10f;

            _gos.Add(leftDown);
            _gos.Add(leftTop);
            _gos.Add(rightDown);
            _gos.Add(rightUp);
            _gos.Add(center);


            //Debug.Log( "左下 "+ Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 610f)));

            //Debug.Log("左上 " + Camera.main.ScreenToWorldPoint(new Vector3(0f, 1200f, 610f)));

            //Debug.Log("右上 " + Camera.main.ScreenToWorldPoint(new Vector3(3840f, 1200f, 610f)));

            //Debug.Log("右下 " + Camera.main.ScreenToWorldPoint(new Vector3(3840f, 0f, 610f)));

            VisualEffect.SetVector3("PictureCenter", rect3D.CenterPos);

            VisualEffect.SetVector3("PictureSize", new Vector3(Size.x, Size.y, 0f));

            VisualEffect.enabled = true;

            VisualEffect.Play();

        }
    }
#endif
    // Start is called before the first frame update 
    void Start()
    {
        Init();
    }

    // Update is called once per frame  
    void Update()
    {
        
    }
}


public class  Rect3D
{
    public Vector3 CenterPos;

    public float WorldWidth;

    public float WorldHeight;
}