using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class HurtBoxObject : MonoBehaviour
{
    [HideInInspector] public HurtBox m_hurtbox;

    public void StupidDestroyHurtBox()
    {
        DestroyImmediate(this.gameObject);
    }


}