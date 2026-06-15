using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class RigManager : MonoBehaviour
{

    public RigBuilder RigBuilder;
    public List<RigLayer> RigLayers;

    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RigLayers = RigBuilder.layers;
       RigLayers[0].active = false;
        RigLayers[1].active = false;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        if (animator.GetLayerWeight(1) == 1f)
        {
            RigLayers[1].active = true;
            RigLayers[0].active = true;



        }
        //else if (animator.GetLayerWeight(1) == 1.0f && animator.GetBool("isAiming"))
        //{

        //    RigLayers[0].active = true;
        //    RigLayers[1].active = true;


        //}

        else
        {
            RigLayers[0].active = false;
            RigLayers[1].active = false;
        
        
        }
        
    }




    private void WalkAnimationEvent() {
      //  RigLayers[0].active = true;
      //  RigLayers[1].active = true;
        
    
    
    }

   
}
