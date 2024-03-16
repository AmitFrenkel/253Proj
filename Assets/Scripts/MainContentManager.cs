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
    public GameObject mapEditButton;
    public GameObject addNewElementButton;
    private bool isEditorDuringBuild;
    //private bool isMapEditEnabled;
    public MapView mapView;
    private bool isEditingScenarioInMapView;

    public enum SimulatorTypes { SystemVersion, Threat, MapCircle, UserResponse, EducationalScreen, Scenario, Train };


    public void initMainContentManager()
    {
        SimulatorDatabase simulatorDatabaseBeforeSerialization = getDemoSimulatorData();
        presentedEditorEntity = null;
        saveButton.GetComponent<SaveButton>().initSaveButton();
        //saveButton.SetActive(false);
        deleteButton.SetActive(false);
        string json = JsonUtility.ToJson(simulatorDatabaseBeforeSerialization);
        //Debug.Log(json);
        simulatorDatabase = JsonUtility.FromJson<SimulatorDatabase>(json);
        isEditingScenarioInMapView = false;
        mapView.initMapViewByScenario(simulatorDatabase.scenarios[0], this);
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
        clearLoadedElement();
        presentedCategory = categoryName;
        elementsMainPanel.clearButtonsForPanel();
        List<SimulatorElement> simulatorElementsFromCategory = getListOfSimulatorElementsFromCategory(simulatorDatabase, categoryName);
        foreach (SimulatorElement simulatorElement in simulatorElementsFromCategory)
            elementsMainPanel.addUIMenuButton(categoryName, simulatorElement.getName(), simulatorElement.getIndex(), false);
        elementsMainPanel.reorderUIMenuButtons();
        addNewElementButton.SetActive(true);
    }

    private void loadElement(string category, int elementIndex)
    {
        // Debug.Log("Cat: " + category + " Load element idx = " + elementIndex);
        clearLoadedElement();
        hideMapViewEdit();
        presentedElementIndex = elementIndex;
        SimulatorElement selectedSimulatorElement = null;
        switch (category)
        {
            case "SystemVersion": selectedSimulatorElement = simulatorDatabase.getSystemVersionByIndex(elementIndex); break;
            case "Threat": selectedSimulatorElement = simulatorDatabase.getThreatByIndex(elementIndex); break;
            case "MapCircle": selectedSimulatorElement = simulatorDatabase.getMapCircleByIndex(elementIndex); break;
            case "UserResponse": selectedSimulatorElement = simulatorDatabase.getUserResponseByIndex(elementIndex); break;
            case "EducationalScreen": selectedSimulatorElement = simulatorDatabase.getEducationalScreenByIndex(elementIndex); break;
            case "Scenario": selectedSimulatorElement = simulatorDatabase.getScenarioByIndex(elementIndex); break;
            case "Train": selectedSimulatorElement = simulatorDatabase.getTrainByIndex(elementIndex); break;
        }
        presentedSimulatorElement = selectedSimulatorElement;
        GameObject editorEntity = Instantiate(editorEntityPrefab) as GameObject;
        editorEntity.transform.parent = dataMainVerticalPanel.contectHolder.transform;
        startEditorBuild();
        editorEntity.GetComponent<EditorEntity>().initEditorEntity(selectedSimulatorElement, null, this, -10f, null);
        endEditorBuild();
        editorEntity.GetComponent<EditorEntity>().setBaseHeight(-10f);
        editorEntity.GetComponent<EditorEntity>().reorderEditorElement();
        presentedEditorEntity = editorEntity;
        //saveButton.SetActive(true);
        //saveButton.GetComponent<SaveButton>().setAsSaved();
        deleteButton.SetActive(true);
        if (category == "Scenario")
            mapEditButton.SetActive(true);
    }

    public void addNewElement()
    {
        int nextIndex = 0;
        switch (presentedCategory)
        {
            case "SystemVersion": nextIndex = simulatorDatabase.getNextSystemVersionIndex(); break;
            case "Threat": nextIndex = simulatorDatabase.getNextThreatIndex(); break;
            case "MapCircle": nextIndex = simulatorDatabase.getNextMapCircleIndex(); break;
            case "UserResponse": nextIndex = simulatorDatabase.getNextUserResponseIndex(); break;
            case "EducationalScreen": nextIndex = simulatorDatabase.getNextEducationalScreenIndex(); break;
            case "Scenario": nextIndex = simulatorDatabase.getNextScenarioIndex(); break;
            case "Train": nextIndex = simulatorDatabase.getNextTrainIndex(); break;
        }
        switch (presentedCategory)
        {
            case "SystemVersion":
                SystemVersion systemVersion = new SystemVersion(nextIndex);
                simulatorDatabase.systemVersions.Add(systemVersion);
                break;
            case "Threat":
                Threat threat = new Threat(nextIndex);
                simulatorDatabase.threats.Add(threat);
                break;
            case "MapCircle":
                MapCircle mapCicle = new MapCircle(nextIndex);
                simulatorDatabase.mapCircles.Add(mapCicle);
                break;
            case "UserResponse":
                UserResponse userResponse = new UserResponse(nextIndex);
                simulatorDatabase.userResponses.Add(userResponse);
                break;
            case "EducationalScreen":
                EducationalScreen educationalScreen = new EducationalScreen(nextIndex);
                simulatorDatabase.educationalScreens.Add(educationalScreen);
                break;
            case "Scenario":
                Scenario scenario = new Scenario(nextIndex);
                simulatorDatabase.scenarios.Add(scenario);
                break;
            case "Train":
                Train train = new Train(nextIndex);
                simulatorDatabase.trains.Add(train);
                break;
        }
        elementsMainPanel.addUIMenuButton(presentedCategory, "<Unnamed>", nextIndex, true);
        elementsMainPanel.reorderUIMenuButtons();
        startEditorBuild();
        loadElement(presentedCategory, nextIndex);
        endEditorBuildAndSave();
    }

    public void saveElementToOutputFile()
    {
        saveChangesFromEditorToDatabase();
        Debug.Log("Saved to output file... (TODO)");
        saveButton.GetComponent<SaveButton>().setAsSaved();
    }

    public void clearLoadedElement()
    {
        if (presentedEditorEntity != null)
            Destroy(presentedEditorEntity);
        presentedElementIndex = 0;
        presentedSimulatorElement = null;
        categoriesMainPanel.collapsePanel();
        elementsMainPanel.expandPanel();
        //saveButton.SetActive(false);
        deleteButton.SetActive(false);
        mapEditButton.SetActive(false);
    }

    public void deleteElement()
    {
        switch (presentedCategory)
        {
            case "SystemVersions": simulatorDatabase.systemVersions.Remove(simulatorDatabase.getSystemVersionByIndex(presentedElementIndex)); break;
            case "Threats": simulatorDatabase.threats.Remove(simulatorDatabase.getThreatByIndex(presentedElementIndex)); break;
            case "MapCircle": simulatorDatabase.mapCircles.Remove(simulatorDatabase.getMapCircleByIndex(presentedElementIndex)); break;
            case "UserResponse": simulatorDatabase.userResponses.Remove(simulatorDatabase.getUserResponseByIndex(presentedElementIndex)); break;
            case "EducationalScreen": simulatorDatabase.educationalScreens.Remove(simulatorDatabase.getEducationalScreenByIndex(presentedElementIndex)); break;
            case "Scenario": simulatorDatabase.scenarios.Remove(simulatorDatabase.getScenarioByIndex(presentedElementIndex)); break;
            case "Train": simulatorDatabase.trains.Remove(simulatorDatabase.getTrainByIndex(presentedElementIndex)); break;
        }
        elementsMainPanel.removeUIMenuButton(presentedElementIndex);
        clearLoadedElement();
    }

    public void setEditorLineModified()
    {
        if (!isEditorDuringBuild)
        {
            saveButton.GetComponent<SaveButton>().setAsNeedToSave();
            saveChangesFromEditorToDatabase();
        }
    }

    public void setMapViewModified()
    {
        saveButton.GetComponent<SaveButton>().setAsNeedToSave();
        mapView.saveChangesFromMapViewToDatabase();
    }

    public void saveChangesFromEditorToDatabase()
    {
        if (presentedEditorEntity != null & !isEditorDuringBuild)
        {
            presentedEditorEntity.GetComponent<EditorEntity>().saveElement();
            elementsMainPanel.editButtonName(presentedElementIndex, presentedSimulatorElement.getName());
        }
    }

    public void setEditorLineChangedHeight()
    {
        if (presentedEditorEntity != null)
            presentedEditorEntity.GetComponent<EditorEntity>().reorderEditorElement();
    }

    public void startEditorBuild()
    {
        isEditorDuringBuild = true;
    }

    public void endEditorBuild()
    {
        isEditorDuringBuild = false;
    }

    public void endEditorBuildAndSave()
    {
        isEditorDuringBuild = false;
        saveChangesFromEditorToDatabase();
    }

    public void toggleEditScenarioOverMap()
    {
        if (isEditingScenarioInMapView)
            hideMapViewEdit();
        else
            loadMapViewEdit();
    }

    private void hideMapViewEdit()
    {
        isEditingScenarioInMapView = false;
        mapView.gameObject.SetActive(false);
        dataMainVerticalPanel.contectHolder.gameObject.SetActive(true);
    }

    private void loadMapViewEdit()
    {
        isEditingScenarioInMapView = true;
        mapView.gameObject.SetActive(true);
        dataMainVerticalPanel.contectHolder.gameObject.SetActive(false);
        mapView.initMapViewByScenario(presentedSimulatorElement as Scenario, this);
    }

    private List<SimulatorElement> getListOfSimulatorElementsFromCategory(SimulatorDatabase simulatorDatabase, string categoryName)
    {
        List<SimulatorElement> simulatorElements = new List<SimulatorElement>();
        switch (categoryName)
        {
            case "SystemVersion":
                if (simulatorDatabase.systemVersions != null)
                    foreach (SystemVersion loopElement in simulatorDatabase.systemVersions)
                        simulatorElements.Add(loopElement);
                        
                break;
            case "Threat":
                if (simulatorDatabase.threats != null)
                    foreach (Threat loopElement in simulatorDatabase.threats)
                        simulatorElements.Add(loopElement);
                break;
            case "MapCircle":
                if (simulatorDatabase.mapCircles != null)
                    foreach (MapCircle loopElement in simulatorDatabase.mapCircles)
                        simulatorElements.Add(loopElement);
                break;
            case "UserResponse":
                if (simulatorDatabase.userResponses != null)
                    foreach (UserResponse loopElement in simulatorDatabase.userResponses)
                        simulatorElements.Add(loopElement);
                break;
            case "EducationalScreen":
                if (simulatorDatabase.educationalScreens != null)
                    foreach (EducationalScreen loopElement in simulatorDatabase.educationalScreens)
                        simulatorElements.Add(loopElement);
                break;
            case "Scenario":
                if (simulatorDatabase.scenarios != null)
                    foreach (Scenario loopElement in simulatorDatabase.scenarios)
                        simulatorElements.Add(loopElement);
                break;
            case "Train":
                if (simulatorDatabase.trains != null)
                    foreach (Train loopElement in simulatorDatabase.trains)
                        simulatorElements.Add(loopElement);
                break;
        }
        return simulatorElements;
    }

    public SimulatorDatabase getSimulatorDatabase()
    {
        return simulatorDatabase;
    }

    public SimulatorElement getPresentedSimilatorElement()
    {
        return presentedSimulatorElement;
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
                                                     new Threat.ThreatLock("MA0",
                                                                           new string[]{"path_to_symbol.jpg"},
                                                                           "path_to_sound.mp3", new RGBColor())
                                                 },
                                                 5f,
                                                 2f));

        simulatorDatabase.threats.Add(new Threat(1,
                                                 "Tr1",
                                                 0.2f,
                                                 new Threat.ThreatLock[] {
                                                     new Threat.ThreatLock("MA1",
                                                                           new string[]{"path_to_symbol_MA.jpg"},
                                                                           "path_to_sound2.mp3", new RGBColor()),
                                                     new Threat.ThreatLock("ML1",
                                                                           new string[]{"path_to_symbol_ML.jpg"},
                                                                           "path_to_sound2.mp3", new RGBColor())
                                                 },
                                                 4.5f,
                                                 2.5f));

        simulatorDatabase.threats.Add(new Threat(2,
                                                 "Tr2",
                                                 0.8f,
                                                 new Threat.ThreatLock[] {
                                                     new Threat.ThreatLock("MA2",
                                                                           new string[]{"path_to_symbol_MA1.jpg"},
                                                                           "path_to_sound2.mp3", new RGBColor()),
                                                     new Threat.ThreatLock("ML2",
                                                                           new string[]{"path_to_symbol_ML1.jpg"},
                                                                           "path_to_sound2.mp3", new RGBColor())
                                                 },
                                                 5f,
                                                 2.5f));

        simulatorDatabase.mapCircles = new List<MapCircle>();
        simulatorDatabase.mapCircles.Add(new MapCircle(0,
                                                       "SAx",
                                                       6.5f,
                                                       "path_to_symbol_SAx.jpg",
                                                       new RGBColor(0.8f, 0.1f, 0.3f)));

        simulatorDatabase.userResponses = new List<UserResponse>();
        simulatorDatabase.userResponses.Add(new UserResponse(0, "Left", true));
        simulatorDatabase.userResponses.Add(new UserResponse(1, "Right", true));
        simulatorDatabase.userResponses.Add(new UserResponse(2, "Keep going", false));

        simulatorDatabase.educationalScreens = new List<EducationalScreen>();

        simulatorDatabase.scenarios = new List<Scenario>();
        simulatorDatabase.scenarios.Add(new Scenario(0,
                                                     "Scenario0",
                                                     new Scenario.SteerPoint[] { new Scenario.SteerPoint("3320.000", "3520.000"),
                                                                                 new Scenario.SteerPoint("3338.000", "3523.000"),
                                                                                 new Scenario.SteerPoint("3340.000", "3540.000"),
                                                                                 new Scenario.SteerPoint("3325.000", "3541.000")
                                                     //new Scenario.SteerPoint[] { new Scenario.SteerPoint("3400.000", "3500.000"),
                                                     //                            new Scenario.SteerPoint("3430.000", "3500.000"),
                                                     //                            new Scenario.SteerPoint("3430.000", "3530.000"),
                                                     //                            new Scenario.SteerPoint("3400.000", "3530.000")
                                                     },
                                                     31000f,
                                                     600f,
                                                     new Scenario.ActiveThreat[] { new Scenario.ActiveThreat(1,
                                                                                                             new Scenario.SteerPoint("3105.000", "3355.000"),
                                                                                                             new Scenario.ActiveThreat.ActiveThreatEvent[] { new Scenario.ActiveThreat.ActiveThreatEvent(0, 5f, false),
                                                                                                                                                             new Scenario.ActiveThreat.ActiveThreatEvent(1, 15f, false)},
                                                                                                             0.5f,
                                                                                                             2f,
                                                                                                             7f,
                                                                                                             new Scenario.ActiveThreat.UserResponeToThreat[] {new Scenario.ActiveThreat.UserResponeToThreat(0, 100f, "", false, -1)})
                                                                                 },
                                                     new Scenario.ActiveMapCircle[] { new Scenario.ActiveMapCircle(0, new Scenario.SteerPoint("3345.000", "3540.000"))},
                                                     new int[] { 0, 1, 2},
                                                     false, false, 0
                                                     ));

        simulatorDatabase.trains = new List<Train>();


        return simulatorDatabase;

    }


}
