using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HitBox
{

    // when the move animation is changed,
    // change slider values for start and end frames 

    enum Shape
    {
        Box,
        Sphere,
        Capsule
    }

    [HideInInspector] public CombatItem m_owner;
    [SerializeField] Transform m_parentTransform;
    [SerializeField] Vector3 m_offset;
    [SerializeField] Shape m_shape;
    [SerializeField] int m_startFrame = 0;
    [SerializeField] int m_endFrame = 1;
    [SerializeField] [Tooltip("Calculate Damage Automatically based on animation length and hit box alive time, NOT RECOMMENDED BUT A FEATURE")] bool m_automaticDamageCalculation;
    [SerializeField] float m_damage;
    [SerializeField] float m_knockbackDistance;
    [SerializeField] [Tooltip("Calculate Knockback Automatically based on angle that hitbox hits a hurtbox")] bool m_automaticKnockbackAngle;
    [SerializeField] Vector3 m_knockbackAngle;

    //box
    [SerializeField] private float m_width, m_height, m_depth;

    //sphere and capsule
    [SerializeField] private float m_radius, m_length;



    void ConstructHitbox()
    {
        //Instantiate(GameObject crap)
    }


    private void FixedUpdate()
    {
        
    }
}
