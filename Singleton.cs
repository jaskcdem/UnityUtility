using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary> 唯一物件樣版 </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : class
{
    /// <summary> 實例 </summary>
    static T _instance;
    /// <summary> 物件實例 </summary>
    public static T Instance => _instance ??= Activator.CreateInstance<T>();
}