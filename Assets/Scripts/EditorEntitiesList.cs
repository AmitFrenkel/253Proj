using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EditorEntitiesList : EditorLine
{
    public TextMeshProUGUI displayNameText;
    public GameObject editorEntityPrefab;
    private List<EditorEntity> editorEntities;
    private float startHeight;
    private float loopHeight;
    private const float editorLineHeight = 40f;
    private const float listXOffset = -40f;
    private SimulatorClass[] arrSimulatorClass;
    private Type elementsType;
    public RectTransform addButton;
    public RectTransform listHeader;


    public void initEditorEntitiesList(SimulatorClass[] simulatorClasss, Type elementsType, MainContentManager mainContentManager, float xOffset)
    {
        this.mainContentManager = mainContentManager;
        this.xOffset = xOffset;
        arrSimulatorClass = simulatorClasss;
        this.elementsType = elementsType;
        editorEntities = new List<EditorEntity>();
        startHeight -= editorLineHeight*2;
        loopHeight = startHeight;
        buildEditorEntitiesList(simulatorClasss);
    }

    public void setDisplayName(string name)
    {
        displayNameText.text = name;
    }

    private void buildEditorEntitiesList(SimulatorClass[] simulatorClasss)
    {
        for (int i=0; i< simulatorClasss.Length; i++)
        {
            GameObject newEditorEntity = Instantiate(editorEntityPrefab) as GameObject;
            newEditorEntity.transform.parent = this.transform;
            //newEditorEntity.GetComponent<RectTransform>().anchoredPosition = new Vector2(listXOffset, loopHeight);
            newEditorEntity.GetComponent<EditorEntity>().initEditorEntity(simulatorClasss[i], mainContentManager, -UIViewConfigurations.headerWidth, this);
            editorEntities.Add(newEditorEntity.GetComponent<EditorEntity>());
        }
    }

    public void deleteEditorEntityInList(EditorEntity editorEntity)
    {
        int indexOfElement = Array.IndexOf(arrSimulatorClass, editorEntity.getLinkedSimulatorClass());
        Debug.Log("beofre delete: " + arrSimulatorClass.Length);
        RemoveAt(ref arrSimulatorClass, indexOfElement);
        Debug.Log("after delete: " + arrSimulatorClass.Length);
        editorEntities.Remove(editorEntity);
        Destroy(editorEntity.gameObject);
        reportEditorLineModified();
        reportEditorLineChangedHeight();
    }

    public void addElement()
    {
        SimulatorClass newElement = null;
        switch (elementsType.ToString())
        {
            case "Threat+ThreatLock": newElement = new Threat.ThreatLock("NEW NAME", new string[] { }, "<Path to sound>"); break;
            default: Debug.Log("Add new type to switch case : " + elementsType.ToString()); break;
        }
        GameObject newEditorEntity = Instantiate(editorEntityPrefab) as GameObject;
        newEditorEntity.transform.parent = this.transform;
        //newEditorEntity.GetComponent<RectTransform>().anchoredPosition = new Vector2(listXOffset, loopHeight);
        newEditorEntity.GetComponent<EditorEntity>().initEditorEntity(newElement, mainContentManager, -UIViewConfigurations.headerWidth, this);
        editorEntities.Add(newEditorEntity.GetComponent<EditorEntity>());
        Array.Resize(ref arrSimulatorClass, arrSimulatorClass.Length + 1);
        arrSimulatorClass[arrSimulatorClass.Length - 1] = newElement;
        //loopHeight -= editorLineHeight * 3;
        reportEditorLineModified();
        reportEditorLineChangedHeight();
    }

    public override void reorderEditorElement()
    {
        reorderLinesInList();
    }

    private void reorderLinesInList()
    {
        this.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, baseHeight);
        float loopHeight = 0f;
        for (int i=0; i< editorEntities.Count; i++)
        {
            EditorEntity loopEditorEntity = editorEntities[i];
            loopEditorEntity.setNameInList((i + 1).ToString());
            loopEditorEntity.setBaseHeight(loopHeight);
            loopEditorEntity.reorderEditorElement();
            loopHeight += loopEditorEntity.getHeightOfEditorElement();
        }
        addButton.anchoredPosition = new Vector2(-UIViewConfigurations.headerWidth, loopHeight);
        loopHeight -= UIViewConfigurations.dataLineHeight;
        listHeader.sizeDelta = new Vector2(listHeader.sizeDelta.x, -loopHeight - UIViewConfigurations.spacingBetweenLinesHeight);
        listHeader.anchoredPosition = new Vector2(0f, 0f);
        editorElementHeight = loopHeight;
    }

    public void saveEditorEntities()
    {
        foreach (EditorEntity editorEntity in editorEntities)
        {
            editorEntity.saveElement();
        }
    }

    public SimulatorClass[] GetSimulatorClassesArr()
    {
        return arrSimulatorClass;
    }

    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            arr[a] = arr[a + 1];
        }
        Array.Resize(ref arr, arr.Length - 1);
    }

    public string getSimulatorClassElementsTypeToString()
    {
        return elementsType.ToString();
    }
}

