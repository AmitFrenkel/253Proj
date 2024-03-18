using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class FloatingMenuSteerPoint : FloatingMenu
{
    private MapAirplaneSteerPoint mapAirplaneSteerPoint;
    public Button makeReleaseSTPTButton;

    public void initFloatingMenuSteerPoint(MapScenarioManager mapScenarioManager, MapAirplaneSteerPoint mapAirplaneSteerPoint)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.mapAirplaneSteerPoint = mapAirplaneSteerPoint;
        if (mapAirplaneSteerPoint.getSteerPointIndex() == 0)
            makeReleaseSTPTButton.interactable = false;
    }

    public void addSTPTbefore()
    {
        mapScenarioManager.addSteerPointBeforeIndex(mapAirplaneSteerPoint.getSteerPointIndex());
        mapAirplaneSteerPoint.closeFloatingMenu();
    }

    public void addSTPTafter()
    {
        mapScenarioManager.addSteerPointAfterIndex(mapAirplaneSteerPoint.getSteerPointIndex());
        mapAirplaneSteerPoint.closeFloatingMenu();
    }

    public void removeSTPT()
    {
        mapScenarioManager.removeSteerPointInIndex(mapAirplaneSteerPoint.getSteerPointIndex());
        mapAirplaneSteerPoint.closeFloatingMenu();
    }

    public void setAsReleaseSTPT()
    {
        mapScenarioManager.setAsReleaseSteerPoint(mapAirplaneSteerPoint.getSteerPointIndex());
        mapAirplaneSteerPoint.closeFloatingMenu();
    }
}
