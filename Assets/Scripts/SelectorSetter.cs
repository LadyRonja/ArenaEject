using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorSetter : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(SetThisSelected), 6.5f);
    }

    private void SetThisSelected()
    {
        if(TryGetComponent<Selectable>(out Selectable me))
        {
            me.Select();
        }
    }
}
