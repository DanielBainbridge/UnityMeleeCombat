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

        BoxCollider box = new BoxCollider();

        if (GetComponentInChildren<MeshRenderer>())
        {
            box.center = GetComponentInChildren<MeshRenderer>().bounds.center;
            box.size = GetComponentInChildren<MeshRenderer>().bounds.extents;
        }
        else if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            box.center = GetComponentInChildren<MeshRenderer>().bounds.center;
            box.size = GetComponentInChildren<MeshRenderer>().bounds.extents;
        }

        newHurtBox.GetComponent<HurtBox>().m_collider = box;

        m_hurtBoxes.Add(newHurtBox.GetComponent<HurtBox>());
    }

    public void RemoveHurtBox()
    {
        if (m_hurtBoxes.Count != 0)
        {
            DestroyImmediate(m_hurtBoxes[m_hurtBoxes.Count - 1]);
            m_hurtBoxes.RemoveAt(m_hurtBoxes.Count - 1);
        }
    }
    public void ClearHurtBoxes()
    {
        foreach (HurtBox hB in m_hurtBoxes)
        {
            DestroyImmediate(hB);
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