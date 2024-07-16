using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExt 
{

    public static void Clears(this Transform transform)
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.transform.GetChild(0);
            child.SetParent(null);
            Object.DestroyImmediate(child.gameObject);
        }
    }
   
}
