using GreenUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal partial class MonoUIManager : MonoSingleton<MonoUIManager>
{
    //ui panel setting
    public Canvas mainCanvas;
    public Camera mainCamera;
    public RectTransform floatingRoot;
    RectTransform mainCanvasRect;
    [SerializeField] string floatingRootName = "floatingBar";

    //floating object(.prefab)
    public GameObject[] barSource, textSource;
    List<BaseFloatingBar> floatingBars = new();
    List<BaseFloatingText> floatingTexts = new();
    [SerializeField] string floatingBarMane = "FloatingBar";
    [SerializeField] string floatingTextMane = "FloatingText (TMP)";
    bool bShowBar = false;

    //bar control
    Dictionary<string, Image> DicBars = new();

    #region unity function
    void Awake()
    {
        try
        {
            if (mainCanvas == null) mainCanvas = FindObjectsOfType<Canvas>().FirstOrDefault();
            if (mainCanvas != null) mainCanvasRect = mainCanvas.GetComponent<RectTransform>();
            if (mainCamera == null) mainCamera = Camera.main;
            if (floatingRoot == null) floatingRoot = GameObject.FindGameObjectsWithTag(floatingRootName).FirstOrDefault().GetComponent<RectTransform>();
        }
        catch (Exception ex)
        {
            Debug.LogError("Find Object go failing, please check your project");
            if (Application.isEditor) Debug.Log(ex.ToString());
            return;
        }

        Debug.Assert(mainCanvas != null, "[Main Canvas] must have value!");
        Debug.Assert(mainCanvasRect != null, "[Main Canvas RectTransform] must have value!");
        Debug.Assert(floatingRoot != null, "[Floating Root] must have value!");
    }
    void Start()
    {
        barSource ??= MonoResourcesLoader.Instance.LoadAllObjects<Image>(floatingBarMane).Select(g => g.gameObject).ToArray();
        textSource ??= MonoResourcesLoader.Instance.LoadAllObjects<TMP_Text>(floatingTextMane).Select(g => g.gameObject).ToArray();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (!bShowBar)
            {
                for (int i = 0; i < floatingBars.Count; i++)
                {
                    if (!floatingBars[i].gameObject.activeSelf)
                    {
                        floatingBars[i].gameObject.SetActive(true);
                        continue;
                    }
                    //floatingBars[i].UpdateBarToFollowTarget((mainCanvas, floatingRoot, mainCamera));
                }

                bShowBar = true;
            }
            else
            {
                for (int i = floatingBars.Count - 1; i >= 0; i--)
                    DeleteFloatingBar(floatingBars[i]);
                bShowBar = false;
            }
        }

        for (int i = floatingTexts.Count - 1; i >= 0; i--)
        {
            if (!floatingTexts[i].UpdatePosition())
                floatingTexts.RemoveAt(i);
        }
    }
    #endregion
    public RectTransform GetCanvasTransform() => mainCanvasRect;
    public string AddBarToDic(Image bar, string barName = null)
    {
        if (string.IsNullOrWhiteSpace(barName) || DicBars.ContainsKey(barName))
            barName = $"{bar.name}_{Guid.NewGuid().ToString().Replace("-", "")[..16]}";
        DicBars.Add(barName, bar);
        return barName;
    }
    public bool RemoveBarFromDic(string barName) => DicBars.Remove(barName);
    public void ClearBarDic() => DicBars.Clear();

    #region Bar for hp or other
    public void UpdateBar(float amount, string barKey)
    {
        float fAmount = amount switch { > 1 => 1, < 0 => 0, _ => amount };
        if (!DicBars.TryGetValue(barKey, out Image bar)) return;
        bar.fillAmount = fAmount;
    }
    public void UpdateBar(float amount, Image bar)
    {
        if (bar == null) return;
        float fAmount = amount switch { > 1 => 1, < 0 => 0, _ => amount };
        bar.fillAmount = fAmount;
    }
    public IEnumerator UpdateBarAsny(float endPrecent, Image bar, float timeSpeed = 10)
    {
        while (MathF.Abs(bar.fillAmount - endPrecent) > 0.01f)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, endPrecent, Time.deltaTime * timeSpeed);
            yield return null;
        }
        bar.fillAmount = endPrecent;
    }

    /// <summary> Create Floating Bar from <see cref="barSource"/> </summary>
    /// <param name="followed">new Floating Bar follow to</param>
    /// <param name="sourceIndex">index of <see cref="barSource"/></param>
    /// <param name="localPos">Local Position</param>
    /// <param name="isPlayer">floating bar belong to player or not</param>
    /// <param name="scaleSize">bar scale size, default = <see cref="Vector3.one"/></param>
    /// <param name="enable">auto set <see cref="GameObject.activeSelf"/></param>
    /// <param name="canvas">new Canvas setting when <paramref name="followed"/> not a canvas</param>
    /// <returns>New Floating Bar. if index out of range, return null</returns>
    public BaseFloatingBar CreateFloatingBar(Transform followed, int sourceIndex, Vector3 localPos, bool isPlayer = true, Vector3 scaleSize = default,
        bool enable = true, FloatingCanvas canvas = null)
    {
        if (sourceIndex > barSource.Length - 1) return null;
        //Debug.Log($"CreateFloatingBar : {followed.name} => {sourceIndex} / { barSource.Length } ");
        GameObject go = Instantiate(barSource[sourceIndex]);
        BaseFloatingBar b = go.GetComponent<BaseFloatingBar>();
        b.SetFollowTarget(followed);
        SetFloatingParent(new FloatingRootInfo { Root = isPlayer ? floatingRoot : followed, canvas = canvas }, b.transform);
        b.transform.SetLocalPositionAndRotation(localPos, Quaternion.identity);
        b.transform.localScale = scaleSize == default ? Vector3.one : scaleSize;
        b.gameObject.SetActive(enable);
        floatingBars.Add(b);
        return b;
    }

    public void DeleteFloatingBar(BaseFloatingBar b)
    {
        Transform followed = b.GetFollowTarget();
        if (followed == null)
        {
            floatingBars.Remove(b);
            Destroy(b.gameObject);
        }
        else if (followed.gameObject.activeSelf) b.gameObject.SetActive(false);
    }
    #endregion

    #region Show damage (simple)
    /// <summary> show Floating text<para>if index out of range, do nothing</para> </summary>
    /// <param name="showText">text will show</param>
    /// <param name="sourceIndex">index of <see cref="textSource"/></param>
    /// <param name="vPos">start position</param>
    /// <param name="isPlayer">floating text belong to player or not</param>
    /// <param name="scaleSize">bar scale size, default = <see cref="Vector3.one"/></param>
    /// <param name="followed">new Floating Text follow to</param>
    /// <param name="canvas">new Canvas setting when <paramref name="followed"/> not a canvas</param>
    public void SpawnFloatingText(string showText, int sourceIndex, Vector3 vPos, bool isPlayer = true, Vector3 scaleSize = default,
        Transform followed = null, FloatingCanvas canvas = null)
    {
        if (sourceIndex > textSource.Length - 1) return;
        GameObject go = Instantiate(textSource[sourceIndex]);
        BaseFloatingText ft = go.GetComponent<BaseFloatingText>();
        ft.ResetTextColor();
        ft.Setup(showText, (mainCanvas, floatingRoot, mainCamera), vPos);
        Debug.Assert(isPlayer || followed != null, "when [isPlayer] is false and [followed] is null, will take [floatingRoot] for default.");
        SetFloatingParent(new FloatingRootInfo { Root = isPlayer || followed == null ? floatingRoot : followed, canvas = canvas }, ft.transform);
        if (!isPlayer) ft.transform.SetLocalPositionAndRotation(vPos, Quaternion.identity);
        else ft.transform.SetLocalPositionAndRotation(Vector3.one, Quaternion.identity);

        ft.transform.localScale = scaleSize == default ? Vector3.one : scaleSize;
        ft.gameObject.SetActive(true);
        floatingTexts.Add(ft);
    }
    /// <summary> show Floating text<para>if index out of range, do nothing</para> </summary>
    /// <param name="showText">text will show</param>
    /// <param name="sourceIndex">index of <see cref="textSource"/></param>
    /// <param name="vPos">start position</param>
    /// <param name="color">text color</param>
    /// <param name="isPlayer">floating text belong to player or not</param>
    /// <param name="scaleSize">bar scale size, default = <see cref="Vector3.one"/></param>
    /// <param name="followed">new Floating Text follow to</param>
    /// <param name="canvas">new Canvas setting when <paramref name="followed"/> not a canvas</param>
    public void SpawnFloatingText(string showText, int sourceIndex, Vector3 vPos, Color color, bool isPlayer = true, Vector3 scaleSize = default,
        Transform followed = null, FloatingCanvas canvas = null)
    {
        if (sourceIndex > textSource.Length - 1) return;
        GameObject go = Instantiate(textSource[sourceIndex]);
        BaseFloatingText ft = go.GetComponent<BaseFloatingText>();
        //go.transform.GetComponent<TMP_Text>().color = color;
        ft.ChangeTextColor(color);
        ft.Setup(showText, (mainCanvas, floatingRoot, mainCamera), vPos);
        Debug.Assert(isPlayer || followed != null, "when [isPlayer] is false and [followed] is null, will take [floatingRoot] for default.");
        SetFloatingParent(new FloatingRootInfo { Root = isPlayer || followed == null ? floatingRoot : followed, canvas = canvas }, ft.transform);
        if (!isPlayer) ft.transform.SetLocalPositionAndRotation(vPos, Quaternion.identity);
        else ft.transform.SetLocalPositionAndRotation(Vector3.one, Quaternion.identity);

        ft.transform.localScale = scaleSize == default ? Vector3.one : scaleSize;
        ft.gameObject.SetActive(true);
        floatingTexts.Add(ft);
    }
    #endregion

    /// <summary> player show on canvas, other bind to followed </summary>
    /// <remarks>notice : floatingRoot should not place too deep to find</remarks>
    void SetFloatingParent(FloatingRootInfo rootInfo, Transform child)
    {
        if (rootInfo.Root.TryGetComponent<Canvas>(out _) || (rootInfo.Root.parent != null && rootInfo.Root.parent.TryGetComponent<Canvas>(out _)))
            child.SetParent(rootInfo.Root);
        else
        {
            GameObject _canvas = InitFloatingCanvas(rootInfo.canvas);
            _canvas.GetComponent<RectTransform>().SetPositionAndRotation(rootInfo.Root.position, Quaternion.identity);
            child.SetParent(_canvas.transform);
            _canvas.transform.SetParent(rootInfo.Root);
        }
    }

    GameObject InitFloatingCanvas(FloatingCanvas canvasInfo = null)
    {
        var canvasGO = new GameObject($"Canvas-{Guid.NewGuid().ToString().Replace("-", "")[..6]}");
        canvasGO.AddComponent<Canvas>();
        //new component
        Canvas _canvas = canvasGO.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.worldCamera = Camera.main;
        canvasInfo ??= new();
        _canvas.sortingLayerName = canvasInfo.SortLayer.name;
        _canvas.sortingOrder = canvasInfo.SortingOrder;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        canvasGO.transform.localScale = Vector3.one;
        return canvasGO;
    }
}
