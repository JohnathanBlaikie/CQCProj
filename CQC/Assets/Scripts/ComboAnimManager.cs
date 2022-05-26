using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAnimManager : MonoBehaviour
{
    public Animator animatorA, animatorB;

    public void PlayJointAnimation(string _AnimatorClipNameA, string _AnimatorClipNameB)
    {
        animatorA.Play(_AnimatorClipNameA);
        animatorB.Play(_AnimatorClipNameB);
    }
}
