using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapThreatOverPath : MonoBehaviour, IPointerClickHandler
{

    Vector3[] newVertices;
    int[] newTriangles;
    public Material material;

    private const float threatOverPathWidthMiles = 3f;

    private const float distSpaceBetweenMeshNodesInMiles = 0.5f;

    public void initMesh(MapScenarioManager mapScenarioManager, Scenario.ActiveThreat activeThreat)
    {
        FlightPath flightPath = mapScenarioManager.getFlightPath();
        float firstDist = activeThreat.activeThreatEvents[0].threatEventDistance;
        float lastDist = activeThreat.activeThreatEvents[activeThreat.activeThreatEvents.Length-1].threatEventDistance;
        int nodes = Mathf.RoundToInt((lastDist - firstDist) / distSpaceBetweenMeshNodesInMiles);
        float distBetweenNodes = (lastDist - firstDist) / (nodes - 1);
        float threatOverPathWidth = threatOverPathWidthMiles / mapScenarioManager.mapView.getMilesPerLengthUnit();

        Mesh mesh = new Mesh();
        newVertices = new Vector3[2* nodes];
        for (int i=0; i<nodes; i++)
        {
            float loopDist = firstDist + i * distBetweenNodes;
            Vector2[] posAndRightVec = flightPath.getPosAndRightDirInMilesDist(loopDist);
            newVertices[2 * i] = posAndRightVec[0] - threatOverPathWidth * posAndRightVec[1];
            newVertices[2 * i + 1] = posAndRightVec[0] + threatOverPathWidth * posAndRightVec[1];
        }

        int numTriangles = 2 * (nodes - 1);
        newTriangles = new int[numTriangles * 3];

        int loopVertexIndex = 0;
        for (int i=0; i<numTriangles*3; i+=6)
        {
            newTriangles[i + 0] = loopVertexIndex + 0;
            newTriangles[i + 1] = loopVertexIndex + 2;
            newTriangles[i + 2] = loopVertexIndex + 1;
            newTriangles[i + 3] = loopVertexIndex + 1;
            newTriangles[i + 4] = loopVertexIndex + 2;
            newTriangles[i + 5] = loopVertexIndex + 3;
            loopVertexIndex += 2;
        }

        //newVertices[0] = new Vector3(0f, 0f, 0f);
        //newVertices[1] = new Vector3(0f, 100f, 0f);
        //newVertices[2] = new Vector3(100f, 200f, 0f);
        //newVertices[3] = new Vector3(100f, 0f, 0f);

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;

        CanvasRenderer canvasRenderer = this.transform.GetComponent<CanvasRenderer>();
        canvasRenderer.materialCount = 1;
        canvasRenderer.SetMaterial(material, 0);
        canvasRenderer.SetMesh(mesh);

        //Graphics.DrawMesh(mesh, Matrix4x4.identity, material, 0);

        this.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click on threat!");
    }
}
