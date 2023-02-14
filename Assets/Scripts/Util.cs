using System;
using System.Collections;
using UnityEngine;

public class Util
{
    public static IEnumerator OnAnimationFinish(GameObject obj, string name, Action callback)
    {
        Animator anim = obj.GetComponent<Animator>();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(name));
        yield return new WaitWhile(() => anim.GetCurrentAnimatorStateInfo(0).IsName(name) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        callback();
    }
}
