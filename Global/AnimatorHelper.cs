using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatorHelper
{
    public AnimatorHelper(Animator ani)
    {
        Debug.Assert(ani != null, "UnKnown Animator Can't use this Class");
        Animator = ani;
    }

    /// <summary> working main Animator </summary>
    public Animator Animator { get; private set; }

    /// <summary> support Animators </summary>
    LinkedList<Animator> AnimatorList = new();

    #region <-- Supporter Builder -->
    public AnimatorHelper AddSupporterLast(Animator ani, out LinkedListNode<Animator> node)
    {
        node = AnimatorList.AddLast(ani);
        return this;
    }
    public AnimatorHelper AddSupporterFirst(Animator ani, out LinkedListNode<Animator> node)
    {
        node = AnimatorList.AddFirst(ani);
        return this;
    }
    public AnimatorHelper AddSupporterAfter(Animator ani, LinkedListNode<Animator> linked, out LinkedListNode<Animator> node)
    {
        node = AnimatorList.AddAfter(linked, ani);
        return this;
    }
    public AnimatorHelper AddSupporterBefore(Animator ani, LinkedListNode<Animator> linked, out LinkedListNode<Animator> node)
    {
        node = AnimatorList.AddBefore(linked, ani);
        return this;
    }
    public AnimatorHelper RemoveSupporter(Animator ani, out bool success)
    {
        success = AnimatorList.Remove(ani);
        return this;
    }
    public AnimatorHelper RemoveLastSupporter()
    {
        AnimatorList.RemoveLast();
        return this;
    }
    public AnimatorHelper RemoveFirstSupporter()
    {
        AnimatorList.RemoveFirst();
        return this;
    }
    public void ClearSupporter() => AnimatorList.Clear();
    public LinkedListNode<Animator> FindSupporter(Animator ani) => AnimatorList.Find(ani);
    public LinkedListNode<Animator> FindLastSupporter(Animator ani) => AnimatorList.FindLast(ani);
    public LinkedListNode<Animator> FirstSupporter => AnimatorList.First;
    public LinkedListNode<Animator> LastSupporter => AnimatorList.Last;
    public int SupporterCount => AnimatorList.Count;
    #endregion <-- Supporter Builder -->

    /// <summary> Current animate match the name of the active state in the statemachine? </summary>
    public bool CurrentIsName(string name, int layerIndex = 0) => Animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(name);
    /// <summary> Sets the value of the given boolean parameter. </summary>
    /// <param name="ani"></param>
    /// <param name="settors">The parameter info</param>
    public static void SetBool(Animator ani, params (string name, bool value)[] settors)
    {
        foreach (var (name, value) in settors) ani.SetBool(name, value);
    }
    /// <summary> Sets the value of the given boolean parameter. </summary>
    /// <param name="ani"></param>
    /// <param name="settors">The parameter info</param>
    public static void SetBool(Animator ani, params (int id, bool value)[] settors)
    {
        foreach (var (id, value) in settors) ani.SetBool(id, value);
    }
}