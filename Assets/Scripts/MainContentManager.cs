using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentManager : MonoBehaviour
{

    private SimulatorDatabase simulatorDatabase;
    public MainUIManager mainUIManager;
    public CollapsibleButtonsMainVerticalPanel categoriesMainPanel;
    public CollapsibleButtonsMainVerticalPanel elementsMainPanel;
    public MainVerticalPanel dataMainVerticalPanel;
    public GameObject editorEntityPrefab;
    private GameObject presentedEditorEntity;
    private SimulatorElement presentedSimulatorElement;
    private string presentedCategory;
    private int presentedElementIndex;
    public GameObject saveButton;
    public GameObject deleteButton;

    public enum SimulatorTypes { SystemVersion, Threat, MapCircle, UserResponse, EducationalScreen, Scenario, Train };


    public void initMainContentManager()
    {
        SimulatorDatabase simulatorDatabaseBeforeSerialization = getDemoSimulatorData();
        presentedEditorEntity = null;
        saveButton.GetComponent<SaveButton>().initSaveButton();
        saveButton.SetActive(false);
        deleteButton.SetActive(false);
        string json = JsonUtility.ToJson(simulatorDatabaseBeforeSerialization);
        //Debug.Log(json);
        simulatorDatabase = JsonUtility.FromJson<SimulatorDatabase>(json);
    }

    public void menuButtonSelected(UIMenuButton selectedButton)
    {
        //print("Menu butto selected. Cat: " + selectedButton.buttonCategory + " Name: " + selectedButton.buttonName);
        if (selectedButton.buttonCategory == "Categories")
        {
            selectMainCategory(selectedButton.buttonName);
            categoriesMainPanel.collapsePanel();
            elementsMainPanel.expandPanel();
        }
            
        else
            loadElement(selectedButton.buttonCategory, selectedButton.buttonIndex);
    }

    private void selectMainCategory(string categoryName)
    {
        presentedCategory = categoryName;
        elementsMainPanel.clearButtonsForPanel();
        List<SimulatorElement> simulatorElementsFromCategory = getListOfSimulatorElementsFromCategory(simulatorDatabase, categoryName);
        foreach (SimulatorElement simulatorElement in simulatorElementsFromCategory)
            elementsMainPanel.addUIMenuButton(categoryName, simulatorElement.getName(), simulatorElement.getIndex());
        elementsMainPanel.reorderUIMenuButtons();
    }

    private void loadElement(string category, int elementIndex)
    {
        // Debug.Log("Cat: " + category + " Load element idx = " + elementIndex);
        clearLoadedElement();
        presentedElementIndex = elementIndex;
        SimulatorElement selectedSimulatorElement = null;
        switch (category)
        {
            case "SystemVersions": selectedSimulatorElement = simulatorDatabase.getSystemVersionByIndex(elementIndex); break;
            case "Threats": selectedSimulatorElement = simulatorDatabase.getThreatByIndex(elementIndex); break;
            case "Circles": selectedSimulatorElement = simulatorDatabase.getMapCircleByIndex(elementIndex); break;
        }
        presentedSimulatorElement = selectedSimulatorElement;
        GameObject editorEntity = Instantiate(editorEntityPrefab) as GameObject;
        editorEntity.transform.parent = dataMainVerticalPanel.contectHolder.transform;
        editorEntity.GetComponent<EditorEntity>().initEditorEntity(selectedSimulatorElement, this, -10f, null);
        editorEntity.GetComponent<EditorEntity>().setBaseHeight(-10f);
        editorEntity.GetComponent<EditorEntity>().reorderEditorElement();
        presentedEditorEntity = editorEntity;
        saveButton.SetActive(true);
        saveButton.GetComponent<SaveButton>().setAsSaved();
        deleteButton.SetActive(true);
    }

    public void addNewElement()
    {
        int nextIndex = 0;
        switch (presentedCategory)
        {
            case "SystemVersions": nextIndex = simulatorDatabase.getNextSystemVersionIndex(); break;
            case "Threats": nextIndex = simulatorDatabase.getNextThreatIndex(); break;
            case "Circles": nextIndex = simulatorDatabase.getNextMapCircleIndex(); break;
        }
        switch (presentedCategory)
        {
            case "SystemVersions":
                SystemVersion systemVersion = new SystemVersion(nextIndex);
                simulatorDatabase.systemVersions.Add(systemVersion);
                break;
            case "Threats":
                Threat threat = new Threat(nextIndex);
                simulatorDatabase.threats.Add(threat);
                break;
            case "Circles":
                MapCircle mapCicle = new MapCircle(nextIndex);
                simulatorDatabase.mapCircles.Add(mapCicle);
                break;
        }
        elementsMainPanel.addUIMenuButton(presentedCategory, "<Unnamed>", nextIndex, true);
        elementsMainPanel.reorderUIMenuButtons();
        loadElement(presentedCategory, nextIndex);
    }

    public void saveElement()
    {
        presentedEditorEntity.GetComponent<EditorEntity>().saveElement();
        elementsMainPanel.editButtonName(presentedElementIndex, presentedSimulatorElement.getName());
    }

    public void clearLoadedElement()
    {
        if (presentedEditorEntity != null)
            Destroy(presentedEditorEntity);
        presentedElementIndex = 0;
        presentedSimulatorElement = null;
        categoriesMainPanel.collapsePanel();
        elementsMainPanel.expandPanel();
        saveButton.SetActive(false);
        deleteButton.SetActive(false);
    }

    public void deleteElement()
    {
        switch (presentedCategory)
        {
            case "SystemVersions": simulatorDatabase.systemVersions.Remove(simulatorDatabase.getSystemVersionByIndex(presentedElementIndex)); break;
            case "Threats": simulatorDatabase.threats.Remove(simulatorDatabase.getThreatByIndex(presentedElementIndex)); break;
            case "Circles": simulatorDatabase.mapCircles.Remove(simulatorDatabase.getMapCircleByIndex(presentedElementIndex)); break;
        }
        elementsMainPanel.removeUIMenuButton(presentedElementIndex);
        clearLoadedElement();
    }

    public void setEditorLineModified()
    {
        saveButton.GetComponent<SaveButton>().setAsNeedToSave();
    }

    public void setEditorLineChangedHeight()
    {
        if (presentedEditorEntity != null)
            presentedEditorEntity.GetComponent<EditorEntity>().reorderEditorElement();
    }

    private List<SimulatorElement> getListOfSimulatorElementsFromCategory(SimulatorDatabase simulatorDatabase, string categoryName)
    {
        List<SimulatorElement> simulatorElements = new List<SimulatorElement>();
        switch (categoryName)
        {
            case "SystemVersions":
                if (simulatorDatabase.systemVersions != null)
                    foreach (SystemVersion loopElement in simulatorDatabase.systemVersions)
                        simulatorElements.Add(loopElement);
                        
                break;
            case "Threats":
                if (simulatorDatabase.threats != null)
                    foreach (Threat loopElement in simulatorDatabase.threats)
                        simulatorElements.Add(loopElement);
                break;
            case "Circles":
                if (simulatorDatabase.mapCircles != null)
                    foreach (MapCircle loopElement in simulatorDatabase.mapCircles)
                        simulatorElements.Add(loopElement);
                break;
        }
        return simulatorElements;
    }

    public SimulatorDatabase getSimulatorDatabase()
    {
        return simulatorDatabase;
    }

    public SimulatorDatabase getDemoSimulatorData()
    {
        SimulatorDatabase simulatorDatabase = new SimulatorDatabase();

        simulatorDatabase.systemVersions = new List<SystemVersion>();
        simulatorDatabase.systemVersions.Add(new SystemVersion(0, "Version 0", new int[] { 0, 1 }));
        simulatorDatabase.systemVersions.Add(new SystemVersion(1, "Version 1", new int[] { 0, 1, 2 }));
        simulatorDatabase.systemVersions.Add(new SystemVersion(2, "Version 2", new int[] { 2 }));

        simulatorDatabase.threats = new List<Threat>();
        simulatorDatabase.threats.Add(new Threat(0,
                                                 "Tr0",
                                                 0.5f,
                                                 new Threat.ThreatLock[] {
                                                     new Threat.ThreatLock("MA",
                                                                           new string[]{"path_to_symbol.jpg"},
                                                                           "path_to_sound.mp3")
                                                 },
                                                 5f,
                                                 2f));

        simulatorDatabase.threats.Add(new Threat(1,
                                                 "Tr1",
                                                 0.2f,
                                                 new Threat.ThreatLock[] {
                                                     new Threat.ThreatLock("MA",
                                                                           new string[]{"path_to_symbol_MA.jpg"},
                                                                           "path_to_sound2.mp3"),
                                                     new Threat.ThreatLock("ML",
                                                                           new string[]{"path_to_symbol_ML.jpg"},
                                                                           "path_to_sound2.mp3")
                                                 },
                                                 4.5f,
                                                 2.5f));

        simulatorDatabase.threats.Add(new Threat(2,
                                                 "Tr1 Improved",
                                                 0.8f,
                                                 new Threat.ThreatLock[] {
                                                     new Threat.ThreatLock("MA",
                                                                           new string[]{"path_to_symbol_MA1.jpg"},
                                                                           "path_to_sound2.mp3"),
                                                     new Threat.ThreatLock("ML",
                                                                           new string[]{"path_to_symbol_ML1.jpg"},
                                                                           "path_to_sound2.mp3")
                                                 },
                                                 5f,
                                                 2.5f));


        return simulatorDatabase;

    }


}
