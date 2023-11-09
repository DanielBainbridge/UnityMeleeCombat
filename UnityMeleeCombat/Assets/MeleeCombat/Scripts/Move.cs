using System;
using System.Collections.Generic;
using UnityEditor.Animations;

using UnityEngine;
namespace MeleeCombatTool
{
    //is the move class
    [Serializable]
    public class Move
    {
        //variables
        [HideInInspector] public Animator m_animator;
        public AnimationClip m_moveAnimation;
        public List<HitBox> m_moveHitBoxes;
        public string m_moveName;
        public bool m_isBeingUsed;


        private int m_totalAnimationFrames;
        private int m_currentAnimationFrame = 0;

        //gets the current frame
        public int GetCurrentAnimationFrame()
        {
            return m_currentAnimationFrame;
        }

        //incremement move to next frame in the animation
        public void IncrementAnimationFrame()
        {
            m_currentAnimationFrame++;
        }

        //make current frame the start
        public void ResetCurrentFrame()
        {
            m_currentAnimationFrame = 0;
        }

        //get length of the frames
        public int GetTotalAnimationFrames()
        {
            return m_totalAnimationFrames;
        }

        //get the frame rate of the animation to step through
        public float GetMoveFrameRate()
        {
            return m_moveAnimation.frameRate;
        }

        //calculate the amount of frames based on the length of the emission
        public void SetTotalFrames()
        {
            m_totalAnimationFrames = (int)(m_moveAnimation.length * m_moveAnimation.frameRate);
        }

        //make a hit box object on the frame when the hit box starts
        public void ConstructHitBoxesPerFrame()
        {
            foreach (HitBox hB in m_moveHitBoxes)
            {
                if (hB.m_startFrame == m_currentAnimationFrame)
                {
                    hB.ConstructHitbox();
                }
            }
        }
        //destroy hit box object on the frame the hit box ends
        public void DestroyHitBoxesPerFrame()
        {
            foreach (HitBox hB in m_moveHitBoxes)
            {
                if (hB.m_endFrame == m_currentAnimationFrame)
                {
                    hB.DestroyHitBoxObject();
                }
            }
        }
        //play animation on the move
        public void StartMove()
        {
            m_animator.Play(m_moveName);
        }
        //plays the animation "no motion" there is no way to force the unity animator to enter the default state without the name that I found
        public void StopMove()
        {
            m_animator.Play("No Motion");
        }
        //check whether the animation is inside of the animator attached to the owner
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

        //adds the move to the animator controller 
        public void AddMoveToAnimator()
        {
            AnimatorController animatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(m_animator.runtimeAnimatorController));
            animatorController.AddMotion(m_moveAnimation, 0);
        }
    }
}

//Custom Editor For Moves
namespace MeleeCombatTool.Editors
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    [CustomEditor(typeof(Move))]
    public class MoveEditor : Editor
    {
        SerializedProperty m_animation;
        SerializedProperty m_hitBoxList;

        public void OnEnable()
        {
            m_animation = serializedObject.FindProperty("m_moveAnimation");
            m_hitBoxList = serializedObject.FindProperty("m_moveHitBoxes");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_animation);

            //Only show hitboxes in the editor if there is an animation plugged into the move
            if (m_animation.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_hitBoxList);
            }
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();

        }
    }
}