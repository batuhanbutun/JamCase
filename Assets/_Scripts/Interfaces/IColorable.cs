using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public interface IColorable
{
    public ObjColor ObjColor { get; }
    public void ColorSetup(ObjColor color);
}
