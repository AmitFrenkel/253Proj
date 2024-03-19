using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapAirplaneSteerPoint : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    private MapScenarioManager mapScenarioManager;
    private Scenario.SteerPoint steerPoint;
    public GameObject connectorLine;
    private int steerPointIndex;

    public GameObject floatingMenuSteerPointPrefab;
    private GameObject activeFloatingMenu;

    private bool isDragging;

    public void initMapAirplaneSteerPoint(MapScenarioManager mapScenarioManager, Scenario.SteerPoint steerPoint, int steerPointIndex)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.steerPoint = steerPoint;
        this.steerPointIndex = steerPointIndex;
        activeFloatingMenu = null;
        isDragging = false;
        this.transform.GetComponent<RectTransform>().anchoredPosition = mapScenarioManager.mapView.FromScenarioSteerPointToAnchoredPosition(steerPoint);
    }

    public void setSteerPointIndex(int steerPointIndex)
    {
        this.steerPointIndex = steerPointIndex;
    }

    public void makeNavLine(Transform newTrans)
    {
        connectorLine.SetActive(true);
        //connectorLine.transform.LookAt(newTrans);
        float diffy = newTrans.position.y - this.transform.position.y;
        float diffx = newTrans.position.x - this.transform.position.x;
        float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
        connectorLine.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        float dist = Mathf.Sqrt(diffx * diffx + diffy * diffy);
        connectorLine.transform.localScale = new Vector3(dist / 100f, 1f, 1f);
    }

    public void hideNavLine()
    {
        connectorLine.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        mapScenarioManager.closeAllFloatingMenus();
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
        mapScenarioManager.airplaneSteerPointDragged();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        steerPoint = mapScenarioManager.mapView.FromAnchoredPositionToSteerPoint(this.transform.GetComponent<RectTransform>().anchoredPosition);
        mapScenarioManager.airplaneSteerPointEndDrag(steerPointIndex, steerPoint);
        mapScenarioManager.closeAllFloatingMenus();
    }

    public float getZRotation()
    {
        return connectorLine.transform.eulerAngles.z;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
            addFloatingMenuSteerPoint(mapScenarioManager.getMapAnchoredPosition(eventData.position));
    }

    public int getSteerPointIndex()
    {
        return steerPointIndex;
    }

    public Scenario.SteerPoint getSteerPointObject()
    {
        return steerPoint;
    }

    private void addFloatingMenuSteerPoint(Vector2 mapPosition)
    {
        mapScenarioManager.closeAllFloatingMenus();
        GameObject floatingMenu = Instantiate(floatingMenuSteerPointPrefab) as GameObject;
        floatingMenu.transform.parent = mapScenarioManager.transform;
        floatingMenu.GetComponent<RectTransform>().anchoredPosition = mapPosition;
        floatingMenu.GetComponent<FloatingMenuSteerPoint>().initFloatingMenuSteerPoint(mapScenarioManager, this);
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
}
