using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EditorEntitiesList : EditorLine
{
    private SimulatorClass parentSimulatorClass;
    public TextMeshProUGUI displayNameText;
    public GameObject editorEntityPrefab;
    private List<EditorEntity> editorEntities;
    private float startHeight;
    private const float editorLineHeight = 40f;
    private bool hasConstantSingleElement;
    private SimulatorClass[] arrSimulatorClass;
    private Type elementsType;
    public RectTransform addButton;
    public RectTransform listHeader;


    public void initEditorEntitiesList(SimulatorClass[] simulatorClasses, SimulatorClass parentSimulatorClass, Type elementsType, MainContentManager mainContentManager, float xOffset, bool hasConstantSingleElement)
    {
        this.mainContentManager = mainContentManager;
        this.parentSimulatorClass = parentSimulatorClass;
        this.xOffset = xOffset;
        arrSimulatorClass = simulatorClasses;
        this.elementsType = elementsType;
        this.hasConstantSingleElement = hasConstantSingleElement;
        editorEntities = new List<EditorEntity>();
        startHeight -= editorLineHeight*2;
        if (hasConstantSingleElement)
        {
            addButton.gameObject.SetActive(false);
            buildConstantSingleElementEntitiesList(simulatorClasses[0], parentSimulatorClass);
        }
        else
        {
            buildEditorEntitiesList(simulatorClasses, parentSimulatorClass);
        }
    }

    public void setDisplayName(string name)
    {
        displayNameText.text = name;
    }

    private void buildEditorEntitiesList(SimulatorClass[] simulatorClasses, SimulatorClass parentSimulatorClass)
    {
        for (int i=0; i< simulatorClasses.Length; i++)
        {
            GameObject newEditorEntity = Instantiate(editorEntityPrefab) as GameObject;
            newEditorEntity.transform.parent = this.transform;
            newEditorEntity.GetComponent<EditorEntity>().initEditorEntity(simulatorClasses[i], parentSimulatorClass, mainContentManager, -UIViewConfigurations.headerWidth, this);
            editorEntities.Add(newEditorEntity.GetComponent<EditorEntity>());
        }
    }

    private void buildConstantSingleElementEntitiesList(SimulatorClass simulatorClass, SimulatorClass parentSimulatorClass)
    {
        GameObject newEditorEntity = Instantiate(editorEntityPrefab) as GameObject;
        newEditorEntity.transform.parent = this.transform;
        newEditorEntity.GetComponent<EditorEntity>().initEditorEntity(simulatorClass, parentSimulatorClass, mainContentManager, -UIViewConfigurations.headerWidth, null);
        editorEntities.Add(newEditorEntity.GetComponent<EditorEntity>());
    }

    public void deleteEditorEntityInList(EditorEntity editorEntity)
    {
        mainContentManager.startEditorBuild();
        int indexOfElement = Array.IndexOf(arrSimulatorClass, editorEntity.getLinkedSimulatorClass());
        RemoveAt(ref arrSimulatorClass, indexOfElement);
        editorEntities.Remove(editorEntity);
        Destroy(editorEntity.gameObject);
        mainContentManager.endEditorBuildAndSave();
        reportEditorLineChangedHeight();
    }

    public void addElement()
    {
        SimulatorClass newElement = null;
        switch (elementsType.ToString())
        {
            case "Threat+ThreatLock": newElement = new Threat.ThreatLock(); break;
            case "Scenario+SteerPoint": newElement = new Scenario.SteerPoint(); break;
            case "Scenario+ActiveThreat": newElement = new Scenario.ActiveThreat(); break;
            case "Scenario+ActiveThreat+ActiveThreatEvent": newElement = new Scenario.ActiveThreat.ActiveThreatEvent(); break;
            case "Scenario+ActiveThreat+UserResponeToThreat": newElement = new Scenario.ActiveThreat.UserResponeToThreat(); break;
            case "Scenario+ActiveMapCircle": newElement = new Scenario.ActiveMapCircle(); break;
            default: Debug.Log("Add new type to switch case : " + elementsType.ToString()); break;
        }
        Array.Resize(ref arrSimulatorClass, arrSimulatorClass.Length + 1);
        arrSimulatorClass[arrSimulatorClass.Length - 1] = newElement;

        mainContentManager.startEditorBuild();
        GameObject newEditorEntity = Instantiate(editorEntityPrefab) as GameObject;
        newEditorEntity.transform.parent = this.transform;
        newEditorEntity.GetComponent<EditorEntity>().initEditorEntity(newElement, parentSimulatorClass, mainContentManager, -UIViewConfigurations.headerWidth, this);
        editorEntities.Add(newEditorEntity.GetComponent<EditorEntity>());
        mainContentManager.endEditorBuildAndSave();
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
        if (!hasConstantSingleElement)
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

