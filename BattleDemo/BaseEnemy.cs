using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  </summary>
/// <remarks>using unity method : Start(Public), FixedUpdate(Public), OnDestroy</remarks>
public class BaseEnemy : MonoBehaviour
{
    public GlobalValues.EnemyType Type;
    public int MaxHp = 10;
    public int Hp;
    protected bool bDead = false;

    //enemy bar setting : subBar is what under mainBar but not background
    [SerializeField] protected SerializeSingleBar EnemyBar;

    //floating bar setting
    [SerializeField] protected SerializeSingleFloatingBar EnemyFloatingBar;
    protected string barKey;
    protected FloatingCanvas baseCanvas = new() { SortLayer = ("Enemys", 2), SortingOrder = 99 };
    //floating root
    Canvas floatRoot;

    public BaseEnemy()
    {
        Hp = MaxHp;
    }

    public void Start()
    {
        Debug.Assert(EnemyBar != null || EnemyFloatingBar != null, "[EnemyBar] and [EnemyFloatingBar] can not both empty");
        EnemyFloatingBar.Bar = MonoUIManager.Instance.CreateFloatingBar(transform, EnemyFloatingBar.defBarIndex, Vector3.up * EnemyFloatingBar.floatingStartY,
            isPlayer: false, enable: true, scaleSize: new(0.01f, 0.01f, 0.01f), canvas: baseCanvas);
        EnemyFloatingBar.Bar.EditFloatHeight(EnemyFloatingBar.floatingStartY);
        barKey = MonoUIManager.Instance.AddBarToDic(EnemyFloatingBar.Bar.GetDefultBar());
        floatRoot = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).FirstOrDefault(c => c.transform.parent == transform);
        //auto-binding
        if (EnemyBar.BarMain == null && !string.IsNullOrWhiteSpace(EnemyFloatingBar.mainChildName))
        {
            Transform tBar = EnemyFloatingBar.Bar.transform.Find(EnemyFloatingBar.mainChildName);
            if (tBar && tBar.TryGetComponent<Image>(out Image imBar)) EnemyBar.BarMain = imBar;
        }
        if (EnemyBar.BarSub == null && !string.IsNullOrWhiteSpace(EnemyFloatingBar.subChildName))
        {
            Transform tBar = EnemyFloatingBar.Bar.transform.Find(EnemyFloatingBar.subChildName);
            if (tBar && tBar.TryGetComponent<Image>(out Image imBar)) EnemyBar.BarSub = imBar;
        }
    }

    public void FixedUpdate()
    {
        if (bDead)
        {
            Deaded();
        }
    }

    private void OnDestroy()
    {
        if (MonoUIManager.Instance != null && !string.IsNullOrWhiteSpace(barKey))
            MonoUIManager.Instance.RemoveBarFromDic(barKey);
    }

    public virtual void Init()
    {
        bDead = false;
        Hp = MaxHp;
    }

    public void ReflashBar()
    {
        if (EnemyBar.BarMain && EnemyBar.BarSub)
            AdjustBar(bars: (EnemyFloatingBar.Bar, barKey, EnemyBar.BarSub, EnemyBar.BarMain), values: (MaxHp, Hp, MaxHp));
    }

    public virtual void Damage(int amount)
    {
        if (bDead) return;
        if (amount > 0) amount = -amount;
        Hp += amount;
        if (Hp <= 0) { Hp = 0; bDead = true; }
        Debug.Log($"{gameObject.name} Leaf Hp: {Hp}");
        AdjustBar(bars: (EnemyFloatingBar.Bar, barKey, EnemyBar.BarMain, EnemyBar.BarSub), values: (amount, Hp, MaxHp));
    }

    public virtual void Heal(int amount)
    {
        Hp += amount;
        if (Hp >= MaxHp) Hp = MaxHp;
        Debug.Log($"{gameObject.name} Leaf Hp: {Hp}");
        AdjustBar(bars: (EnemyFloatingBar.Bar, barKey, EnemyBar.BarSub, EnemyBar.BarMain), values: (amount, Hp, MaxHp));
    }

    public virtual void Deaded()
    {
        var eList = GlobalValues.AliveEnemies;
        eList.RemoveAll(e => e.name.Equals(gameObject.name));
        BattleManager.UnLoadDataToPool(Type, gameObject);
        int _aliveCnt = eList.Count;
        Debug.Log(_aliveCnt switch
        {
            0 => $"No enemies are alive.",
            1 => $"Left only {_aliveCnt} enemy is alive.",
            _ => $"Still {_aliveCnt} enemies are alive.",
        });
        //GlobalValues.AliveEnemies = eList;
    }

    /// <summary> bar ui control </summary>
    /// <param name="bars">bar info</param>
    /// <param name="values">change value info</param>
    /// <param name="textColor">floting text color</param>
    protected void AdjustBar((BaseFloatingBar barScript, string key, Image suddenChangeBar, Image slowChangeBar) bars,
        (float amount, float changed, float max) values, Color textColor = default)
    {
        string flowText = values.amount switch { > 0 => $"+{values.amount}", _ => values.amount.ToString() };
        if (textColor != default)
            MonoUIManager.Instance.SpawnFloatingText(flowText, EnemyFloatingBar.defTextIndex, Vector3.up * EnemyFloatingBar.floatingStartY, textColor, isPlayer: false,
              scaleSize: new(0.01f, 0.01f, 0.01f), followed: floatRoot ? floatRoot.transform : transform, canvas: baseCanvas);
        else
            MonoUIManager.Instance.SpawnFloatingText(flowText, EnemyFloatingBar.defTextIndex, Vector3.up * EnemyFloatingBar.floatingStartY, isPlayer: false,
              scaleSize: new(0.01f, 0.01f, 0.01f), followed: floatRoot ? floatRoot.transform : transform, canvas: baseCanvas);

        float _pcent = values.changed / values.max;
        if (bars.barScript.gameObject.activeSelf) MonoUIManager.Instance.UpdateBar(_pcent, bars.key);
        if (bars.suddenChangeBar) MonoUIManager.Instance.UpdateBar(_pcent, bars.suddenChangeBar);
        if (bars.slowChangeBar) StartCoroutine(MonoUIManager.Instance.UpdateBarAsny(_pcent, bars.slowChangeBar, 3f));
    }
}
