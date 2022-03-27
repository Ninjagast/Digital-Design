using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseState : MonoBehaviour
{
    private static MouseState _current;
    public static MouseState Current => _current;

    public bool mouseOverSheet;

    private void Awake()
    {
        _current = this;
    }
}
