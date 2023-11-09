using System;
using System.Collections.Generic;
using UnityEngine;
namespace MeleeCombatTool
{
    [ExecuteInEditMode]
    public class HurtBoxObject : MonoBehaviour
    {
        [HideInInspector] public HurtBox m_hurtbox;

        //Update the debug mesh of the hurt box
        private void Update()
        {
            if (m_hurtbox.m_owner.m_debugHurtBoxes)
            {
                m_hurtbox.UpdateDebugMeshSize();
            }
        }
        //Destroy the hurtbox object
        public void DestroyHurtBox()
        {
            DestroyImmediate(this.gameObject);
        }
    }
}