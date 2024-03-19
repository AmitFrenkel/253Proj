using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapThreatOverPath : MonoBehaviour
{

    Vector3[] newVertices;
    int[] newTriangles;
    public Material baseMaterial;
    private Material material;
    private MapScenarioManager mapScenarioManager;
    private FlightPath flightPath;
    private float firstDist;
    private float lastDist;

    private const float threatOverPathWidthMiles = 3f;
    private const float distSpaceBetweenMeshNodesInMiles = 0.5f;
    private Color pathColor;

    public void initMesh(MapScenarioManager mapScenarioManager, float firstDist, float lastDist, Color baseColor, bool isBoldColor)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.firstDist = firstDist;
        this.lastDist = lastDist;

        this.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);

        float h_color = 0f;
        float s_color = 0f;
        float v_color = 0f;
        Color.RGBToHSV(baseColor, out h_color, out s_color, out v_color);
        Color matColor = Color.HSVToRGB(h_color, isBoldColor ? s_color : s_color/5f, v_color);
        matColor.a = 0.3f;
        pathColor = matColor;
        material = new Material(baseMaterial);
        material.SetColor("_Color", matColor);
        //material.color = matColor;

        buildMesh();

    }

    public void buildMesh()
    {
        int nodes = Mathf.RoundToInt((lastDist - firstDist) / distSpaceBetweenMeshNodesInMiles);
        float distBetweenNodes = (lastDist - firstDist) / (nodes - 1);
        float threatOverPathWidth = threatOverPathWidthMiles / mapScenarioManager.mapView.getMilesPerLengthUnit();
        flightPath = mapScenarioManager.getFlightPath();

        Mesh mesh = new Mesh();
        newVertices = new Vector3[2 * nodes];
        for (int i = 0; i < nodes; i++)
        {
            float loopDist = firstDist + i * distBetweenNodes;
            Vector2[] posAndRightVec = flightPath.getPosAndRightDirInMilesDist(loopDist);
            newVertices[2 * i] = posAndRightVec[0] - threatOverPathWidth * posAndRightVec[1];
            newVertices[2 * i + 1] = posAndRightVec[0] + threatOverPathWidth * posAndRightVec[1];
        }

        int numTriangles = 2 * (nodes - 1);
        newTriangles = new int[numTriangles * 3];

        int loopVertexIndex = 0;
        for (int i = 0; i < numTriangles * 3; i += 6)
        {
            newTriangles[i + 0] = loopVertexIndex + 0;
            newTriangles[i + 1] = loopVertexIndex + 2;
            newTriangles[i + 2] = loopVertexIndex + 1;
            newTriangles[i + 3] = loopVertexIndex + 1;
            newTriangles[i + 4] = loopVertexIndex + 2;
            newTriangles[i + 5] = loopVertexIndex + 3;
            loopVertexIndex += 2;
        }

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;

        CanvasRenderer canvasRenderer = this.transform.GetComponent<CanvasRenderer>();
        canvasRenderer.materialCount = 1;
        canvasRenderer.SetMaterial(material, 0);
        canvasRenderer.SetMesh(mesh);
    }

}
