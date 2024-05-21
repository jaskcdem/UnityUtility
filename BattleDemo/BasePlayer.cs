using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

/// <summary>  </summary>
/// <remarks>using unity method : Start, Update(Public), FixedUpdate, OnDrawGizmos</remarks>
public class BasePlayer : MonoBehaviour
{
    public int MaxHp = 10, Hp, MaxMp = 10, Mp;
    public float AttackRange = 1.5f;
    GameUtility gu;
    protected bool bDead = false;
    Vector3 startPos;

    //player bar setting : subBar is what under mainBar but not background
    [SerializeField] protected SerializePlayerBar PlayerBar;

    //floating bar setting
    [SerializeField] protected SerializePlayerFloatingBar PlayerFloatingBar;
    protected string barKeyHP, barkeyMP;

    private void Start()
    {
        gu = new(transform);
        startPos = transform.position;
        Debug.Assert(PlayerBar != null || PlayerFloatingBar != null, "[PlayerBar] and [PlayerFloatingBar] can not both empty");
        if (PlayerFloatingBar != null && PlayerFloatingBar.showFloating)
        {
            PlayerFloatingBar.hpBar = PlayerFloatingBar.hpBar != null
                ? PlayerFloatingBar.hpBar : MonoUIManager.Instance.CreateFloatingBar(transform, PlayerFloatingBar.defHpBarIndex, Vector3.up * PlayerFloatingBar.floatingStartY);
            PlayerFloatingBar.hpBar.EditFloatHeight(PlayerFloatingBar.floatingStartY);
            barKeyHP = MonoUIManager.Instance.AddBarToDic(PlayerFloatingBar.hpBar.GetDefultBar());

            PlayerFloatingBar.mpBar = PlayerFloatingBar.mpBar != null
                ? PlayerFloatingBar.mpBar : MonoUIManager.Instance.CreateFloatingBar(transform, PlayerFloatingBar.defMpBarIndex, Vector3.up * PlayerFloatingBar.floatingStartY);
            PlayerFloatingBar.mpBar.EditFloatHeight(PlayerFloatingBar.floatingStartY);
            barkeyMP = MonoUIManager.Instance.AddBarToDic(PlayerFloatingBar.mpBar.GetDefultBar());
        }

        if (PlayerBar != null)
        {
            Debug.Assert(PlayerBar.Healthbar, "if [Healthbar] is not using, please turn on [showFloating]");
            Debug.Assert(PlayerBar.Manabar, "if [Manabar] is not using, please turn on [showFloating]");
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attacking();
        }
    }

    private void FixedUpdate()
    {
        if (bDead)
        {
            transform.position = startPos;
            Revival();
        }
        if (transform.position.y <= -50) transform.position = startPos;
    }

    private void OnDrawGizmos()
    {
        gu ??= new(transform);
        gu.DrawWireSphere(AttackRange, Color.cyan);
    }

    public virtual void Attacking()
    {
        var eList = GlobalValues.AliveEnemies;
        Vector3 selfPos = transform.position;
        BaseEnemy enemy;
        for (int i = 0; i < eList.Count; i++)
        {
            enemy = eList[i];
            float d = Vector3.Distance(selfPos, eList[i].transform.position);
            if (d < AttackRange)
            {
                Debug.Log("Attack---!!!");
                enemy.Damage();
            }
        }
    }

    #region << HP/MP Change >>
    public virtual void Damage(int amount)
    {
        if (bDead) return;
        if (amount > 0) amount = -amount;
        Hp += amount;
        if (Hp <= 0) { Hp = 0; bDead = true; }
        Debug.Log($"{gameObject.name} Leaf Hp: {Hp}");
        AdjustBar((PlayerFloatingBar.hpBar, barKeyHP, PlayerBar.Healthbar, PlayerBar.HealthbarSub), (amount, Hp, MaxHp));
    }

    public virtual void Burnout(int amount)
    {
        if (amount > 0) amount = -amount;
        Mp += amount;
        if (Mp <= 0) Mp = 0;
        Debug.Log($"{gameObject.name} Leaf Mp: {Mp}");
        AdjustBar((PlayerFloatingBar.mpBar, barkeyMP, PlayerBar.Manabar, PlayerBar.ManabarSub), (amount, Mp, MaxMp), Color.cyan);
    }

    public virtual void Heal(int amount)
    {
        Hp += amount;
        if (Hp >= MaxHp) Hp = MaxHp;
        Debug.Log($"{gameObject.name} Leaf Hp: {Hp}");
        AdjustBar((PlayerFloatingBar.hpBar, barKeyHP, PlayerBar.HealthbarSub, PlayerBar.Healthbar), (amount, Hp, MaxHp), Color.green);
    }

    public virtual void Recover(int amount)
    {
        Mp += amount;
        if (Mp >= MaxHp) Mp = MaxMp;
        Debug.Log($"{gameObject.name} Leaf Mp: {Mp}");
        AdjustBar((PlayerFloatingBar.mpBar, barkeyMP, PlayerBar.ManabarSub, PlayerBar.Manabar), (amount, Mp, MaxMp), Color.blue);
    }

    public virtual void Revival()
    {
        Hp = MaxHp;
        int recover = MaxMp / 2;
        Mp += recover;
        AdjustBar((PlayerFloatingBar.hpBar, barKeyHP, PlayerBar.HealthbarSub, PlayerBar.Healthbar), (MaxHp, Hp, MaxHp), Color.green);
        AdjustBar((PlayerFloatingBar.mpBar, barkeyMP, PlayerBar.ManabarSub, PlayerBar.Manabar), (recover, Mp, MaxMp), Color.blue);
        bDead = false;
    }
    #endregion

    /// <summary> bar ui control </summary>
    /// <param name="bars">bar info</param>
    /// <param name="values">change value info</param>
    /// <param name="textColor">floting text color</param>
    protected void AdjustBar((BaseFloatingBar barScript, string key, Image suddenChangeBar, Image slowChangeBar) bars,
        (float amount, float changed, float max) values, Color textColor = default)
    {
        string flowText = values.amount switch { > 0 => $"+{values.amount}", _ => values.amount.ToString() };
        if (textColor != default)
            MonoUIManager.Instance.SpawnFloatingText(flowText, PlayerFloatingBar.defTextIndex, transform.position + Vector3.up * PlayerFloatingBar.floatingStartY, textColor);
        else
            MonoUIManager.Instance.SpawnFloatingText(flowText, PlayerFloatingBar.defTextIndex, transform.position + Vector3.up * PlayerFloatingBar.floatingStartY);

        float _pcent = values.changed / values.max;
        if (PlayerFloatingBar.showFloating && bars.barScript) MonoUIManager.Instance.UpdateBar(_pcent, bars.key);
        if (bars.suddenChangeBar) MonoUIManager.Instance.UpdateBar(_pcent, bars.suddenChangeBar);
        if (bars.slowChangeBar) StartCoroutine(MonoUIManager.Instance.UpdateBarAsny(_pcent, bars.slowChangeBar, 3f));
    }
}
