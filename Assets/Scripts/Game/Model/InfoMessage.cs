using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMessage : MonoBehaviour
{
    public delegate void DisappearedEventHandler(object sender, EventArgs e);
    public event DisappearedEventHandler Disappeared;

    [SerializeField] private Animator _animator;

    public void Disappear()
    {
        _animator.Play("InfoDisappearAnimation");
    }

    public void Anim_Disappear()
    {
        Disappeared?.Invoke(this, EventArgs.Empty);
    }
}
