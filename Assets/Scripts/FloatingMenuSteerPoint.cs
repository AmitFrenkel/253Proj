using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class FloatingMenuSteerPoint : FloatingMenu
{
    private MapAirplaneSteerPoint mapAirplaneSteerPoint;
    public Button makeReleaseSTPTButton;

    public void initFloatingMenuSteerPoint(MapView mapView, MapAirplaneSteerPoint mapAirplaneSteerPoint)
    {
        this.mapView = mapView;
        this.mapAirplaneSteerPoint = mapAirplaneSteerPoint;
        if (mapAirplaneSteerPoint.getSteerPointIndex() == 0)
            makeReleaseSTPTButton.interactable = false;
    }

    public void addSTPTbefore()
    {
        mapView.addSteerPointBeforeIndex(mapAirplaneSteerPoint.getSteerPointIndex());
        mapAirplaneSteerPoint.closeFloatingMenu();
    }

    public void addSTPTafter()
    {
        mapView.addSteerPointAfterIndex(mapAirplaneSteerPoint.getSteerPointIndex());
        mapAirplaneSteerPoint.closeFloatingMenu();
    }

    public void removeSTPT()
    {
        mapView.removeSteerPointInIndex(mapAirplaneSteerPoint.getSteerPointIndex());
        mapAirplaneSteerPoint.closeFloatingMenu();
    }

    public void setAsReleaseSTPT()
    {
        mapView.setAsReleaseSteerPoint(mapAirplaneSteerPoint.getSteerPointIndex());
        mapAirplaneSteerPoint.closeFloatingMenu();
    }
}
