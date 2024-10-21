using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityCallStack : MonoBehaviour
{

    private static PriorityCallStack _Instance;
    public static PriorityCallStack Instance()
    {
        if (_Instance is null)
        {
            GameObject stackObject = new GameObject("PriorityCallStackHelper");
            _Instance = stackObject.AddComponent<PriorityCallStack>();
        }
        return _Instance;
    }

    private List<(Action, uint)> _ActionList = new List<(Action, uint)>();

    private Stack<Action> _ToInvoke = new Stack<Action>();

    void Update()
    {
        UpdateStack();
        InvokeThisFrame();
    }

    private void InvokeThisFrame()
    {
        for(int i = 0; i < _ToInvoke.Count; i++)
        {
            var action = _ToInvoke.Pop();
            action.Invoke();
        }
    }

    private void UpdateStack()
    {
        for(var i = _ActionList.Count - 1; i >= 0; i--)
        {
            if(_ActionList[i].Item2 == 0)
            {
                _ToInvoke.Push(_ActionList[i].Item1);
                _ActionList.RemoveAt(i);
            }
            else _ActionList[i] = (_ActionList[i].Item1, _ActionList[i].Item2 - 1);
        }
    }

    public void Push(Action action, uint delay)
    {
        _ActionList.Add((action, delay));
    }

}
