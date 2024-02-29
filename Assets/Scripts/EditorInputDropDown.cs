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

    public void initEditorIndputDropDown(MainContentManager.SimulatorTypes simulatorCategory, int selectedValue, MainContentManager mainContentManager)
    {
        this.mainContentManager = mainContentManager;
        indexAndNames = new List<IndexAndName>();
        dropDownField.options = new List<TMP_Dropdown.OptionData>();
        SimulatorDatabase simulatorDatabase = mainContentManager.getSimulatorDatabase();
        List<SimulatorElement> simulatorElements = null;
        switch (simulatorCategory)
        {
            case MainContentManager.SimulatorTypes.Threat: simulatorElements = simulatorDatabase.threats.ConvertAll(x => (SimulatorElement)x); break;
            default: Debug.Log("could not find type!"); break;
        }

        List<int> existIndices = simulatorDatabase.getListOfExistIndices(simulatorElements);
        int selectedDropDownValue = -1;
        for (int i=0; i<existIndices.Count; i++)
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
        else
            Debug.Log("could not find selected row in dropdown!");

    }

    public void setDisplayName(string name)
    {
        displayNameText.text = name;
    }

    public string getValue()
    {
        return indexAndNames[dropDownField.value].index.ToString();
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

    public void lockEditorInputLineFromEdit()
    {
        //inputField.interactable = false;
    }

}
