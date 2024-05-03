using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

/// <summary>  </summary>
/// <remarks>using unity method : Start, Update(Public), OnDrawGizmos</remarks>
public class BasePlayer : MonoBehaviour
{
    public int MaxHp = 10, Hp, MaxMp = 10, Mp;
    public float AttackRange = 1.5f;
    GameUtility gu;
    protected bool bDead = false;

    //player bar setting : subBar is what under mainBar but not background
    [SerializeField] protected SerializePlayerBar PlayerBar;

    //floating bar setting
    [SerializeField] protected SerializePlayerFloatingBar PlayerFloatingBar;
    protected string barKeyHP, barkeyMP;
    Vector3 StartPosition => transform.position + Vector3.up * PlayerFloatingBar.floatingStartY;

    private void Start()
    {
        gu = new(transform);
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

    public virtual void Damage()
    {
        if (bDead) return;
        Hp--;
        if (Hp <= 0) { Hp = 0; bDead = true; }
        Debug.Log($"{gameObject.name} Leaf Hp: {Hp}");
        MonoUIManager.Instance.SpawnFloatingText("-1", PlayerFloatingBar.defTextIndex, StartPosition);

        float _pcent = Hp / MaxHp;
        if (PlayerFloatingBar.showFloating && PlayerFloatingBar.hpBar) MonoUIManager.Instance.UpdateBar(_pcent, barKeyHP);
        if (PlayerBar.Healthbar) MonoUIManager.Instance.UpdateBar(_pcent, PlayerBar.Healthbar);
        if (PlayerBar.HealthbarSub) StartCoroutine(MonoUIManager.Instance.UpdateBarAsny(_pcent, PlayerBar.HealthbarSub));
    }

    public virtual void Burnout()
    {
        Mp--;
        if (Mp <= 0) Mp = 0;
        Debug.Log($"{gameObject.name} Leaf Mp: {Mp}");
        MonoUIManager.Instance.SpawnFloatingText("-1", PlayerFloatingBar.defTextIndex, StartPosition, Color.cyan);

        float _pcent = Mp / MaxMp;
        if (PlayerFloatingBar.showFloating && PlayerFloatingBar.mpBar) MonoUIManager.Instance.UpdateBar(_pcent, barkeyMP);
        if (PlayerBar.Manabar) MonoUIManager.Instance.UpdateBar(_pcent, PlayerBar.Manabar);
        if (PlayerBar.ManabarSub) StartCoroutine(MonoUIManager.Instance.UpdateBarAsny(_pcent, PlayerBar.ManabarSub));
    }

    public virtual void Heal()
    {
        Hp++;
        if (Hp >= MaxHp) Hp = MaxHp;
        Debug.Log($"{gameObject.name} Leaf Hp: {Hp}");
        MonoUIManager.Instance.SpawnFloatingText("+1", PlayerFloatingBar.defTextIndex, StartPosition, Color.green);

        float _pcent = Hp / MaxHp;
        if (PlayerFloatingBar.showFloating && PlayerFloatingBar.hpBar) MonoUIManager.Instance.UpdateBar(_pcent, barKeyHP);
        if (PlayerBar.HealthbarSub) MonoUIManager.Instance.UpdateBar(_pcent, PlayerBar.HealthbarSub);
        if (PlayerBar.Healthbar) StartCoroutine(MonoUIManager.Instance.UpdateBarAsny(_pcent, PlayerBar.Healthbar));
    }

    public virtual void Recover()
    {
        Mp++;
        if (Mp >= MaxHp) Mp = MaxMp;
        Debug.Log($"{gameObject.name} Leaf Mp: {Mp}");
        MonoUIManager.Instance.SpawnFloatingText("+1", PlayerFloatingBar.defTextIndex, StartPosition, Color.blue);

        float _pcent = Mp / MaxMp;
        if (PlayerFloatingBar.showFloating && PlayerFloatingBar.mpBar) MonoUIManager.Instance.UpdateBar(_pcent, barkeyMP);
        if (PlayerBar.ManabarSub) MonoUIManager.Instance.UpdateBar(_pcent, PlayerBar.ManabarSub);
        if (PlayerBar.Manabar) StartCoroutine(MonoUIManager.Instance.UpdateBarAsny(_pcent, PlayerBar.Manabar));
    }

    /// <summary> bar ui control </summary>
    /// <param name="bars">bar info</param>
    /// <param name="values">change value info</param>
    /// <param name="textColor">floting text color</param>
    protected void AdjustBar((BaseFloatingBar barScript, string key, Image suddenChangeBar, Image slowChangeBar) bars,
        (float amount, float changed, float max) values, Color textColor = default)
    {
        string flowText = values.amount switch { > 0 => $"+{values}", _ => values.amount.ToString() };
        if (textColor != default)
            MonoUIManager.Instance.SpawnFloatingText(flowText, PlayerFloatingBar.defTextIndex, StartPosition, textColor);
        else
            MonoUIManager.Instance.SpawnFloatingText(flowText, PlayerFloatingBar.defTextIndex, StartPosition);

        float _pcent = values.changed / values.max;
        if (PlayerFloatingBar.showFloating && bars.barScript) MonoUIManager.Instance.UpdateBar(_pcent, bars.key);
        if (bars.suddenChangeBar) MonoUIManager.Instance.UpdateBar(_pcent, bars.suddenChangeBar);
        if (bars.slowChangeBar) StartCoroutine(MonoUIManager.Instance.UpdateBarAsny(_pcent, bars.slowChangeBar));
    }
}
