using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[RequireComponent(typeof(Animator))]
[ExecuteInEditMode]
public class CombatItem : MonoBehaviour
{
    //the combat items stuff itself
    [SerializeField] private Animator m_animator;
    [SerializeField] private List<Move> m_moves;
    [SerializeField] private List<HurtBox> m_hurtBoxes;

    
    public void GenerateHurtBoxes()
    {
        Debug.Log("Generate Hurt Boxes Button Pressed");
        //Do stuff on button press dummy
    }

    public void AddHurtBox()
    {
        Debug.Log("Add Hurt Box Button Pressed");
        //Do stuff on button press dummy
    }
    public void AddMove()
    {
        m_moves.Add(new Move());
    }

    //dependent structs
    [Serializable]
    public struct HitBoxConstructor
    {
        enum Shape
        {
            Box,
            Sphere,
            Capsule
        }
        [SerializeField] Transform m_parentTransform;
        [SerializeField] Shape m_shape;
        [SerializeField] int m_startFrame;
        [SerializeField] int m_endFrame;
        [SerializeField] bool m_automaticDamageCalculation;
        [SerializeField] float m_damage;
        [SerializeField] float m_knockbackDistance;
        [SerializeField] bool m_automaticKnockbackAngle;
        [SerializeField] Vector3 m_knockbackAngle;
        void ConstructHitbox()
        {
            //Instantiate(GameObject crap)
        }
    }
    [Serializable]
    public struct Move
    {
        public Animation m_moveAnimation;
        private int m_totalAnimationFrames;
        private int m_currentAnimationFrame;
        [SerializeField]
        private List<HitBoxConstructor> m_moveHitboxes;

        void UseMove()
        {
            //Play Animation
            //For Each Frame of animation step check if there is a hitbox starting or ending on that frame,
            //If there is call for the creation/destruction of the hitbox
            //At the end of the move set current frame to first frame
        }
    }

}
