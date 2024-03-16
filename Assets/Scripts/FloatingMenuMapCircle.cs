using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class FloatingMenuMapCircle : FloatingMenu
{
    private MapActiveCircle mapActiveCircle;
    //public Button makeReleaseSTPTButton;

    public void initFloatingMenuMapCircle(MapView mapView, MapActiveCircle mapActiveCircle)
    {
        this.mapView = mapView;
        this.mapActiveCircle = mapActiveCircle;
    }

    //public void addSTPTbefore()
    //{
    //    mapView.addSteerPointBeforeIndex(mapAirplaneSteerPoint.getSteerPointIndex());
    //    mapAirplaneSteerPoint.closeFloatingMenu();
    //}

    //public void addSTPTafter()
    //{
    //    mapView.addSteerPointAfterIndex(mapAirplaneSteerPoint.getSteerPointIndex());
    //    mapAirplaneSteerPoint.closeFloatingMenu();
    //}

    //public void removeSTPT()
    //{
    //    mapView.removeSteerPointInIndex(mapAirplaneSteerPoint.getSteerPointIndex());
    //    mapAirplaneSteerPoint.closeFloatingMenu();
    //}

    //public void setAsReleaseSTPT()
    //{
    //    mapView.setAsReleaseSteerPoint(mapAirplaneSteerPoint.getSteerPointIndex());
    //    mapAirplaneSteerPoint.closeFloatingMenu();
    //}
}
