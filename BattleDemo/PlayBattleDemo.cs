using GreenUtility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayBattleDemo : MonoBehaviour
{
    //playing setting
    [SerializeField] GameObject[] inputTempletes;
    [SerializeField] int startNumber = 20;
    [SerializeField] int popupNumber = 5;
    [SerializeField] bool autoExpand = false;
    [SerializeField] Vector3 PositionRangeMin = new(-10.0f, 0.5f, -10.0f);
    [SerializeField] Vector3 PositionRangeMax = new(10.0f, 1.0f, 10.0f);

    List<(GlobalValues.EnemyType eType, GameObject obj)> templetes = new();
    (Vector3 min, Vector3 max) PositionRange; //Serialize valueTuple is too hard to Unity
    float second; // Time Measurement
    [SerializeField] int startItemNumber = 1, popupItemNumber = 5;
    [SerializeField] string[] ItemResourcesPath = new[] { "HealItem", "RecoverItem", "DamageItem", "BurnoutItem" };

    private void Awake()
    {
        PositionRange = (PositionRangeMin, PositionRangeMax);
        foreach (var t in inputTempletes)
        {
            //Instantiate(t);
            if (t.TryGetComponent(out BaseEnemy @base))
                templetes.Add((@base.Type, t));
        }
    }

    void Start()
    {
        if (templetes != null && templetes.Count > 0)
        {
            foreach (var (eType, obj) in templetes)
                BattleManager.InitDatas(eType, obj, startNumber);
        }

        if (startItemNumber > 0)
            foreach (string path in ItemResourcesPath)
            {
                for (int i = 0; i < startItemNumber; i++)
                {
                    GameObject item = MonoResourcesLoader.Instance.LoadGameObject(path);
                    item.transform.SetLocalPositionAndRotation(GameUtility.Range(PositionRange), Quaternion.identity);
                    Instantiate(item);
                }
            }
    }

    void Update()
    {
        second += Time.deltaTime;
        if ((int)second % 7 == 0 && popupItemNumber > 0)
        {
            GameObject[] pus = GameObject.FindGameObjectsWithTag("PowerUp");
            if (pus.Length <= popupItemNumber)
                foreach (string path in ItemResourcesPath)
                {
                    for (int i = 0; i < popupItemNumber; i++)
                    {
                        GameObject item = MonoResourcesLoader.Instance.LoadGameObject(path);
                        item.transform.SetLocalPositionAndRotation(GameUtility.Range(PositionRange), Quaternion.identity);
                        Instantiate(item);
                    }
                }
        }
        if (second >= 15)
        {
            int _aliveCnt = GlobalValues.AliveEnemies.Count;
            var popList = GlobalValues.AliveEnemies.GroupBy(z => z.Type).Select(x => (x.Key, x.Count())).ToList();
            if (popList.Count <= 0) popList.AddRange(templetes.Select(x => (x.eType, 0)));
            else popList.AddRange(templetes.Where(z => !popList.Select(y => y.Key).Contains(z.eType)).Select(x => (x.eType, 0)));
            PopUpEnemies(popList);
            second = 0;
        }
    }

    void PopUpEnemies(IEnumerable<(GlobalValues.EnemyType eType, int cnt)> popList)
    {
        var eList = GlobalValues.AliveEnemies;
        foreach (var (eType, cnt) in popList)
        {
            //Debug.Log($"{eType}:{cnt}");
            if (cnt < popupNumber)
            {
                var popUps = BattleManager.LoadDataFromPool(eType, popupNumber, autoExpand);
                if (popUps != null && popUps.Any())
                    for (int i = 0; i < popUps.Count(); i++)
                    {
                        GameObject goo = popUps.ElementAt(i);
                        if (goo != null)
                        {
                            goo.transform.position = GameUtility.Range(PositionRange);
                            goo.transform.Rotate(0, Random.Range(-180.0f, 180.0f), 0.0f);
                            BaseEnemy @base = goo.GetComponent<BaseEnemy>();
                            @base.Init();
                            @base.ReflashBar();
                            eList.Add(@base);
                        }
                    }
            }
        }
    }
}
