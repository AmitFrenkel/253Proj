using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MapActiveCircle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{

    private MapScenarioManager mapScenarioManager;
    private Scenario.ActiveMapCircle activeMapCircle;

    public GameObject outerCircle;
    public TextMeshProUGUI activeCircleName;

    public GameObject floatingMenuActiveCirclePrefab;
    private GameObject activeFloatingMenu;

    private bool isDragging;

    public void initMapActiveCircle(MapScenarioManager mapScenarioManager, Scenario.ActiveMapCircle activeMapCircle)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.activeMapCircle = activeMapCircle;
        activeFloatingMenu = null;
        isDragging = false;

        this.transform.GetComponent<RectTransform>().anchoredPosition = mapScenarioManager.mapView.FromScenarioSteerPointToAnchoredPosition(activeMapCircle.mapCenterPosition);

        MapCircle relatedMapCircle = mapScenarioManager.getSimulatorDatabase().getMapCircleByIndex(activeMapCircle.mapCircleIndexLinkIndex);
        activeCircleName.text = relatedMapCircle.circleName;
        float outerCircleSize = 2f * relatedMapCircle.circleRadius / mapScenarioManager.mapView.getMilesPerLengthUnit();
        outerCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(outerCircleSize, outerCircleSize);


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
        activeMapCircle.mapCenterPosition = mapScenarioManager.mapView.FromAnchoredPositionToSteerPoint(this.transform.GetComponent<RectTransform>().anchoredPosition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
            addFloatingMenuMapCircle(mapScenarioManager.getMapAnchoredPosition(eventData.position));
    }

    private void addFloatingMenuMapCircle(Vector2 mapPosition)
    {
        mapScenarioManager.closeAllFloatingMenus();
        GameObject floatingMenu = Instantiate(floatingMenuActiveCirclePrefab) as GameObject;
        floatingMenu.transform.parent = mapScenarioManager.transform;
        floatingMenu.GetComponent<RectTransform>().anchoredPosition = mapPosition;
        floatingMenu.GetComponent<FloatingMenuMapCircle>().initFloatingMenuMapCircle(mapScenarioManager, this);
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

    public Scenario.ActiveMapCircle getActiveMapCircleObject()
    {
        return activeMapCircle;
    }

    public void changeLinkedMapCircle(int newLinkIndex)
    {
        activeMapCircle.mapCircleIndexLinkIndex = newLinkIndex;

        MapCircle relatedMapCircle = mapScenarioManager.getSimulatorDatabase().getMapCircleByIndex(activeMapCircle.mapCircleIndexLinkIndex);
        activeCircleName.text = relatedMapCircle.circleName;
        float outerCircleSize = 2f * relatedMapCircle.circleRadius / mapScenarioManager.mapView.getMilesPerLengthUnit();
        outerCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(outerCircleSize, outerCircleSize);
    }


}
