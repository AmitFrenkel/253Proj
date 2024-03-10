using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    public const string MAP_TOP_STEER_POINT = "3430.000";
    public const string MAP_BOTTOM_STEER_POINT = "3300.000";
    public const string MAP_LEFT_STEER_POINT = "3500.000";
    public const string MAP_RIGHT_STEER_POINT = "3630.000";
    public const float mapSize = 800f;

    public GameObject mapAirplaneSteerPointPrefab;

    private List<GameObject> airplaneSteerPoints;
    public Transform mapAirplaneTargetSymbol;
    public MainContentManager mainContentManager;

    public void initMapViewByScenario(Scenario scenario)
    {
        airplaneSteerPoints = new List<GameObject>();
        foreach (Scenario.SteerPoint loopAirplaneSteerPoint in scenario.airplaneSteerPoints)
        {
            GameObject newMapSteerPoint = Instantiate(mapAirplaneSteerPointPrefab) as GameObject;
            newMapSteerPoint.transform.parent = this.transform;
            newMapSteerPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(ToXmapPos(loopAirplaneSteerPoint.E), ToYmapPos(loopAirplaneSteerPoint.N));
            newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>().initMapAirplaneSteerPoint(this);
            airplaneSteerPoints.Add(newMapSteerPoint);
        }
        airplaneSteerPointDragged();
    }

    public void airplaneSteerPointDragged()
    {
        for (int i = 0; i < airplaneSteerPoints.Count - 1; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().makeALine(airplaneSteerPoints[i + 1].transform);
        mapAirplaneTargetSymbol.position = airplaneSteerPoints[1].transform.position;
        mapAirplaneTargetSymbol.transform.eulerAngles = new Vector3(0f, 0f, airplaneSteerPoints[0].GetComponent<MapAirplaneSteerPoint>().getZRotation());
    }

    public void airplaneSteerPointEndDrag()
    {
        mainContentManager.setMapViewModified();
    }

    public void saveChangesFromMapViewToDatabase()
    {

    }

    // ====================== SteerPoint Conversions ==========================

    public static float ToContinousSteerPoint(string stp)
    {
        float deg = float.Parse(stp.Substring(0, 2));
        float min = float.Parse(stp.Substring(2, 2)) + (float.Parse(stp.Substring(5, 3))/1000f);
        return deg + min/60f;
    }

    public static float SteerPointToMapPosition(string stpPoint, string stpMax, string stpMin, float mapLength)
    {
        return ((ToContinousSteerPoint(stpPoint) - ToContinousSteerPoint(stpMin))/ (ToContinousSteerPoint(stpMax) - ToContinousSteerPoint(stpMin))) * mapLength;
    }

    public static float ToXmapPos(string stpPointEast)
    {
        return SteerPointToMapPosition(stpPointEast, MAP_RIGHT_STEER_POINT, MAP_LEFT_STEER_POINT, mapSize);
    }

    public static float ToYmapPos(string stpPointNorth)
    {
        return SteerPointToMapPosition(stpPointNorth, MAP_TOP_STEER_POINT, MAP_BOTTOM_STEER_POINT, mapSize);
    }

    
}
