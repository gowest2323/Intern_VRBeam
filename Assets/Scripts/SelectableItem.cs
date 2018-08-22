using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItem : MonoBehaviour
{
    [SerializeField]
    private SelectionAnswer selectionAns;
    public SelectionAnswer SelectionAns
    {
        get { return selectionAns; }
    }

    public enum SelectionAnswer
    {
        GameStart,
        Yes,
        No
    }
}
