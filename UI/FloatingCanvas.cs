using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FloatingRootInfo
{
    public FloatingCanvas canvas;
    public Transform Root;
}

public class FloatingCanvas
{
    public (string name, int id) SortLayer = ("Default", 0);
    public int SortingOrder = 0;
}
