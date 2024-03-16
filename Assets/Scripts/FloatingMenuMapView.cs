using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMenuMapView : FloatingMenu
{

    public void initFloatingMapView(MapView mapView)
    {
        this.mapView = mapView;
    }

    public void addActiveThreat()
    {

    }

    public void addActiveMapCircle()
    {
        mapView.addNewActiveMapCircle();
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
