using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleManager
{
    static List<GameEnemyDataPool> _dataPools = new();

    /// <summary> ��l�Ʃ��X�W�귽�� </summary>
    /// <param name="etype">���O</param>
    /// <param name="templete">�귽�d��(.prefab)</param>
    /// <param name="number">��l�Ʃ��X�W�ƶq</param>
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

    /// <summary> �����O�M��귽�� </summary>
    /// <returns>��쪺���Ӹ귽���A�Y�S���h��null</returns>
    public static GameEnemyDataPool FindByType(GlobalValues.EnemyType etype)
    {
        var pool = _dataPools.FirstOrDefault(x => x.EmType.Equals(etype));
        if (pool == default) pool = null;
        return pool;
    }

    /// <summary> ���J�귽 </summary>
    /// <param name="etype">���O</param>
    /// <param name="loadNumber">���J��</param>
    /// <param name="autoExpand">�O�_�۰��X�W</param>
    /// <returns>�i�θ귽�C�Y�L�������O�귽���θ��J�Ƥ���1�A�h���Ÿ��</returns>
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
    /// <summary> �����귽 </summary>
    /// <remarks>�p�G<paramref name="unloadObj"/>���b�����귽�����A�h���ʧ@</remarks>
    /// <param name="etype">���O</param>
    /// <param name="unloadObj">�n�������귽</param>
    public static void UnLoadDataToPool(GlobalValues.EnemyType etype, GameObject unloadObj)
    {
        var p = FindByType(etype) ?? new();
        var _checker = p.DataList.FirstOrDefault(x => x.Equals(unloadObj));
        if (_checker != default) unloadObj.SetActive(false);
    }
    /// <summary> �����귽 </summary>
    /// <remarks>�p�G<paramref name="unloadObjs"/>���b�����귽�����A�h���ʧ@</remarks>
    /// <param name="etype">���O</param>
    /// <param name="unloadObjs">�n�������귽</param>
    public static void UnLoadDataToPool(GlobalValues.EnemyType etype, params GameObject[] unloadObjs)
    {
        foreach (var obj in unloadObjs) UnLoadDataToPool(etype, obj);
    }
}
