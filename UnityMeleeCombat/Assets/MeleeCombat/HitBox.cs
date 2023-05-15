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

    [SerializeField] Transform m_parentTransform;
    [SerializeField] Shape m_shape;
    [SerializeField] int m_startFrame;
    [SerializeField] int m_endFrame;
    [SerializeField] bool m_automaticDamageCalculation;
    [SerializeField] float m_damage;
    [SerializeField] float m_knockbackDistance;
    [SerializeField] bool m_automaticKnockbackAngle;
    [SerializeField] Vector3 m_knockbackAngle;

    //box
    private float m_width, m_height, m_depth;

    //sphere and capsule
    private float m_radius, m_length;



    void ConstructHitbox()
    {
        //Instantiate(GameObject crap)
    }


    private void FixedUpdate()
    {
        
    }
}
