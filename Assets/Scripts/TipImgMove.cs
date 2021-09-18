using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipImgMove : MonoBehaviour
{

    /// <summary>
    /// 是左移还是右移
    /// </summary>
    public bool IsMoveLeft = true;

    public Transform Target;

    /// <summary>
    /// 与目标的距离
    /// </summary>
    public float Distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            this.transform.position = new Vector3(Target.position.x+ Distance, Target.position.y, Target.position.z);
        }
    }
}
