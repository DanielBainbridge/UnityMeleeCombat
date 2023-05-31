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
    [SerializeField] Vector3 m_rotationOffset;
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
        m_owner = m_parentTransform.GetComponent<CombatItem>();
        if (m_owner == null)
        {
            m_owner = m_parentTransform.GetComponentInParent<CombatItem>();
        }
        newHitBox.transform.parent = m_parentTransform;
        newHitBox.transform.localPosition = m_offset;
        newHitBox.transform.localRotation = Quaternion.Euler(m_rotationOffset);
        newHitBox.name = $"Hit Box: {m_shape}";


        newHitBox.AddComponent<MeshRenderer>();
        MeshRenderer newHitBoxMeshRenderer = newHitBox.GetComponent<MeshRenderer>();
        Material hitBoxMaterial = Resources.Load("Materials/HitBoxMaterial") as Material;
        newHitBoxMeshRenderer.material = hitBoxMaterial;
        newHitBox.AddComponent<MeshFilter>();
        MeshFilter meshFilter = newHitBox.GetComponent<MeshFilter>();

        switch (m_shape)
        {
            case Shape.Box:
                BoxCollider boxCollider = newHitBox.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                //set sizes of stuff
                boxCollider.size = new Vector3(m_width, m_height, m_depth);

                GameObject tempBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ScaleHitBoxMesh(tempBox.GetComponent<MeshFilter>().mesh, new Vector3(m_width, m_height, m_depth));
                meshFilter.mesh = tempBox.GetComponent<MeshFilter>().mesh;
                GameObject.Destroy(tempBox);
                break;

            case Shape.Sphere:
                SphereCollider sphereCollider = newHitBox.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                //set sizes of stuff
                sphereCollider.radius = m_radius;

                GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ScaleHitBoxMesh(tempSphere.GetComponent<MeshFilter>().mesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));
                meshFilter.mesh = tempSphere.GetComponent<MeshFilter>().mesh;
                GameObject.Destroy(tempSphere);
                break;

            case Shape.Capsule:
                CapsuleCollider capsuleCollider = newHitBox.AddComponent<CapsuleCollider>();
                capsuleCollider.isTrigger = true;
                //set sizes of stuff
                capsuleCollider.height = m_height;
                capsuleCollider.radius = m_radius;

                GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ScaleHitBoxMesh(tempCylinder.GetComponent<MeshFilter>().mesh, new Vector3(m_radius * 2, m_height / 5.0f, m_radius * 2));
                meshFilter.mesh = tempCylinder.GetComponent<MeshFilter>().mesh;
                GameObject.Destroy(tempCylinder);

                GameObject capsuleCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                capsuleCap.GetComponent<MeshRenderer>().material = hitBoxMaterial;
                ScaleHitBoxMesh(capsuleCap.GetComponent<MeshFilter>().mesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));
                capsuleCap.transform.parent = capsuleCollider.transform;
                capsuleCap.name = "Capsule Cap";
                capsuleCap.transform.localPosition = new Vector3(0, (-m_height / 2.0f)  + m_radius, 0);
                GameObject.Destroy(capsuleCap.GetComponent<Collider>());

                GameObject capsuleCap2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                capsuleCap2.GetComponent<MeshRenderer>().material = hitBoxMaterial;
                ScaleHitBoxMesh(capsuleCap2.GetComponent<MeshFilter>().mesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));
                capsuleCap2.transform.parent = capsuleCollider.transform;
                capsuleCap2.name = "Capsule Cap";
                capsuleCap2.transform.localPosition = new Vector3(0, (m_height / 2.0f) - m_radius, 0);
                GameObject.Destroy(capsuleCap2.GetComponent<Collider>());

                break;
        }
        m_hitBoxObject = newHitBox.GetComponent<HitBoxObject>();





        if (!m_owner.m_debugHitBoxes)
        {
            newHitBoxMeshRenderer.enabled = false;
        }
        return m_hitBoxObject;
    }

    public void DestroyHitBoxObject()
    {
        GameObject.Destroy(m_hitBoxObject.gameObject);
    }
    private void ScaleHitBoxMesh(Mesh mesh, Vector3 scale)
    {
        Vector3[] baseVertices = mesh.vertices;
        Vector3[] vertices = new Vector3[mesh.vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            vertex.x *= scale.x;
            vertex.y *= scale.y;
            vertex.z *= scale.z;

            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
    }
}
