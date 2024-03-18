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

    public GameObject outerCircle;
    public TextMeshProUGUI activeThreatName;

    public GameObject floatingMenuActiveThreatPrefab;
    private GameObject activeFloatingMenu;

    public GameObject threatEventPrefab;
    public GameObject mapThreatOverPathPrefab;

    private List<MapThreatEvent> mapThreatEvents;

    private bool isDragging;

    public void initMapActiveThreat(MapScenarioManager mapScenarioManager, Scenario.ActiveThreat activeThreat)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.activeThreat = activeThreat;
        activeFloatingMenu = null;
        isDragging = false;

        this.transform.GetComponent<RectTransform>().anchoredPosition = mapScenarioManager.mapView.FromScenarioSteerPointToAnchoredPosition(activeThreat.threatPosition);

        threat = mapScenarioManager.getSimulatorDatabase().getThreatByIndex(activeThreat.activeThreatLinkIndex);
        activeThreatName.text = threat.threatName;
        float outerCircleSize = 2f * threat.threatRadius / mapScenarioManager.mapView.getMilesPerLengthUnit();
        outerCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(outerCircleSize, outerCircleSize);

        //initActiveThreatOverPath();
    }

    private void initActiveThreatOverPath()
    {
        FlightPath flightPath = mapScenarioManager.getFlightPath();
        mapThreatEvents = new List<MapThreatEvent>();
        foreach (Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent in activeThreat.activeThreatEvents)
        {
            GameObject threatEventGameObject = Instantiate(threatEventPrefab) as GameObject;
            threatEventGameObject.transform.parent = mapScenarioManager.transform;
            threatEventGameObject.GetComponent<MapThreatEvent>().initMapThreatEvent(mapScenarioManager, activeThreatEvent);

            Threat.ThreatLock threatLock = threat.threatLocks[activeThreatEvent.threatLockListIndex];
            string threatLockName = threatLock.threatLockName;
            float activeThreatEventDist = activeThreatEvent.threatEventDistance;
            Vector2 activeThreatEventPos = flightPath.getPosInMilesDist(activeThreatEventDist);
            Vector2 activeThreatEventPointAtPos = flightPath.getPosInMilesDist(activeThreatEventDist + 0.1f);

            threatEventGameObject.GetComponent<RectTransform>().anchoredPosition = activeThreatEventPos;

            float diffy = activeThreatEventPointAtPos.y - activeThreatEventPos.y;
            float diffx = activeThreatEventPointAtPos.x - activeThreatEventPos.x;
            float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
            threatEventGameObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);

            threatEventGameObject.GetComponent<MapThreatEvent>().setText(threat.threatName + " " + threatLockName);
            mapThreatEvents.Add(threatEventGameObject.GetComponent<MapThreatEvent>());

        }

        GameObject mapThreatOverPath = Instantiate(mapThreatOverPathPrefab) as GameObject;
        mapThreatOverPath.transform.parent = this.transform;
        mapThreatOverPath.GetComponent<MapThreatOverPath>().initMesh(mapScenarioManager, activeThreat);
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
