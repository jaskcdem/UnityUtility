using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour
{
    /// <summary> ��� </summary>
  static  T _instance;
    /// <summary> ������ </summary>
    public static T Instance => _instance ??= Activator.CreateInstance<T>();
}
