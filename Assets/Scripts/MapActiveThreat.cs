using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MapActiveThreat : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{

    private MapScenarioManager mapScenarioManager;
    private Scenario.ActiveThreat activeThreat;
    private Threat threat;
    private FlightPath flightPath;

    public GameObject outerCircle;
    public TextMeshProUGUI activeThreatName;

    public GameObject floatingMenuActiveThreatPrefab;
    private GameObject activeFloatingMenu;

    public GameObject threatEventPrefab;
    public GameObject mapThreatOverPathPrefab;

    private List<MapThreatOverPath> mapThreatsOverPath;

    private List<MapThreatEvent> mapThreatEvents;

    private bool isDragging;
    private bool isEditingThreatEvents;

    public void initMapActiveThreat(MapScenarioManager mapScenarioManager, Scenario.ActiveThreat activeThreat)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.activeThreat = activeThreat;
        activeFloatingMenu = null;
        isDragging = false;
        isEditingThreatEvents = false;

        this.transform.GetComponent<RectTransform>().anchoredPosition = mapScenarioManager.mapView.FromScenarioSteerPointToAnchoredPosition(activeThreat.threatPosition);

        threat = mapScenarioManager.getSimulatorDatabase().getThreatByIndex(activeThreat.activeThreatLinkIndex);
        activeThreatName.text = threat.threatName;
        float outerCircleSize = 2f * threat.threatRadius / mapScenarioManager.mapView.getMilesPerLengthUnit();
        outerCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(outerCircleSize, outerCircleSize);

        initActiveThreatOverPath();
    }

    private void initActiveThreatOverPath()
    {
        mapThreatsOverPath = new List<MapThreatOverPath>();
        for (int i=0; i<activeThreat.activeThreatEvents.Length-1; i++)
        {
            Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent = activeThreat.activeThreatEvents[i];
            float firstDist = activeThreat.activeThreatEvents[i].threatEventDistance;
            float lastDist = activeThreat.activeThreatEvents[i+1].threatEventDistance;

            string threatEventName = threat.threatLocks[activeThreatEvent.threatLockListIndex].threatLockName;
            GameObject mapThreatOverPathGameObject = Instantiate(mapThreatOverPathPrefab) as GameObject;
            mapThreatOverPathGameObject.transform.parent = mapScenarioManager.transform;
            mapThreatOverPathGameObject.GetComponent<MapThreatOverPath>().initMesh(mapScenarioManager, firstDist, lastDist, Color.blue, !threatEventName.ToLower().Contains("search"));
            mapThreatsOverPath.Add(mapThreatOverPathGameObject.GetComponent<MapThreatOverPath>());
        }
    }

    public void updateActiveThreatOverPathAfterFlightPathChanged()
    {
        foreach (MapThreatOverPath mapThreatOverPath in mapThreatsOverPath)
            mapThreatOverPath.buildMesh();
    }

    public void editThreatEvents()
    {
        isEditingThreatEvents = true;
        mapScenarioManager.closeAllFloatingMenus();

        flightPath = mapScenarioManager.getFlightPath();
        mapThreatEvents = new List<MapThreatEvent>();
        foreach (Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent in activeThreat.activeThreatEvents)
        {
            GameObject threatEventGameObject = Instantiate(threatEventPrefab) as GameObject;
            threatEventGameObject.transform.parent = mapScenarioManager.transform;
            threatEventGameObject.GetComponent<MapThreatEvent>().initMapThreatEvent(mapScenarioManager, this, activeThreatEvent);

            
            float activeThreatEventDist = activeThreatEvent.threatEventDistance;
            Vector2 activeThreatEventPos = flightPath.getPosInMilesDist(activeThreatEventDist);
            Vector2 activeThreatEventPointAtPos = flightPath.getPosInMilesDist(activeThreatEventDist + 0.1f);

            threatEventGameObject.GetComponent<RectTransform>().anchoredPosition = activeThreatEventPos;

            float diffy = activeThreatEventPointAtPos.y - activeThreatEventPos.y;
            float diffx = activeThreatEventPointAtPos.x - activeThreatEventPos.x;
            float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
            threatEventGameObject.GetComponent<MapThreatEvent>().pivotGameObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);

            int linkIndex = activeThreatEvent.threatLockListIndex;
            if (linkIndex != -1)
            {
                Threat.ThreatLock threatLock = threat.threatLocks[activeThreatEvent.threatLockListIndex];
                string threatLockName = threatLock.threatLockName;
                threatEventGameObject.GetComponent<MapThreatEvent>().setText(threat.threatName + " " + threatLockName);
            }
            else
            {
                threatEventGameObject.GetComponent<MapThreatEvent>().setText(threat.threatName + " HIDE");
            }
            mapThreatEvents.Add(threatEventGameObject.GetComponent<MapThreatEvent>());

        }
    }

    public void threatEventIsDragging(MapThreatEvent threatEvent, Vector2 position)
    {

        Vector2 anchoredPosition = mapScenarioManager.getMapAnchoredPosition(position);
        float newDist = flightPath.getDistOfClosestPointOnPath(anchoredPosition);

        Vector2 activeThreatEventPos = flightPath.getPosInMilesDist(newDist);
        Vector2 activeThreatEventPointAtPos = flightPath.getPosInMilesDist(newDist + 0.1f);
        if (activeThreatEventPos == activeThreatEventPointAtPos)
            activeThreatEventPointAtPos = 2f * activeThreatEventPos - flightPath.getPosInMilesDist(newDist - 0.1f);

        threatEvent.gameObject.GetComponent<RectTransform>().anchoredPosition = activeThreatEventPos;

        float diffy = activeThreatEventPointAtPos.y - activeThreatEventPos.y;
        float diffx = activeThreatEventPointAtPos.x - activeThreatEventPos.x;
        float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
        threatEvent.pivotGameObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);

        threatEvent.activeThreatEvent.threatEventDistance = newDist;

    }

    public void threatEventEndDrag()
    {
        foreach (MapThreatOverPath mapThreatOverPath in mapThreatsOverPath)
            Destroy(mapThreatOverPath.gameObject);
        initActiveThreatOverPath();
    }

    public void stopEditThreatEvents()
    {
        isEditingThreatEvents = false;
        if (mapThreatEvents != null)
        {
            for (int i=0; i<mapThreatEvents.Count; i++)
            {
                Destroy(mapThreatEvents[i].gameObject);
            }
        }
        mapThreatEvents = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        mapScenarioManager.closeAllFloatingMenus();
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        mapScenarioManager.closeAllFloatingMenus();
        activeThreat.threatPosition = mapScenarioManager.mapView.FromAnchoredPositionToSteerPoint(this.transform.GetComponent<RectTransform>().anchoredPosition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
            addFloatingMenuThreat(mapScenarioManager.getMapAnchoredPosition(eventData.position));
    }

    private void addFloatingMenuThreat(Vector2 mapPosition)
    {
        mapScenarioManager.closeAllFloatingMenus();
        GameObject floatingMenu = Instantiate(floatingMenuActiveThreatPrefab) as GameObject;
        floatingMenu.transform.parent = mapScenarioManager.transform;
        floatingMenu.GetComponent<RectTransform>().anchoredPosition = mapPosition;
        floatingMenu.GetComponent<FloatingMenuActiveThreat>().initFloatingMenuActiveThreat(mapScenarioManager, this);
        activeFloatingMenu = floatingMenu;
    }

    public void closeFloatingMenu()
    {
        if (activeFloatingMenu != null)
        {
            Destroy(activeFloatingMenu);
            activeFloatingMenu = null;
        }
        if (mapThreatEvents != null)
        {
            foreach (MapThreatEvent mapThreatEvent in mapThreatEvents)
                mapThreatEvent.closeFloatingMenu();
        }
    }

    public Scenario.ActiveThreat getActiveThreatObject()
    {
        return activeThreat;
    }

    public void changeLinkedThreat(int newLinkIndex)
    {
        activeThreat.activeThreatLinkIndex = newLinkIndex;

        Threat relatedThreat = mapScenarioManager.getSimulatorDatabase().getThreatByIndex(activeThreat.activeThreatLinkIndex);
        activeThreatName.text = relatedThreat.threatName;
        float outerCircleSize = 2f * relatedThreat.threatRadius / mapScenarioManager.mapView.getMilesPerLengthUnit();
        outerCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(outerCircleSize, outerCircleSize);
    }


}
