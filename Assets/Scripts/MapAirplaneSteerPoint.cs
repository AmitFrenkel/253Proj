using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapAirplaneSteerPoint : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private MapView mapView;
    public GameObject connectorLine;
    public Image thisImage; 

    public void initMapAirplaneSteerPoint(MapView mapView)
    {
        this.mapView = mapView;
    }

    public void makeALine(Transform newTrans)
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        //thisImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
        mapView.airplaneSteerPointDragged();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //thisImage.raycastTarget = true;
        mapView.airplaneSteerPointEndDrag();
    }

    public float getZRotation()
    {
        return connectorLine.transform.eulerAngles.z;
    }
}
