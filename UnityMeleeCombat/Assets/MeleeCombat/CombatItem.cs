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

    private void OnEnable()
    {
        m_animator = GetComponent<Animator>();
    }

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