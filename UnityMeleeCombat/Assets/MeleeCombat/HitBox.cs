using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] public int m_startFrame = 0;
    [SerializeField] public int m_endFrame = 1;
    [SerializeField][Tooltip("Calculate Damage Automatically based on animation length and hit box alive time, NOT RECOMMENDED BUT A FEATURE")] bool m_automaticDamageCalculation;
    [SerializeField] float m_damage;
    [SerializeField] float m_knockbackDistance;
    [SerializeField][Tooltip("Calculate Knockback Automatically based on angle that hitbox hits a hurtbox")] bool m_automaticKnockbackAngle;
    [SerializeField] Vector3 m_knockbackAngle;
    [SerializeField][Tooltip("Calculate Hit Stop scaling with damage dealt, more damage more hit stop")] bool m_automaticHitStop;
    [SerializeField] float m_hitStopLength;
    [SerializeField] float m_hitStopMultiplier;
    private HitBoxObject m_hitBoxObject;


    //box
    [SerializeField] private float m_width, m_height, m_depth;

    //sphere and capsule
    [SerializeField] private float m_radius;



    public HitBoxObject ConstructHitbox()
    {
        //Instantiate(GameObject crap)
        GameObject newHitBox = new GameObject();
        newHitBox.AddComponent<HitBoxObject>();
        newHitBox.GetComponent<HitBoxObject>().m_hitbox = this;
        newHitBox.transform.parent = m_owner.transform;
        switch (m_shape)
        {
            case Shape.Box:
                BoxCollider boxCollider = newHitBox.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                //set sizes of stuff
                break;
            case Shape.Sphere:
                SphereCollider sphereCollider = newHitBox.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                //set sizes of stuff
                break;
            case Shape.Capsule:
                CapsuleCollider capsuleCollider = newHitBox.AddComponent<CapsuleCollider>();
                capsuleCollider.isTrigger = true;
                //set sizes of stuff
                break;
        }
        m_hitBoxObject = newHitBox.GetComponent<HitBoxObject>();
        return m_hitBoxObject;
    }

    public void DestroyHitBoxObject()
    {
        GameObject.Destroy(m_hitBoxObject.gameObject);
    }
}
