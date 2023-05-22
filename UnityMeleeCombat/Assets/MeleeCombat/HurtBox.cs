using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[Serializable]

public class HurtBox
{
    [SerializeField] public CombatItem m_owner;
    [SerializeField] public Collider m_collider;
    private HurtBoxObject m_hurtBoxObject;
    enum Shape
    {
        Box,
        Sphere,
        Capsule
    }
    Shape m_shape;

    //box
    [SerializeField] private float m_width, m_height, m_depth;

    //sphere and capsule
    [SerializeField] private float m_radius, m_length;

    void ConstructHurtbox(Transform parentTransform)
    {
        GameObject newHurtBoxObject = new GameObject();
        newHurtBoxObject.AddComponent<HurtBoxObject>();
        switch(m_shape)
        {
            case Shape.Box:
                BoxCollider boxColldier = newHurtBoxObject.AddComponent<BoxCollider>();
                
                break;
            case Shape.Sphere:
                SphereCollider sphereColldier = newHurtBoxObject.AddComponent<SphereCollider>();
                break;
            case Shape.Capsule:
                CapsuleCollider capsuleColldier= newHurtBoxObject.AddComponent <CapsuleCollider>();
                break;
        }

    }

    void DestroyHurtBox()
    {
        UnityEngine.Object.DestroyImmediate(m_hurtBoxObject);
    }
}
