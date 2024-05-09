using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseFloatingBar : MonoBehaviour
{
    [SerializeField] Image barImage;
    [SerializeField] string floatingBarNane = "FloatingBar";
    [SerializeField] float floatHeight = 2.0f;
    private Transform followTarget;

    void Start()
    {
        LoadingSource();
    }

    void LoadingSource()
    {
        if (barImage == null) barImage = MonoResourcesLoader.Instance.LoadObject<Image>(floatingBarNane);
    }
    public Transform GetFollowTarget() => followTarget;
    public Image GetDefultBar() => barImage;

    public void SetFollowTarget(Transform _target) { followTarget = _target; }
    public void ReSetImageSimple(Sprite background = default, Sprite foreground = default)
    {
        if (barImage == null) LoadingSource();
        else
        {
            if (background != default) barImage.sprite = background;
            Image _child = barImage.GetComponentInChildren<Image>();
            if (_child != null && foreground != default)
                _child.sprite = foreground;
        }
    }
    public void EditFloatHeight(float height) { floatHeight = height; }

    public void UpdateBarToFollowTarget((Canvas canvas, RectTransform rect, Camera camera) adsObj)
    {
        Vector3 vTarget = followTarget.position + Vector3.up * floatHeight;
        UpdateBarPosition(adsObj, vTarget);
    }
    public void UpdateBarPosition((Canvas canvas, RectTransform rect, Camera camera) adsObj, Vector3 vTarget)
    {
        Vector3 sPos = adsObj.camera.WorldToScreenPoint(vTarget);
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
}
