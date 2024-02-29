using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainVerticalPanel : MonoBehaviour
{
    public MainUIManager mainUIManager;
    
    protected float maxXAnchor = 1f;
    protected float minXAnchor = 0f;
    protected RectTransform rectTransform;
    public GameObject contectHolder;
    

    public void initMainVerticalPanel()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
    }

    public void setXAnchorMinAndMax(float minXAnchor, float maxXAnchor)
    {
        rectTransform.anchorMin = new Vector2(minXAnchor, 0f);
        rectTransform.anchorMax = new Vector2(maxXAnchor, 1f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
