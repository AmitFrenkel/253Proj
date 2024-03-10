using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditorInputDropDown : EditorLine
{
    public TextMeshProUGUI displayNameText;
    public TMP_Dropdown dropDownField;
    private EditorInputList editorInputList;
    public GameObject deleteButton;
    private List<IndexAndName> indexAndNames;

    private class IndexAndName
    {
        public int index;
        public string name;

        public IndexAndName(int index, string name)
        {
            this.index = index;
            this.name = name;
        }
    }

    public void initEditorIndputDropDown(MainContentManager mainContentManager, float xOffset)
    {
        this.mainContentManager = mainContentManager;
        editorElementHeight = -UIViewConfigurations.dataLineHeight;
        this.xOffset = xOffset;
        indexAndNames = new List<IndexAndName>();
        dropDownField.options = new List<TMP_Dropdown.OptionData>();  
    }

    public void buildEditorIndputDropDownBySimulatorCategory(MainContentManager.SimulatorTypes simulatorCategory, int selectedValue)
    {
        SimulatorDatabase simulatorDatabase = mainContentManager.getSimulatorDatabase();
        List<SimulatorElement> simulatorElements = null;
        switch (simulatorCategory)
        {
            case MainContentManager.SimulatorTypes.SystemVersion: simulatorElements = simulatorDatabase.systemVersions.ConvertAll(x => (SimulatorElement)x); break;
            case MainContentManager.SimulatorTypes.Threat: simulatorElements = simulatorDatabase.threats.ConvertAll(x => (SimulatorElement)x); break;
            case MainContentManager.SimulatorTypes.MapCircle: simulatorElements = simulatorDatabase.mapCircles.ConvertAll(x => (SimulatorElement)x); break;
            case MainContentManager.SimulatorTypes.UserResponse: simulatorElements = simulatorDatabase.userResponses.ConvertAll(x => (SimulatorElement)x); break;
            case MainContentManager.SimulatorTypes.EducationalScreen: simulatorElements = simulatorDatabase.educationalScreens.ConvertAll(x => (SimulatorElement)x); break;
            case MainContentManager.SimulatorTypes.Scenario: simulatorElements = simulatorDatabase.scenarios.ConvertAll(x => (SimulatorElement)x); break;
            case MainContentManager.SimulatorTypes.Train: simulatorElements = simulatorDatabase.trains.ConvertAll(x => (SimulatorElement)x); break;
            default: Debug.Log("could not find type!"); break;
        }

        List<int> existIndices = simulatorDatabase.getListOfExistIndices(simulatorElements);
        int selectedDropDownValue = -1;
        for (int i = 0; i < existIndices.Count; i++)
        {
            int existIndex = existIndices[i];
            string elementName = simulatorDatabase.getSimulatorElementByIndex(simulatorElements, existIndex).getName();
            indexAndNames.Add(new IndexAndName(existIndex, elementName));
            dropDownField.options.Add(new TMP_Dropdown.OptionData(elementName + " (index " + existIndex + ")"));
            if (existIndex == selectedValue)
                selectedDropDownValue = i;
        }
        if (selectedDropDownValue != -1)
            dropDownField.value = selectedDropDownValue;
    }

    public void buildEditorIndputDropDownBySimulatorClassElementList(string[] sourceList)
    {
        for (int i = 0; i < sourceList.Length; i++)
        {
            indexAndNames.Add(new IndexAndName(i, sourceList[i]));
            dropDownField.options.Add(new TMP_Dropdown.OptionData(sourceList[i]));
        }
        if (sourceList.Length > 0)
            dropDownField.value = 0;
    }

    public void setDisplayName(string name)
    {
        displayNameText.text = name;
    }

    public string getValue()
    {
        if (indexAndNames.Count > 0)
            return indexAndNames[dropDownField.value].index.ToString();
        return "-1";
    }

    public void assignToEditorInputList(EditorInputList newList)
    {
        editorInputList = newList;
        deleteButton.SetActive(true);
    }

    public void deleteInputLine()
    {
        if (editorInputList != null)
            editorInputList.deleteElementInList(this);
    }

    public override void reorderEditorElement()
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, baseHeight);
    }

}
