using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FloatingMenuThreatEvent : FloatingMenu, IPointerClickHandler
{
    private MapThreatEvent mapThreatEvent;
    public GameObject mainFloatingMenu;
    public GameObject editFloatingMenu;
    public MapEditorDropDown linkedThreatIndexDropdown;

    public void initFloatingMenuActiveThreat(MapScenarioManager mapScenarioManager, MapThreatEvent mapThreatEvent)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.mapThreatEvent = mapThreatEvent;
    }

    public void editValues()
    {
        mainFloatingMenu.SetActive(false);
        editFloatingMenu.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
