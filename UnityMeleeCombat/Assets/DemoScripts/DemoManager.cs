using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public Transform m_defenceObject;
    CombatItem m_defenceCombatItem;
    Vector3 m_defenceObjectOriginalLocation;
    [SerializeField] float m_defenceObjectOffset = 0;
    public Transform m_offenceObject;
    CombatItem m_offenceCombatItem;
    bool m_moveUsed = false;

    public Transform m_knockbackAngleItem;

    //[SerializeField] Slider m_knockbackStrength;
    //[SerializeField] Slider m_knockbackAngle;
    //[SerializeField] Toggle m_autoKnockBack;
    //[SerializeField] Toggle m_debug;

    private void Start()
    {
        Application.targetFrameRate = 60;
        m_offenceCombatItem = m_offenceObject.GetComponent<CombatItem>();
        m_defenceCombatItem = m_defenceObject.GetComponent<CombatItem>();
        m_defenceObjectOriginalLocation = m_defenceObject.position;
    }
    private void Update()
    {
        if (!m_moveUsed)
        {
            m_defenceObject.position = m_defenceObjectOriginalLocation + new Vector3(0, 0, m_defenceObjectOffset);
            m_defenceObject.rotation = Quaternion.Euler(new Vector3(0,180,0));
            m_defenceObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_defenceObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    public void ResetDefenceObject()
    {
        m_defenceObject.position = m_defenceObjectOriginalLocation + new Vector3(0, 0, m_defenceObjectOffset);
        if (m_defenceObject.GetComponent<Rigidbody>())
        {
            m_defenceObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_defenceObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        m_moveUsed = false;
    }

    public void DoMoveOne()
    {
        m_offenceCombatItem.UseMove(0);
        m_moveUsed = true;
    }
    public void DoMoveTwo()
    {
        m_offenceCombatItem.UseMove(1);
        m_moveUsed = true;
    }    

    public void UpdateDefenceDistance(float distance)
    {
        m_defenceObjectOffset = distance;
    }

    public void UpdateKnockBack(float knockBackDistance)
    {
        foreach (Move m in m_offenceCombatItem.m_moves)
        {
            foreach (HitBox hB in m.m_moveHitBoxes)
            {
                hB.m_knockbackDistance = knockBackDistance;
            }
        }
    }

    public void UpdateKnockBackAngle(float knockbackAngle)
    {
        foreach (Move m in m_offenceCombatItem.m_moves)
        {
            foreach (HitBox hB in m.m_moveHitBoxes)
            {
                hB.m_knockbackAngle.y = knockbackAngle;
            }
        }
    }
    public void ToggleAutoKnockBack()
    {
        foreach (Move m in m_offenceCombatItem.m_moves)
        {
            foreach (HitBox hB in m.m_moveHitBoxes)
            {
                hB.m_automaticKnockbackAngle = !hB.m_automaticKnockbackAngle;
            }
        }
        if(m_offenceCombatItem.m_moves[0].m_moveHitBoxes[0].m_automaticKnockbackAngle)
            m_knockbackAngleItem.gameObject.SetActive(true);
        else
            m_knockbackAngleItem.gameObject.SetActive(false);
    }

    public void UpdateHitStopLength(float stopLength)
    {
        foreach (Move m in m_offenceCombatItem.m_moves)
        {
            foreach (HitBox hB in m.m_moveHitBoxes)
            {
                hB.m_hitStopLength = stopLength;
            }
        }
    }

    public void ToggleDebug()
    {
        m_offenceCombatItem.ToggleDebug();
        m_defenceCombatItem.ToggleDebug();
        m_offenceCombatItem.DebugBoxCheck();
        m_defenceCombatItem.DebugBoxCheck();
    }
}
