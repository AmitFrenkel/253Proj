using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditorInputList : EditorLine
{
    public TextMeshProUGUI displayNameText;
    public GameObject editorLinePrefab;
    public GameObject editorDropdownPrefab;
    private List<EditorLine> editorLines;
    private const string defaultElementName = "Element";
    private string listDataType;
    private bool isListOfDropdownIndexes;
    public RectTransform addButton;
    public RectTransform listHeader;
    MainContentManager.SimulatorTypes listSimulatorType;


    public void initEditorInputList(string dataType, MainContentManager mainContentManager, float xOffset)
    {
        this.mainContentManager = mainContentManager;
        this.xOffset = xOffset;
        editorLines = new List<EditorLine>();
        //totalHeight = 0f;
        listDataType = dataType;
        isListOfDropdownIndexes = false;
    }

    public void setAsListOfDropdownIndices(MainContentManager.SimulatorTypes listSimulatorType)
    {
        isListOfDropdownIndexes = true;
        this.listSimulatorType = listSimulatorType;
    }

    public void setDisplayName(string name)
    {
        displayNameText.text = name;
    }

    public void addEditorInputLineToList(string value)
    {
        GameObject newEditorInputLine = Instantiate(editorLinePrefab) as GameObject;
        newEditorInputLine.transform.parent = this.transform;
        newEditorInputLine.GetComponent<EditorInputLine>().initEditorInputLine(listDataType, mainContentManager, xOffset);
        newEditorInputLine.GetComponent<EditorInputLine>().setDisplayName(defaultElementName);
        newEditorInputLine.GetComponent<EditorInputLine>().setValue(value);
        newEditorInputLine.GetComponent<EditorInputLine>().assignToEditorInputList(this);
        editorLines.Add(newEditorInputLine.GetComponent<EditorLine>());
    }

    public void addEditorInputDropdownToList(int selectedIndex)
    {
        GameObject newEditorDropDown = Instantiate(editorDropdownPrefab) as GameObject;
        newEditorDropDown.transform.parent = this.transform;
        newEditorDropDown.GetComponent<EditorInputDropDown>().initEditorIndputDropDown(listSimulatorType, selectedIndex, mainContentManager, 0f);
        newEditorDropDown.GetComponent<EditorInputDropDown>().setDisplayName((editorLines.Count+1).ToString());
        newEditorDropDown.GetComponent<EditorInputDropDown>().assignToEditorInputList(this);
        editorLines.Add(newEditorDropDown.GetComponent<EditorLine>());
    }

    public void addElement()
    {
        if (isListOfDropdownIndexes)
            addEditorInputDropdownToList(0);
        else
            addEditorInputLineToList("");
        reorderLinesInList();
        reportEditorLineModified();
        reportEditorLineChangedHeight();
    }

    public void deleteElementInList(EditorLine editorLineToDelete)
    {
        editorLines.Remove(editorLineToDelete);
        Destroy(editorLineToDelete.gameObject);
        reorderLinesInList();
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
        for (int i=0; i<editorLines.Count; i++)
        {
            editorLines[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, loopHeight);
            if (isListOfDropdownIndexes)
                (editorLines[i] as EditorInputDropDown).setDisplayName((i + 1).ToString());
            else
                (editorLines[i] as EditorInputLine).setDisplayName((i + 1).ToString());
            loopHeight -= UIViewConfigurations.dataLineHeight;
        }
        addButton.anchoredPosition = new Vector2(-UIViewConfigurations.headerWidth, loopHeight);
        loopHeight -= UIViewConfigurations.dataLineHeight;
        listHeader.sizeDelta = new Vector2(listHeader.sizeDelta.x, -loopHeight - UIViewConfigurations.spacingBetweenLinesHeight);
        listHeader.anchoredPosition = new Vector2(0f, 0f);
        editorElementHeight = loopHeight;
    }

    public int getNumberOfElementsInList()
    {
        return editorLines.Count;
    }

    public EditorLine getEditorLineInIndex(int index)
    {
        return editorLines[index];
    }

    public string getValueInLineIndex(int lineIndex)
    {
        if (isListOfDropdownIndexes)
            return (editorLines[lineIndex] as EditorInputDropDown).getValue();
        else
            return (editorLines[lineIndex] as EditorInputLine).getValue();
    }
}
