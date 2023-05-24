using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]

public class HurtBox
{
    public HurtBox(Shape shape, Transform parent)
    {
        m_shape = shape;
        Assert.IsTrue(parent.GetComponent<CombatItem>() != null, "HurtBox Not Parented to a combat item");
        m_owner = parent.GetComponent<CombatItem>();
        ConstructHurtbox();
    }
    [SerializeField] public CombatItem m_owner;
    [SerializeField] public Collider m_collider;
    [SerializeField] public Vector3 m_center;
    private HurtBoxObject m_hurtBoxObject = null;
    public enum Shape
    {
        Box,
        Sphere,
        Capsule
    }
    [SerializeField] Shape m_shape;

    //box
    [SerializeField] private float m_width, m_height, m_depth;

    //sphere and capsule
    [SerializeField] private float m_radius;

    HurtBox ConstructHurtbox()
    {
        GameObject newHurtBoxObject = new GameObject();
        newHurtBoxObject.AddComponent<HurtBoxObject>();
        newHurtBoxObject.GetComponent<HurtBoxObject>().m_hurtbox = this;
        newHurtBoxObject.transform.parent = m_owner.transform;
        switch (m_shape)
        {
            case Shape.Box:
                BoxCollider boxCollider = newHurtBoxObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                break;
            case Shape.Sphere:
                SphereCollider sphereCollider = newHurtBoxObject.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                break;
            case Shape.Capsule:
                CapsuleCollider capsuleCollider = newHurtBoxObject.AddComponent<CapsuleCollider>();
                capsuleCollider.isTrigger = true;
                break;
        }

        m_hurtBoxObject = newHurtBoxObject.GetComponent<HurtBoxObject>();
        return this;

    }

    public void DestroyHurtBox()
    {
        m_hurtBoxObject.StupidDestroyHurtBox();    
    }

    public void UpdateHurtBoxObject()
    {

        switch (m_shape)
        {
            case Shape.Box:
                BoxCollider boxCollider = m_hurtBoxObject.GetComponent<BoxCollider>();
                boxCollider.size = new Vector3(m_width, m_height, m_depth);
                boxCollider.center = m_center;
                break;
            case Shape.Sphere:
                SphereCollider sphereCollider = m_hurtBoxObject.GetComponent<SphereCollider>();
                sphereCollider.radius = m_radius;
                sphereCollider.center = m_center;
                break;
            case Shape.Capsule:
                CapsuleCollider capsuleCollider = m_hurtBoxObject.GetComponent<CapsuleCollider>();
                capsuleCollider.radius = m_radius;
                capsuleCollider.height = m_height;
                capsuleCollider.center = m_center;
                break;
        }
    }
    public void SetBoxHurtBoxObject(Vector3 extents, Vector3 center)
    {
        extents *= 2.0f;
        m_hurtBoxObject.GetComponent<BoxCollider>().size = extents;
        m_width = extents.x; m_height = extents.y; m_depth = extents.z;
        m_hurtBoxObject.GetComponent<BoxCollider>().center = m_center = center;
    }
    public void SetSphereHurtBoxObject(float radius, Vector3 center)
    {
        m_hurtBoxObject.GetComponent<SphereCollider>().radius = m_radius = radius;
        m_hurtBoxObject.GetComponent<SphereCollider>().center = m_center = center;
    }
    public void SetCapsuleHurtBoxObject(float radius, float height, Vector3 center)
    {
        m_hurtBoxObject.GetComponent<CapsuleCollider>().radius = m_radius = radius;
        m_hurtBoxObject.GetComponent<CapsuleCollider>().height = m_height = height;
        m_hurtBoxObject.GetComponent<CapsuleCollider>().center = m_center = center;
    }
    public void NameHurtBoxObject(string name)
    {
        m_hurtBoxObject.gameObject.name = name;
    }
}
