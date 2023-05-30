using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class Move
{
    [HideInInspector] public Animator m_animator;
    public AnimationClip m_moveAnimation;
    public List<HitBox> m_moveHitBoxes;
    public string m_moveName;


    private int m_totalAnimationFrames;
    private int m_currentAnimationFrame = 0;

    public void SetTotalFrames()
    {
        m_totalAnimationFrames = (int)(m_moveAnimation.length * m_moveAnimation.frameRate);
    }

    public void UseMove()
    {
        //make this a coroutine so that we can keep track of frames easier, make hitbox life time a coroutine?
        //give yourself the
        m_animator.Play(m_moveName);


        //Play Animation
        //For Each Frame of animation step check if there is a hitbox starting or ending on that frame,
        //If there is call for the creation/destruction of the hitbox
        //At the end of the move set current frame to first frame
    }

    private void ConstructHitBoxesPerFrame()
    {
        foreach (HitBox hB in m_moveHitBoxes)
        {
            if (hB.m_startFrame == m_currentAnimationFrame)
            {
                hB.ConstructHitbox();
            }
        }
    }
    private void DestroyHitBoxesPerFrame()
    {
        foreach (HitBox hB in m_moveHitBoxes)
        {
            if (hB.m_endFrame == m_currentAnimationFrame)
            {
                hB.DestroyHitBoxObject();
            }
        }
    }


    private IEnumerable PlayMoveFrame()
    {
        if (m_currentAnimationFrame == 0)
        {
            m_animator.Play(m_moveName);
        }

        else if (m_currentAnimationFrame != m_totalAnimationFrames)
        {
            m_currentAnimationFrame++;

            //construction and destruction functions are dependent on current animation frame
            ConstructHitBoxesPerFrame();
            DestroyHitBoxesPerFrame();

            yield return new WaitForNextFrameUnit();
            PlayMoveFrame();
        }

        else
        {
            m_currentAnimationFrame = 0;
            yield return null;
        }

    }

    public bool IsMoveInAnimator()
    {
        List<AnimatorState> animStates = new List<AnimatorState>();
        AnimatorController animatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(m_animator.runtimeAnimatorController));
        foreach (AnimatorControllerLayer aCL in animatorController.layers)
        {
            ChildAnimatorState[] childAnimStates = aCL.stateMachine.states;
            foreach (ChildAnimatorState cAS in childAnimStates)
            {
                animStates.Add(cAS.state);
            }
        }

        foreach (AnimatorState aS in animStates)
        {
            if (aS.name == m_moveName)
            {
                return true;
            }
        }
        return false;
    }

    public void AddMoveToAnimator()
    {
        AnimatorController animatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(m_animator.runtimeAnimatorController));
        animatorController.AddMotion(m_moveAnimation, 0);
    }
}