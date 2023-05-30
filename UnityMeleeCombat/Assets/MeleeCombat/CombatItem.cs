using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[ExecuteInEditMode]
public class CombatItem : MonoBehaviour
{
    //the combat items stuff itself
    [SerializeField] private Animator m_animator;
    [SerializeField] public List<Move> m_moves;
    [SerializeField] public List<HurtBox> m_hurtBoxes;
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
        HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Box, transform);
        newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Box");

        if (GetComponentInChildren<MeshRenderer>())
        {
            newHurtBox.SetBoxHurtBoxObject(GetComponentInChildren<MeshRenderer>().bounds.extents, GetComponentInChildren<MeshRenderer>().bounds.center);
        }
        else if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            newHurtBox.SetBoxHurtBoxObject(GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents, GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
        }

        m_hurtBoxes.Add(newHurtBox);
    }
    public void AddHurtBoxSphere()
    {
        HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Sphere, transform);
        newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Sphere");

        if (GetComponentInChildren<MeshRenderer>())
        {
            newHurtBox.SetSphereHurtBoxObject(GetComponentInChildren<MeshRenderer>().bounds.extents.y, GetComponentInChildren<MeshRenderer>().bounds.center);
        }
        else if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            newHurtBox.SetSphereHurtBoxObject(GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.y, GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
        }

        m_hurtBoxes.Add(newHurtBox);
    }
    public void AddHurtBoxCapsule()
    {
        HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Capsule, transform);
        newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Capsule");

        if (GetComponentInChildren<MeshRenderer>())
        {
            newHurtBox.SetCapsuleHurtBoxObject(
            GetComponentInChildren<MeshRenderer>().bounds.extents.x,
            GetComponentInChildren<MeshRenderer>().bounds.extents.y,
            GetComponentInChildren<MeshRenderer>().bounds.center);
        }
        else if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            newHurtBox.SetCapsuleHurtBoxObject(
            GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.x,
            GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.y,
            GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
        }

        m_hurtBoxes.Add(newHurtBox);
    }

    public void RemoveHurtBox()
    {
        if (m_hurtBoxes.Count != 0)
        {
            m_hurtBoxes[m_hurtBoxes.Count - 1].DestroyHurtBox();
            m_hurtBoxes.RemoveAt(m_hurtBoxes.Count - 1);
        }
    }
    public void ClearHurtBoxes()
    {
        foreach (HurtBox hB in m_hurtBoxes)
        {           
            hB.DestroyHurtBox();
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
    public void UpdateMove()
    {
        foreach(Move m in m_moves)
        {
            if(m.m_moveAnimation != null)
            {
                m.m_moveName = m.m_moveAnimation.name;
                m.SetTotalFrames();
                if (!m.IsMoveInAnimator())
                {
                    m.AddMoveToAnimator();
                }
            }
        }
    }
}