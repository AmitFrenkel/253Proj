using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MapActiveCircle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{

    private MapView mapView;
    private Scenario.ActiveMapCircle activeMapCircle;

    public GameObject outerCircle;
    public TextMeshProUGUI activeCircleName;

    public GameObject floatingMenuActiveCirclePrefab;
    private GameObject activeFloatingMenu;

    private bool isDragging;

    public void initMapActiveCircle(MapView mapView, Scenario.ActiveMapCircle activeMapCircle)
    {
        this.mapView = mapView;
        this.activeMapCircle = activeMapCircle;
        activeFloatingMenu = null;
        isDragging = false;

        Scenario.SteerPoint mapCircleSteerPoint = activeMapCircle.mapCenterPosition;
        this.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(MapView.ToXmapPos(mapCircleSteerPoint.E), MapView.ToYmapPos(mapCircleSteerPoint.N));

        MapCircle relatedMapCircle = mapView.getSimulatorDatabase().getMapCircleByIndex(activeMapCircle.mapCircleIndexLinkIndex);
        activeCircleName.text = relatedMapCircle.circleName;
        float outerCircleSize = 2f * relatedMapCircle.circleRadius / mapView.getMilesPerLengthUnit();
        outerCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(outerCircleSize, outerCircleSize);


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        closeFloatingMenu();
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        mapView.airplaneSteerPointEndDrag();
        closeFloatingMenu();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
            addFloatingMenuMapCircle(mapView.getMapAnchoredPosition(eventData.position));
    }

    private void addFloatingMenuMapCircle(Vector2 mapPosition)
    {
        mapView.closeAllFloatingMenus();
        GameObject floatingMenu = Instantiate(floatingMenuActiveCirclePrefab) as GameObject;
        floatingMenu.transform.parent = mapView.transform;
        floatingMenu.GetComponent<RectTransform>().anchoredPosition = mapPosition;
        floatingMenu.GetComponent<FloatingMenuMapCircle>().initFloatingMenuMapCircle(mapView, this);
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
