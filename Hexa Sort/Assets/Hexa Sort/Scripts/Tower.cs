using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class Tower : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Animator animator;
    private Renderer renderer;

    [Header(" Settings ")]
    private float fillPercent;
    [SerializeField] private float fillIncrement;
    [SerializeField] private float maxFillIncrement;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
    
    void Start()
    {
        UpdateMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
           
            Fill();
        }
        
    }

    private void Fill()
    {
         if(fillPercent >= 1)
            {
                return;
            }
        fillPercent += fillIncrement;
        UpdateMaterials();
        animator.Play("Bump");
    }

    private void UpdateMaterials()
    {
        foreach(Material material in renderer.materials)
        {
            material.SetFloat("_Fill_Percent", fillPercent * maxFillIncrement);
        }
    }
}
