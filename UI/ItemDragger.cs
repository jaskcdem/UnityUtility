using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDragger : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //ui panel setting
    private Canvas _mainCanvas;
    private RectTransform _mainCanvasTransform;

    private Transform _originalParent;
    private Image _image;
    private Vector3 _dragDeltaVector;

    public Transform GetOriginalParent() => _originalParent;

    void Start()
    {
        _mainCanvas = MonoUIManager.Instance.mainCanvas;
        _mainCanvasTransform = MonoUIManager.Instance.GetCanvasTransform();
        _image = GetComponent<Image>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;
        _dragDeltaVector = Input.mousePosition;
        if (_mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            _dragDeltaVector = Input.mousePosition - transform.position;
        else
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_mainCanvasTransform, Input.mousePosition, _mainCanvas.worldCamera, out Vector3 vOut);
            _dragDeltaVector = vOut - transform.position;
        }
        _originalParent = transform.parent;
        transform.SetParent(_mainCanvasTransform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            transform.position = Input.mousePosition - _dragDeltaVector;
        else
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_mainCanvasTransform, Input.mousePosition, _mainCanvas.worldCamera, out Vector3 vOut);
            transform.position = vOut - _dragDeltaVector;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.raycastTarget = true;
        if (transform.parent == _mainCanvasTransform)
            transform.SetParent(_originalParent);
        transform.localPosition = Vector3.zero;
        _originalParent = null;
    }
}
