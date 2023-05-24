using System;
using System.Collections.Generic;
using UnityEngine;

class HurtBoxObject : MonoBehaviour
{
    [HideInInspector] public HurtBox m_hurtbox;

    public void StupidDestroyHurtBox()
    {
        DestroyImmediate(this.gameObject);
    }
}