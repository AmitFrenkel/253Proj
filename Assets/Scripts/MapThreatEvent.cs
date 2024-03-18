using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MapThreatEvent : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public TextMeshProUGUI desc;
    private MapScenarioManager mapScenarioManager;
    public Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent;

    public void initMapThreatEvent(MapScenarioManager mapScenarioManager, Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.activeThreatEvent = activeThreatEvent;
    }

    public void setText(string text)
    {
        desc.text = text;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //this.transform.position = eventData.position;
        Vector2 dragPosition = new Vector2(eventData.position.x, eventData.position.y); // + dragOffset;
        mapScenarioManager.threatEventDragged(this, dragPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //thisImage.raycastTarget = true;
        //mapScenarioManager.airplaneSteerPointEndDrag();
    }
}
