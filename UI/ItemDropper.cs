using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class ItemDropper : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Transform sender = eventData.pointerDrag.transform;
        int childCnt = transform.childCount;
        if (childCnt > 0)
        {
            //first switch, else destroy.
            for (int i = childCnt -1; i >= 0; i--)
            {
                Transform chItem = transform.GetChild(i);
                if (i == 0)
                {
                    ItemDragger _dragger = sender.GetComponent<ItemDragger>();
                    chItem.SetParent(_dragger.GetOriginalParent());
                    chItem.localPosition = Vector3.zero;
                }
                else Destroy(chItem.gameObject);
            }
        }
        sender.SetParent(transform);
    }
}
