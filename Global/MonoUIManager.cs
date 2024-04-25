using System;
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

    //player bar
    public Dictionary<string, Image> DicBars;

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

        Debug.Assert(mainCanvas == null, "[Main Canvas] must have value!");
        Debug.Assert(mainCanvasRect == null, "[Main Canvas RectTransform] must have value!");
        Debug.Assert(floatingRoot == null, "[Floating Root] must have value!");
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
                    floatingBars[i].UpdateBarToFollowTarget((mainCanvas, floatingRoot, mainCamera), 2.0f);
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

    #region Bar for hp or other
    public void UpdateBar(float amount, string barKey)
    {
        float fAmount = amount switch { > 1 => 1, < 0 => 0, _ => amount };
        DicBars[barKey].fillAmount = fAmount;
    }

    public BaseFloatingBar CreateFloatingBar(Transform followed, int sourceIndex)
    {
        GameObject go = Instantiate(barSource[sourceIndex]);
        BaseFloatingBar b = go.GetComponent<BaseFloatingBar>();
        b.SetFollowTarget(followed);
        b.transform.SetParent(floatingRoot);
        //b.transform.localPosition = Vector3.zero;
        b.transform.localRotation = Quaternion.identity;
        b.transform.localScale = Vector3.one;
        floatingBars.Add(b);
        return b;
    }

    public void DeleteFloatingBar(BaseFloatingBar b)
    {
        floatingBars.Remove(b);
        Destroy(b.gameObject);
    }
    #endregion

    #region Show damage (simple)
    public void SpawnFloatingText(string showText, int sourceIndex, Vector3 vPos)
    {
        GameObject go = Instantiate(textSource[sourceIndex]);
        BaseFloatingText ft = go.GetComponent<BaseFloatingText>();
        ft.ResetTextColor();
        ft.Setup(showText, (mainCanvas, floatingRoot, mainCamera), vPos);
        ft.transform.SetParent(floatingRoot);
        //ft.transform.localPosition = Vector3.zero;
        ft.transform.localRotation = Quaternion.identity;
        ft.transform.localScale = Vector3.one;
        floatingTexts.Add(ft);
    }
    public void SpawnFloatingText(string showText, int sourceIndex, Vector3 vPos, Color color)
    {
        GameObject go = Instantiate(textSource[sourceIndex]);
        BaseFloatingText ft = go.GetComponent<BaseFloatingText>();
        //go.transform.GetComponent<TMP_Text>().color = color;
        ft.ChangeTextColor(color);
        ft.Setup(showText, (mainCanvas, floatingRoot, mainCamera), vPos);
        ft.transform.SetParent(floatingRoot);
        //ft.transform.localPosition = Vector3.zero;
        ft.transform.localRotation = Quaternion.identity;
        ft.transform.localScale = Vector3.one;
        floatingTexts.Add(ft);
    }
    #endregion

}
