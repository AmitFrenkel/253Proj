using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloatingMenuMapScenarioManager : FloatingMenu, IPointerClickHandler
{

    public void initFloatingMapScenarioManager(MapScenarioManager mapScenarioManager)
    {
        this.mapScenarioManager = mapScenarioManager;
    }

    public void addActiveThreat()
    {

    }

    public void addActiveMapCircle()
    {
        mapScenarioManager.addNewActiveMapCircle();
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
