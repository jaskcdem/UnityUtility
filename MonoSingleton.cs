using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour
{
    /// <summary> 實例 </summary>
  static  T _instance;
    /// <summary> 物件實例 </summary>
    public static T Instance => _instance ??= Activator.CreateInstance<T>();
}
