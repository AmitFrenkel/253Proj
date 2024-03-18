using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FloatingMenuMapCircle : FloatingMenu, IPointerClickHandler
{
    private MapActiveCircle mapActiveCircle;
    public GameObject mainFloatingMenu;
    public GameObject editFloatingMenu;
    public MapEditorDropDown linkedMapCircleIndexDropdown;

    public void initFloatingMenuMapCircle(MapScenarioManager mapScenarioManager, MapActiveCircle mapActiveCircle)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.mapActiveCircle = mapActiveCircle;
    }

    public void editMapCircle()
    {
        mainFloatingMenu.SetActive(false);
        editFloatingMenu.SetActive(true);

        linkedMapCircleIndexDropdown.initMapEditorIndputDropDown();
        linkedMapCircleIndexDropdown.buildEditorIndputDropDownBySimulatorCategory(mapScenarioManager.getSimulatorDatabase(), MainContentManager.SimulatorTypes.MapCircle, mapActiveCircle.getActiveMapCircleObject().mapCircleIndexLinkIndex);
    }

    public void duplicateMapCircle()
    {
        mapScenarioManager.duplicateActiveMapCircle(mapActiveCircle);
        mapScenarioManager.closeAllFloatingMenus();
    }

    public void removeMapCircle()
    {
        mapScenarioManager.removeActiveMapCircle(mapActiveCircle);
        mapScenarioManager.closeAllFloatingMenus();
    }

    public void linkedMapCircleChanged()
    {
        mapActiveCircle.changeLinkedMapCircle(int.Parse(linkedMapCircleIndexDropdown.getValue()));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    //public void addSTPTbefore()
    //{
    //    mapScenarioManager.addSteerPointBeforeIndex(mapAirplaneSteerPoint.getSteerPointIndex());
    //    mapAirplaneSteerPoint.closeFloatingMenu();
    //}

    //public void addSTPTafter()
    //{
    //    mapScenarioManager.addSteerPointAfterIndex(mapAirplaneSteerPoint.getSteerPointIndex());
    //    mapAirplaneSteerPoint.closeFloatingMenu();
    //}

    //public void removeSTPT()
    //{
    //    mapScenarioManager.removeSteerPointInIndex(mapAirplaneSteerPoint.getSteerPointIndex());
    //    mapAirplaneSteerPoint.closeFloatingMenu();
    //}

    //public void setAsReleaseSTPT()
    //{
    //    mapScenarioManager.setAsReleaseSteerPoint(mapAirplaneSteerPoint.getSteerPointIndex());
    //    mapAirplaneSteerPoint.closeFloatingMenu();
    //}
}
