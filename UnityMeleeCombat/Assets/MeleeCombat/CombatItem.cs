using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[ExecuteInEditMode]
public class CombatItem : MonoBehaviour
{
    //the combat items stuff itself
    [SerializeField] private Animator m_animator;
    [SerializeField] private List<Move> m_moves;
    [SerializeField] private List<HurtBox> m_hurtBoxes;
    [SerializeField] private bool m_debugHurtBoxes;
    [SerializeField] private bool m_debugHitBoxes;

    private void OnEnable()
    {
        m_animator = GetComponent<Animator>();
    }

    public void GenerateHurtBoxes()
    {
        Debug.Log("Generate Hurt Boxes Button Pressed");
        //Do stuff on button press dummy
    }

    public void AddHurtBoxBox()
    {
        GameObject newHurtBox = new GameObject();
        newHurtBox.AddComponent<HurtBox>();
        newHurtBox.GetComponent<HurtBox>().m_owner = this;
        newHurtBox.transform.parent = transform;
        newHurtBox.name = $"Hurt Box {m_hurtBoxes.Count + 1}";

        BoxCollider box = newHurtBox.AddComponent<BoxCollider>();

        if (GetComponentInChildren<MeshRenderer>())
        {
            box.center = GetComponentInChildren<MeshRenderer>().bounds.center;
            box.size = GetComponentInChildren<MeshRenderer>().bounds.extents * 2.0f;
        }
        else if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            box.center = GetComponentInChildren<SkinnedMeshRenderer>().bounds.center;
            box.size = GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents * 2.0f;
        }

        newHurtBox.GetComponent<HurtBox>().m_collider = box;

        m_hurtBoxes.Add(newHurtBox.GetComponent<HurtBox>());
    }
    public void AddHurtBoxSphere()
    {
        HurtBox newHurtBox = new HurtBox();
        newHurtBox.m_owner = this;
        newHurtBox.transform.parent = transform;
        newHurtBox.name = $"Hurt Box {m_hurtBoxes.Count + 1}";

        SphereCollider sphere = newHurtBox.AddComponent<SphereCollider>();

        if (GetComponentInChildren<MeshRenderer>())
        {
            sphere.center = GetComponentInChildren<MeshRenderer>().bounds.center;
            sphere.radius = GetComponentInChildren<MeshRenderer>().bounds.extents.y;
        }
        else if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            sphere.radius = GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.y;
            sphere.center = GetComponentInChildren<SkinnedMeshRenderer>().bounds.center;
        }

        newHurtBox.GetComponent<HurtBox>().m_collider = sphere;

        m_hurtBoxes.Add(newHurtBox.GetComponent<HurtBox>());
    }
    public void AddHurtBoxCapsule()
    {
        GameObject newHurtBox = new GameObject();
        newHurtBox.AddComponent<HurtBox>();
        newHurtBox.GetComponent<HurtBox>().m_owner = this;
        newHurtBox.transform.parent = transform;
        newHurtBox.name = $"Hurt Box {m_hurtBoxes.Count + 1}";

        CapsuleCollider capsule = newHurtBox.AddComponent<CapsuleCollider>();

        if (GetComponentInChildren<MeshRenderer>())
        {
            capsule.center = GetComponentInChildren<MeshRenderer>().bounds.center;
            capsule.height = GetComponentInChildren<MeshRenderer>().bounds.extents.y;
            capsule.radius = GetComponentInChildren<MeshRenderer>().bounds.extents.x;
        }
        else if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            capsule.center = GetComponentInChildren<SkinnedMeshRenderer>().bounds.center;
            capsule.height = GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.y;
            capsule.radius = GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.x;
        }

        newHurtBox.GetComponent<HurtBox>().m_collider = capsule;

        m_hurtBoxes.Add(newHurtBox.GetComponent<HurtBox>());
    }

    public void RemoveHurtBox()
    {
        if (m_hurtBoxes.Count != 0)
        {
            DestroyImmediate(m_hurtBoxes[m_hurtBoxes.Count - 1].gameObject);
            m_hurtBoxes.RemoveAt(m_hurtBoxes.Count - 1);
        }
    }
    public void ClearHurtBoxes()
    {
        foreach (HurtBox hB in m_hurtBoxes)
        {
            DestroyImmediate(hB.gameObject);
        }
        m_hurtBoxes.Clear();
    }

    public void AddMove()
    {
        Move newMove = new Move();
        newMove.m_animator = m_animator;
        m_moves.Add(newMove);
    }
    public void RemoveMove()
    {
        if (m_moves.Count != 0)
            m_moves.RemoveAt(m_moves.Count - 1);
    }
    public void AddHitBoxToMove(int moveNumber)
    {
        m_moves[moveNumber].m_moveHitBoxes.Add(new HitBox());
    }
    public void RemoveHitBoxFromMove(int moveNumber)
    {
        if (m_moves.Count != 0 && m_moves[moveNumber].m_moveHitBoxes.Count != 0)
            m_moves[moveNumber].m_moveHitBoxes.RemoveAt(m_moves[moveNumber].m_moveHitBoxes.Count - 1);
    }
}