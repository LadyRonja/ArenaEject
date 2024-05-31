using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Serializable Animation Curve", order = 999)]
public class SerializedAnimationCurve : ScriptableObject
{
    public AnimationCurve curve;
}
