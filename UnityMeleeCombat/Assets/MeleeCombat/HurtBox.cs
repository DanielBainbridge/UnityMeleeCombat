using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [HideInInspector] public CombatItem m_owner;
    [HideInInspector] public Collider m_collider;
}
