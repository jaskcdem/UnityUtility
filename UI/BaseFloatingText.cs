using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseFloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text textSource;
    [SerializeField] string floatingTextNane = "FloatingText (TMP)";
    [SerializeField] Color defColor = Color.red;
    public float moveSpeed = 2.0f, moveDuration = 2.0f;
    private float fTimer = 0.0f;

    void Start()
    {
        LoadingSource();
    }
    void LoadingSource()
    {
        if (textSource == null) textSource = MonoResourcesLoader.Instance.LoadObject<TMP_Text>(floatingTextNane);
        if (textSource != null) defColor = textSource.color;
    }

    public TMP_Text GetDefultText() => textSource;
    public void ChangeTextColor(Color color)
    {
        if (textSource == null) return;
        textSource.color = color;
    }
    public void ResetTextColor()
    {
        if (textSource == null) return;
        textSource.color = defColor;
    }

    public void Setup(string showText, (Canvas canvas, RectTransform rect, Camera camera) adsObj, Vector3 vStart)
    {
        fTimer = 0.0f;
        if (textSource == null) return;
        textSource.text = showText;
        Vector3 sPos = adsObj.camera.WorldToScreenPoint(vStart);
        if ((adsObj.camera.transform.position - sPos).z < 0.01f)
        {
            gameObject.SetActive(false);
            return;
        }
        else if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        switch (adsObj.canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                transform.position = sPos;
                break;
            case RenderMode.ScreenSpaceCamera:
                {
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(adsObj.rect, sPos, adsObj.canvas.worldCamera, out Vector3 outPos);
                    transform.position = outPos;
                    break;
                }
        }
    }

    public bool UpdatePosition()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        fTimer += Time.deltaTime;
        if (fTimer > moveDuration)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }
}
