using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapView
{

    private string MAP_TOP_STEER_POINT;
    private string MAP_BOTTOM_STEER_POINT;
    private string MAP_LEFT_STEER_POINT;
    private string MAP_RIGHT_STEER_POINT;
    private float mapSize;
    private float milesPerLengthUnit;

    public MapView(Image mapImage, string MAP_TOP_STEER_POINT, string MAP_BOTTOM_STEER_POINT, string MAP_LEFT_STEER_POINT, string MAP_RIGHT_STEER_POINT)
    {
        Vector2 mapSize2D = mapImage.transform.GetComponent<RectTransform>().sizeDelta;
        if (mapSize2D.x != mapSize2D.y)
            Debug.Log("map size must by equal for X and Y!");
        mapSize = mapSize2D.x;
        this.MAP_TOP_STEER_POINT = MAP_TOP_STEER_POINT;
        this.MAP_BOTTOM_STEER_POINT = MAP_BOTTOM_STEER_POINT;
        this.MAP_LEFT_STEER_POINT = MAP_LEFT_STEER_POINT;
        this.MAP_RIGHT_STEER_POINT = MAP_RIGHT_STEER_POINT;

        // Calculate milesPerLengthUnit
        float minContinuesSteerPoint = ToContinousSteerPoint(MAP_BOTTOM_STEER_POINT);
        float maxContinuesSteerPoint = ToContinousSteerPoint(MAP_TOP_STEER_POINT);
        milesPerLengthUnit = (maxContinuesSteerPoint - minContinuesSteerPoint) * 60f / mapSize;
    }

    public float getMilesPerLengthUnit()
    {
        return milesPerLengthUnit;
    }

    // ====================== SteerPoint Conversions ==========================

    // ----------------------- From string STPT to map coordiantes ------------

    public Vector2 FromScenarioSteerPointToAnchoredPosition(Scenario.SteerPoint steerPoint)
    {
        return new Vector2(ToXmapPos(steerPoint.E), ToYmapPos(steerPoint.N));
    }

    public float ToContinousSteerPoint(string stp)
    {
        float deg = float.Parse(stp.Substring(0, 2));
        float min = float.Parse(stp.Substring(2, 2)) + (float.Parse(stp.Substring(5, 3))/1000f);
        return deg + min/60f;
    }

    public float SteerPointToMapPosition(string stpPoint, string stpMax, string stpMin, float mapLength)
    {
        return ((ToContinousSteerPoint(stpPoint) - ToContinousSteerPoint(stpMin))/ (ToContinousSteerPoint(stpMax) - ToContinousSteerPoint(stpMin))) * mapLength;
    }

    public float ToXmapPos(string stpPointEast)
    {
        return SteerPointToMapPosition(stpPointEast, MAP_RIGHT_STEER_POINT, MAP_LEFT_STEER_POINT, mapSize);
    }

    public float ToYmapPos(string stpPointNorth)
    {
        return SteerPointToMapPosition(stpPointNorth, MAP_TOP_STEER_POINT, MAP_BOTTOM_STEER_POINT, mapSize);
    }

    // ----------------------- From map coordiantes to string STPT ------------

    public Scenario.SteerPoint FromAnchoredPositionToSteerPoint(Vector2 anchoredPosition)
    {
        string N = ToStringSTPT(anchoredPosition.y, "N", mapSize);
        string E = ToStringSTPT(anchoredPosition.x, "E", mapSize);
        return new Scenario.SteerPoint(N, E);
    }

    public string ToStringSTPT(float val, string axe, float mapLength)
    {
        float relativePos = val / mapLength;
        float maxContinousSteerPoint = ToContinousSteerPoint(axe == "N" ? MAP_TOP_STEER_POINT : MAP_RIGHT_STEER_POINT);
        float minContinousSteerPoint = ToContinousSteerPoint(axe == "N" ? MAP_BOTTOM_STEER_POINT : MAP_LEFT_STEER_POINT);
        float continousSteerPoint = minContinousSteerPoint + (maxContinousSteerPoint - minContinousSteerPoint) * relativePos;
        return FromContinousToStringFormatSteerPoint(continousSteerPoint);
    }

    public string FromContinousToStringFormatSteerPoint(float contSteerPoint)
    {
        float fullPart = Mathf.Floor(contSteerPoint);
        float notFullPart = contSteerPoint - fullPart;
        float minStpt = notFullPart * 60f;
        return Mathf.RoundToInt(fullPart).ToString() + (minStpt < 10 ? "0" : "") + minStpt.ToString("0.000");
    }



}
