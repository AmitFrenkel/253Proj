using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapScenarioManager : MonoBehaviour, IPointerClickHandler
{
    private MainContentManager mainContentManager;
    private Scenario scenario;
    public MapView mapView;
    private FlightPath flightPath;

    //======= Map Objetcs / elements Prefabs =======
    public GameObject mapAirplaneSteerPoint;
    //public GameObject threatEventPrefab;
    public GameObject mapActiveThreatPrefab;
    public GameObject flightPathPointPrefab;
    //public GameObject mapThreatOverPathPrefab;
    public GameObject mapActiveCirclePrefab;
    // =============================================


    //======= Map Objetcs / elements =======
    private List<MapAirplaneSteerPoint> airplaneSteerPoints;
    public Transform mapAirplaneTargetSymbol;
    private List<GameObject> flightPathPoints;
    private List<MapThreatEvent> threatEventLines;
    private List<MapActiveCircle> mapActiveCircles;
    private List<MapActiveThreat> mapActiveThreats;
    // =====================================

    public GameObject pointDraggedOnMouse;
    private Vector3 lastClickPosition;

    private float flightVelocity;

    public GameObject floatingMenuMapViewPrefab;
    private GameObject activeFloatingMenu;

    private const float distToNewSteerPointInEdgesMiles = 5f;
    private const float distToDuplicatedElement = 50f;

    public void initMapScenarioManagerByScenario(Scenario scenario, MainContentManager mainContentManager)
    {
        this.scenario = scenario;
        this.mainContentManager = mainContentManager;
        mapView = new MapView(this.transform.GetComponent<Image>(), "3430.000", "3300.000", "3500.000", "3630.000");
        activeFloatingMenu = null;

        flightPathPoints = new List<GameObject>();
        threatEventLines = new List<MapThreatEvent>();

        flightVelocity = 500f;

        airplaneSteerPoints = new List<MapAirplaneSteerPoint>();
        for (int i = 0; i < scenario.airplaneSteerPoints.Length; i++)
        {
            Scenario.SteerPoint loopAirplaneSteerPoint = scenario.airplaneSteerPoints[i];
            GameObject newMapSteerPoint = Instantiate(mapAirplaneSteerPoint) as GameObject;
            newMapSteerPoint.transform.parent = this.transform;
            newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>().initMapAirplaneSteerPoint(this, loopAirplaneSteerPoint, i);
            airplaneSteerPoints.Add(newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>());
        }
        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();

        mapActiveCircles = new List<MapActiveCircle>();
        foreach (Scenario.ActiveMapCircle activeMapCircle in scenario.activeMapCircles)
        {
            GameObject newMapActiveCircle = Instantiate(mapActiveCirclePrefab) as GameObject;
            newMapActiveCircle.transform.parent = this.transform;
            newMapActiveCircle.GetComponent<MapActiveCircle>().initMapActiveCircle(this, activeMapCircle);
            mapActiveCircles.Add(newMapActiveCircle.GetComponent<MapActiveCircle>());
        }

        mapActiveThreats = new List<MapActiveThreat>();
        foreach (Scenario.ActiveThreat activeThreat in scenario.activeThreats)
        {
            GameObject newMapActiveThreat = Instantiate(mapActiveThreatPrefab) as GameObject;
            newMapActiveThreat.transform.parent = this.transform;
            newMapActiveThreat.GetComponent<MapActiveThreat>().initMapActiveThreat(this, activeThreat);
            mapActiveThreats.Add(newMapActiveThreat.GetComponent<MapActiveThreat>());
        }

    }

    public void addSteerPointBeforeIndex(int steerPointIndex)
    {
        GameObject newMapSteerPoint = Instantiate(mapAirplaneSteerPoint) as GameObject;
        newMapSteerPoint.transform.parent = this.transform;

        Vector2 newPosition = Vector2.zero;
        if (steerPointIndex == 0)
            newPosition = airplaneSteerPoints[0].GetComponent<RectTransform>().anchoredPosition + (airplaneSteerPoints[0].GetComponent<RectTransform>().anchoredPosition - airplaneSteerPoints[1].GetComponent<RectTransform>().anchoredPosition).normalized * (distToNewSteerPointInEdgesMiles / mapView.getMilesPerLengthUnit());
        else
            newPosition = (airplaneSteerPoints[steerPointIndex].GetComponent<RectTransform>().anchoredPosition + airplaneSteerPoints[steerPointIndex - 1].GetComponent<RectTransform>().anchoredPosition) / 2f;
        //newMapSteerPoint.GetComponent<RectTransform>().anchoredPosition = newPosition;
        Scenario.SteerPoint newSteerPoint = mapView.FromAnchoredPositionToSteerPoint(newPosition);
        newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>().initMapAirplaneSteerPoint(this, newSteerPoint, steerPointIndex);
        airplaneSteerPoints.Insert(steerPointIndex, newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>());

        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().setSteerPointIndex(i);

        Array.Resize(ref scenario.airplaneSteerPoints, airplaneSteerPoints.Count);
        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            scenario.airplaneSteerPoints[i] = airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().getSteerPointObject();

        if (steerPointIndex <= scenario.airPlaneSteerPointOfRelease)
            scenario.airPlaneSteerPointOfRelease++;

        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();
    }

    public void addSteerPointAfterIndex(int steerPointIndex)
    {
        GameObject newMapSteerPoint = Instantiate(mapAirplaneSteerPoint) as GameObject;
        newMapSteerPoint.transform.parent = this.transform;

        Vector2 newPosition = Vector2.zero;
        if (steerPointIndex == airplaneSteerPoints.Count - 1)
            newPosition = airplaneSteerPoints[airplaneSteerPoints.Count - 1].GetComponent<RectTransform>().anchoredPosition + (airplaneSteerPoints[airplaneSteerPoints.Count - 1].GetComponent<RectTransform>().anchoredPosition - airplaneSteerPoints[airplaneSteerPoints.Count - 2].GetComponent<RectTransform>().anchoredPosition).normalized * (distToNewSteerPointInEdgesMiles / mapView.getMilesPerLengthUnit());
        else
            newPosition = (airplaneSteerPoints[steerPointIndex].GetComponent<RectTransform>().anchoredPosition + airplaneSteerPoints[steerPointIndex + 1].GetComponent<RectTransform>().anchoredPosition) / 2f;
        //newMapSteerPoint.GetComponent<RectTransform>().anchoredPosition = newPosition;
        Scenario.SteerPoint newSteerPoint = mapView.FromAnchoredPositionToSteerPoint(newPosition);
        newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>().initMapAirplaneSteerPoint(this, newSteerPoint, steerPointIndex);
        airplaneSteerPoints.Insert(steerPointIndex + 1, newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>());

        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().setSteerPointIndex(i);

        Array.Resize(ref scenario.airplaneSteerPoints, airplaneSteerPoints.Count);
        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            scenario.airplaneSteerPoints[i] = airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().getSteerPointObject();

        if (steerPointIndex < scenario.airPlaneSteerPointOfRelease)
            scenario.airPlaneSteerPointOfRelease++;

        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();
    }

    public void removeSteerPointInIndex(int steerPointIndex)
    {

        Destroy(airplaneSteerPoints[steerPointIndex].gameObject);
        airplaneSteerPoints.RemoveAt(steerPointIndex);
        RemoveAt(ref scenario.airplaneSteerPoints, steerPointIndex);

        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().setSteerPointIndex(i);

        if (steerPointIndex <= scenario.airPlaneSteerPointOfRelease)
            scenario.airPlaneSteerPointOfRelease = Mathf.Max(scenario.airPlaneSteerPointOfRelease - 1, 1);


        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();

    }

    public void setAsReleaseSteerPoint(int steerPointIndex)
    {
        scenario.airPlaneSteerPointOfRelease = steerPointIndex;
        airplaneSteerPointDragged();
    }


    public void airplaneSteerPointDragged()
    {
        for (int i = 0; i < airplaneSteerPoints.Count - 1; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().makeNavLine(airplaneSteerPoints[i + 1].transform);
        airplaneSteerPoints[airplaneSteerPoints.Count - 1].GetComponent<MapAirplaneSteerPoint>().hideNavLine();
        mapAirplaneTargetSymbol.position = airplaneSteerPoints[scenario.airPlaneSteerPointOfRelease].transform.position;
        mapAirplaneTargetSymbol.transform.eulerAngles = new Vector3(0f, 0f, airplaneSteerPoints[scenario.airPlaneSteerPointOfRelease - 1].GetComponent<MapAirplaneSteerPoint>().getZRotation());
    }

    public void airplaneSteerPointEndDrag(int steerPointIndex, Scenario.SteerPoint newSteerPoint)
    {
        scenario.airplaneSteerPoints[steerPointIndex] = newSteerPoint;
        updateMapViewAfterSteerPointChanged();
        mainContentManager.setMapScenarioManagerModified();
    }

    private void updateMapViewAfterSteerPointChanged()
    {
        flightPath = new FlightPath(mapView);
        flightPath.buildNavSectionsByAirplaneNav(scenario.airplaneSteerPoints, mapView.getMilesPerLengthUnit());
        foreach (GameObject loopFlightPathPoint in flightPathPoints)
            Destroy(loopFlightPathPoint);
        flightPathPoints.Clear();
        float spacing = 0.5f;
        float loopDist = 0f;
        float totalLength = flightPath.getLengthOfFlightPathInMiles();
        while (loopDist < totalLength)
        {
            GameObject newPoint = Instantiate(flightPathPointPrefab) as GameObject;
            newPoint.transform.parent = this.transform;
            newPoint.GetComponent<RectTransform>().anchoredPosition = flightPath.getPosInMilesDist(loopDist);
            flightPathPoints.Add(newPoint);
            loopDist += spacing;
        }

        foreach (MapThreatEvent loopThreatEventLine in threatEventLines)
            Destroy(loopThreatEventLine.gameObject);
        threatEventLines.Clear();
        foreach (Scenario.ActiveThreat activeThreat in scenario.activeThreats)
        {
            Threat threat = mainContentManager.getSimulatorDatabase().getThreatByIndex(activeThreat.activeThreatLinkIndex);
            string threatName = threat.threatName;
            foreach (Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent in activeThreat.activeThreatEvents)
            {
                //GameObject threatEventGameObject = Instantiate(threatEventPrefab) as GameObject;
                //threatEventGameObject.transform.parent = this.transform;
                //threatEventGameObject.GetComponent<MapThreatEvent>().initMapThreatEvent(this, activeThreatEvent);

                Threat.ThreatLock threatLock = threat.threatLocks[activeThreatEvent.threatLockListIndex];
                string threatLockName = threatLock.threatLockName;
                float activeThreatEventDist = activeThreatEvent.threatEventDistance;
                Vector2 activeThreatEventPos = flightPath.getPosInMilesDist(activeThreatEventDist);
                Vector2 activeThreatEventPointAtPos = flightPath.getPosInMilesDist(activeThreatEventDist + 0.1f);

                //threatEventGameObject.GetComponent<RectTransform>().anchoredPosition = activeThreatEventPos;

                float diffy = activeThreatEventPointAtPos.y - activeThreatEventPos.y;
                float diffx = activeThreatEventPointAtPos.x - activeThreatEventPos.x;
                float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
                //threatEventGameObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);

                //threatEventGameObject.GetComponent<MapThreatEvent>().setText(threatName + " " + threatLockName);
                //threatEventLines.Add(threatEventGameObject.GetComponent<MapThreatEvent>());

            }

            //GameObject mapThreatOverPath = Instantiate(mapThreatOverPathPrefab) as GameObject;
            //mapThreatOverPath.transform.parent = this.transform;
            //mapThreatOverPath.GetComponent<MapThreatOverPath>().initMesh(this, activeThreat);
        }
    }

    public void threatEventDragged(MapThreatEvent threatEvent, Vector2 position)
    {

        Vector2 anchoredPosition = getMapAnchoredPosition(position);
        float newDist = flightPath.getDistOfClosestPointOnPath(anchoredPosition);

        Vector2 activeThreatEventPos = flightPath.getPosInMilesDist(newDist);
        Vector2 activeThreatEventPointAtPos = flightPath.getPosInMilesDist(newDist + 0.1f);
        if (activeThreatEventPos == activeThreatEventPointAtPos)
            activeThreatEventPointAtPos = 2f * activeThreatEventPos - flightPath.getPosInMilesDist(newDist - 0.1f);

        threatEvent.gameObject.GetComponent<RectTransform>().anchoredPosition = activeThreatEventPos;

        float diffy = activeThreatEventPointAtPos.y - activeThreatEventPos.y;
        float diffx = activeThreatEventPointAtPos.x - activeThreatEventPos.x;
        float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
        threatEvent.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);

        threatEvent.activeThreatEvent.threatEventDistance = newDist;

    }

    // =================== Map Circles ====================

    public void addNewActiveMapCircle()
    {
        closeFloatingMenu();
        Vector2 anchoredPosition = getMapAnchoredPosition(lastClickPosition);
        Scenario.SteerPoint newSteerPoint = mapView.FromAnchoredPositionToSteerPoint(anchoredPosition);
        Scenario.ActiveMapCircle activeMapCircle = new Scenario.ActiveMapCircle(0, newSteerPoint);
        GameObject newMapActiveCircle = Instantiate(mapActiveCirclePrefab) as GameObject;
        newMapActiveCircle.transform.parent = this.transform;
        newMapActiveCircle.GetComponent<MapActiveCircle>().initMapActiveCircle(this, activeMapCircle);
        mapActiveCircles.Add(newMapActiveCircle.GetComponent<MapActiveCircle>());

        Array.Resize(ref scenario.activeMapCircles, scenario.activeMapCircles.Length + 1);
        scenario.activeMapCircles[scenario.activeMapCircles.Length - 1] = activeMapCircle;
    }

    public void duplicateActiveMapCircle(MapActiveCircle mapActiveCircleToClone)
    {
        closeFloatingMenu();

        Scenario.ActiveMapCircle clonedObject = mapActiveCircleToClone.getActiveMapCircleObject();

        Vector2 anchoredPosition = mapActiveCircleToClone.transform.GetComponent<RectTransform>().anchoredPosition + new Vector2(distToDuplicatedElement, 0f);
        Scenario.SteerPoint newSteerPoint = mapView.FromAnchoredPositionToSteerPoint(anchoredPosition);
        Scenario.ActiveMapCircle activeMapCircle = new Scenario.ActiveMapCircle(clonedObject.mapCircleIndexLinkIndex, newSteerPoint);
        GameObject newMapActiveCircle = Instantiate(mapActiveCirclePrefab) as GameObject;
        newMapActiveCircle.transform.parent = this.transform;
        newMapActiveCircle.GetComponent<MapActiveCircle>().initMapActiveCircle(this, activeMapCircle);
        mapActiveCircles.Add(newMapActiveCircle.GetComponent<MapActiveCircle>());

        Array.Resize(ref scenario.activeMapCircles, scenario.activeMapCircles.Length + 1);
        scenario.activeMapCircles[scenario.activeMapCircles.Length - 1] = activeMapCircle;
    }

    public void removeActiveMapCircle(MapActiveCircle mapActiveCircleToRemove)
    {
        closeAllFloatingMenus();
        mapActiveCircles.Remove(mapActiveCircleToRemove);
        for (int indexInMapCirclesArray = 0; indexInMapCirclesArray < scenario.activeMapCircles.Length; indexInMapCirclesArray++)
            if (scenario.activeMapCircles[indexInMapCirclesArray] == mapActiveCircleToRemove.getActiveMapCircleObject())
            {
                RemoveAt(ref scenario.activeMapCircles, indexInMapCirclesArray);
                break;
            }
        Destroy(mapActiveCircleToRemove.gameObject);

    }

    // =================== General Functions ====================

    public void saveChangesFromMapScenarioManagerToDatabase()
    {

    }

    public FlightPath getFlightPath()
    {
        return flightPath;
    }
    

    public Vector2 getMapAnchoredPosition(Vector3 position)
    {
        pointDraggedOnMouse.GetComponent<RectTransform>().position = position;

        return pointDraggedOnMouse.GetComponent<RectTransform>().anchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        addFloatingMenuMapView(getMapAnchoredPosition(eventData.position));
        lastClickPosition = eventData.position;
    }

    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            arr[a] = arr[a + 1];
        }
        Array.Resize(ref arr, arr.Length - 1);
    }

    public void closeAllFloatingMenus()
    {
        closeFloatingMenu();
        foreach (MapAirplaneSteerPoint airplaneSteerPoint in airplaneSteerPoints)
            airplaneSteerPoint.closeFloatingMenu();
        foreach (MapActiveCircle mapActiveCircle in mapActiveCircles)
            mapActiveCircle.closeFloatingMenu();
        foreach (MapActiveThreat mapActiveThreat in mapActiveThreats)
            mapActiveThreat.closeFloatingMenu();

    }

    private void addFloatingMenuMapView(Vector2 mapPosition)
    {
        closeAllFloatingMenus();
        GameObject floatingMenu = Instantiate(floatingMenuMapViewPrefab) as GameObject;
        floatingMenu.transform.parent = this.transform;
        floatingMenu.GetComponent<RectTransform>().anchoredPosition = mapPosition;
        floatingMenu.GetComponent<FloatingMenuMapScenarioManager>().initFloatingMapScenarioManager(this);
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

    public SimulatorDatabase getSimulatorDatabase()
    {
        return mainContentManager.getSimulatorDatabase();
    }
}
