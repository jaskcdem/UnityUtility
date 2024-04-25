using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleManager
{
    static List<GameEnemyDataPool> _dataPools = new();

    /// <summary> 初始化或擴增資源池 </summary>
    /// <param name="etype">類別</param>
    /// <param name="templete">資源範本(.prefab)</param>
    /// <param name="number">初始化或擴增數量</param>
    public static void InitDatas(GlobalValues.EnemyType etype, GameObject templete, int number)
    {
        GameEnemyDataPool p = FindByType(etype) ?? new();
        p.SetEmType(etype).SetSrcTemplete(templete);
        for (int i = 0; i < number; i++)
        {
            GameObject gObj = GameObject.Instantiate(templete);
            gObj.SetActive(false);
            gObj.name += $"_{Guid.NewGuid().ToString().Replace("-", "")[..6]}";
            while (p.DataList.Any(y => y.name == gObj.name))
                gObj.name = $"{templete.name}_{Guid.NewGuid().ToString().Replace("-", "")[..6]}";
            p.DataList.Add(gObj);
        }
        _dataPools.Add(p);
    }

    /// <summary> 依類別尋找資源池 </summary>
    /// <returns>找到的首個資源池，若沒有則給null</returns>
    public static GameEnemyDataPool FindByType(GlobalValues.EnemyType etype)
    {
        var pool = _dataPools.FirstOrDefault(x => x.EmType.Equals(etype));
        if (pool == default) pool = null;
        return pool;
    }

    /// <summary> 載入資源 </summary>
    /// <param name="etype">類別</param>
    /// <param name="loadNumber">載入數</param>
    /// <param name="autoExpand">是否自動擴增</param>
    /// <returns>可用資源。若無對應類別資源池或載入數不足1，則給空資料</returns>
    public static IEnumerable<GameObject> LoadDataFromPool(GlobalValues.EnemyType etype, int loadNumber, bool autoExpand = false)
    {
        var p = FindByType(etype);
        if (p == null || loadNumber < 1) return Enumerable.Empty<GameObject>();
        var loaded = p.DataList.Where(x => !x.activeSelf).Take(loadNumber).ToList();
        if (autoExpand && loaded.Count < loadNumber)
        {
            int _expandNum = loadNumber - loaded.Count;
            GameObject templete = loaded.Last();
            InitDatas(etype, templete, _expandNum);
            loaded.AddRange(p.DataList.Where(x => !loaded.Contains(x) && !x.activeSelf).Take(_expandNum));
        }
        foreach (var go in loaded) go.SetActive(true);
        return loaded;
    }
    /// <summary> 卸載資源 </summary>
    /// <remarks>如果<paramref name="unloadObj"/>不在對應資源池內，則不動作</remarks>
    /// <param name="etype">類別</param>
    /// <param name="unloadObj">要卸載的資源</param>
    public static void UnLoadDataToPool(GlobalValues.EnemyType etype, GameObject unloadObj)
    {
        var p = FindByType(etype) ?? new();
        var _checker = p.DataList.FirstOrDefault(x => x.Equals(unloadObj));
        if (_checker != default) unloadObj.SetActive(false);
    }
    /// <summary> 卸載資源 </summary>
    /// <remarks>如果<paramref name="unloadObjs"/>不在對應資源池內，則不動作</remarks>
    /// <param name="etype">類別</param>
    /// <param name="unloadObjs">要卸載的資源</param>
    public static void UnLoadDataToPool(GlobalValues.EnemyType etype, params GameObject[] unloadObjs)
    {
        foreach (var obj in unloadObjs) UnLoadDataToPool(etype, obj);
    }
}
