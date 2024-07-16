using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements.Experimental;

public class Hexagon : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new Collider collider;
    public Score score;

     public HexStack HexStack {get; private set;}
    public Color Color
    {
        get =>  renderer.material.color;
        set =>  renderer.material.color = value ;
    }

    public void Configure(HexStack hexStack)
    {
        HexStack = hexStack;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        
    }

    public void DisableCollider() => collider.enabled = false;

    public void Vanish(float delay)
    {
        LeanTween.cancel(gameObject);

        LeanTween.scale(gameObject, Vector3.zero, .2f)
        .setEase(LeanTweenType.easeInBack)
        .setDelay(delay)
        .setOnComplete(() => Destroy(gameObject));      
    }
    
    public void MoveToLocal(Vector3 targetLocalPos) 
    {
        LeanTween.cancel(gameObject);

        float delay = transform.GetSiblingIndex() * .01f;

        LeanTween.moveLocal(gameObject, targetLocalPos , .2f)
        .setEase(LeanTweenType.easeInOutSine)
        .setDelay(delay);

        Vector3 direction = (targetLocalPos - transform.localPosition).Withs(y : 0).normalized;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        LeanTween.rotateAround(gameObject , rotationAxis, 180, .2f)
        .setEase(LeanTweenType.easeInOutSine)
        .setDelay(delay);
    }
}
