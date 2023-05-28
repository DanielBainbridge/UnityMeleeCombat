using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[Serializable]
public class Move 
{
    [HideInInspector] public Animator m_animator;
    public AnimationClip m_moveAnimation;
    public List<HitBox> m_moveHitBoxes;
    public string m_moveName;


    private int m_totalAnimationFrames;
    private int m_currentAnimationFrame;

    public void SetTotalFrames()
    {
        m_totalAnimationFrames = (int)(m_moveAnimation.length * m_moveAnimation.frameRate);
    }

    public void UseMove()
    {
        //use playables they have what we want, we want to play the animation without the user going into the animation editor and without me needing to either
        //we use playables because the animation contorller is always turned on under the hood and we don't want that
        //we have to disable the animation controller while we do our manual playables

        //stuff to google, how to play a unity playable in code

        //make this a coroutine so that we can keep track of frames easier, make hitbox life time a coroutine?
        //give yourself the

        //Play Animation
        //For Each Frame of animation step check if there is a hitbox starting or ending on that frame,
        //If there is call for the creation/destruction of the hitbox
        //At the end of the move set current frame to first frame
    }
}