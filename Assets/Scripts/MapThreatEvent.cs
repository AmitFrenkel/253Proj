using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MapThreatEvent : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public TextMeshProUGUI desc;
    private MapScenarioManager mapScenarioManager;
    private MapActiveThreat mapActiveThreat;
    public Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent;
    public GameObject pivotGameObject;

    private bool isDragging;

    public GameObject floatingMenuThreatEventPrefab;
    private GameObject activeFloatingMenu;

    public void initMapThreatEvent(MapScenarioManager mapScenarioManager, MapActiveThreat mapActiveThreat, Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.mapActiveThreat = mapActiveThreat;
        this.activeThreatEvent = activeThreatEvent;
        activeFloatingMenu = null;
        isDragging = false;
    }

    public void setText(string text)
    {
        desc.text = text;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        mapScenarioManager.closeAllFloatingMenus(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //this.transform.position = eventData.position;
        Vector2 dragPosition = new Vector2(eventData.position.x, eventData.position.y); // + dragOffset;
        mapActiveThreat.threatEventIsDragging(this, dragPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        mapActiveThreat.threatEventEndDrag();
        //thisImage.raycastTarget = true;
        //mapScenarioManager.airplaneSteerPointEndDrag();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
            addFloatingMenuThreat(mapScenarioManager.getMapAnchoredPosition(eventData.position));
    }

    private void addFloatingMenuThreat(Vector2 mapPosition)
    {
        mapScenarioManager.closeAllFloatingMenus(false);
        GameObject floatingMenu = Instantiate(floatingMenuThreatEventPrefab) as GameObject;
        floatingMenu.transform.parent = mapScenarioManager.transform;
        floatingMenu.GetComponent<RectTransform>().anchoredPosition = mapPosition;
        //floatingMenu.GetComponent<FloatingMenuActiveThreat>().initFloatingMenuActiveThreat(mapScenarioManager, this);
        activeFloatingMenu = floatingMenu;
    }

    public void closeFloatingMenu()
    {
        if (activeFloatingMenu != null)
        {
            Destroy(activeFloatingMenu);
            activeFloatingMenu = null;
        }
    }
}
