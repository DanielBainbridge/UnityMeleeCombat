using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class Move 
{
    [HideInInspector] public Animator m_animator;
    public AnimationClip m_moveAnimation;
    public List<HitBox> m_moveHitBoxes;


    private int m_totalAnimationFrames;
    private int m_currentAnimationFrame;



    void UseMove()
    {

        //Play Animation
        //For Each Frame of animation step check if there is a hitbox starting or ending on that frame,
        //If there is call for the creation/destruction of the hitbox
        //At the end of the move set current frame to first frame
    }
}