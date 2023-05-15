using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[RequireComponent(typeof(Animator))]
[ExecuteInEditMode]
public class CombatItem : MonoBehaviour
{
    //the combat items stuff itself
    [SerializeField] private Animator m_animator;
    [SerializeField] private List<Move> m_moves;
    [SerializeField] private List<HurtBox> m_hurtBoxes;

    
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
        m_moves.RemoveAt(m_moves.Count - 1);
    }
}