using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using TMPro;

public class EditorEntity : EditorLine
{

    private SimulatorClass linkedSimulatorClass;
    private List<EditorLine> editorLines;
    private float startHeight;
    private float loopHeight;
    private const float editorLineHeight = 40f;
    private bool isEditorEntityPartOfAList;
    private EditorEntitiesList editorEntitiesList;
    public GameObject deleteButton;
    public RectTransform listHeader;
    public TextMeshProUGUI listHeaderText;

    public GameObject editorInputLinePrefab;
    public GameObject editorInputListPrefab;
    public GameObject editorEntityListPrefab;

    public void initEditorEntity(SimulatorClass SimulatorClass, MainContentManager mainContentManager, float xOffset, EditorEntitiesList editorEntitiesList)
    {
        linkedSimulatorClass = SimulatorClass;
        this.mainContentManager = mainContentManager;
        this.xOffset = xOffset;
        editorLines = new List<EditorLine>();
        startHeight = 0f;
        loopHeight = startHeight;
        isEditorEntityPartOfAList = (editorEntitiesList != null);
        if (isEditorEntityPartOfAList)
        {
            this.editorEntitiesList = editorEntitiesList;
            listHeader.gameObject.SetActive(true);
        }
            
        builtElement();
    }

    private void builtElement()
    {
        FieldInfo[] props = linkedSimulatorClass.GetType().GetFields();
        foreach (FieldInfo prop in props)
        {
            if (prop.GetValue(linkedSimulatorClass) != null)
            {
                if (prop.GetValue(linkedSimulatorClass).GetType() == typeof(int))
                {
                    EditorInputLine newEditorInputLine = addEditorInputLine(prop.Name, prop.GetValue(linkedSimulatorClass).ToString(), "int");
                    if (prop.Name.Contains("Index"))
                    {
                        newEditorInputLine.lockEditorInputLineFromEdit();
                    }
                }
                else if (prop.GetValue(linkedSimulatorClass).GetType() == typeof(string))
                    addEditorInputLine(prop.Name, prop.GetValue(linkedSimulatorClass).ToString(), "string");
                else if (prop.GetValue(linkedSimulatorClass).GetType() == typeof(float))
                    addEditorInputLine(prop.Name, prop.GetValue(linkedSimulatorClass).ToString(), "float");
                else if (prop.GetValue(linkedSimulatorClass).GetType().IsArray)
                {
                    if (prop.GetValue(linkedSimulatorClass).GetType().GetElementType() == typeof(int))
                    {
                        if (prop.Name.Contains("Indexes"))
                        {
                            MainContentManager.SimulatorTypes simulatorType = MainContentManager.SimulatorTypes.SystemVersion;
                            if (prop.Name.Contains("threat"))
                                simulatorType = MainContentManager.SimulatorTypes.Threat;
                            int[] arr = prop.GetValue(linkedSimulatorClass) as int[];
                            addEditorIndexInputDropdownList(prop.Name, simulatorType, arr);
                        }
                        else
                        {
                            int[] arr = prop.GetValue(linkedSimulatorClass) as int[];
                            string[] arr_strings = new string[arr.Length];
                            for (int i = 0; i < arr.Length; i++)
                                arr_strings[i] = arr[i].ToString();
                            addEditorInputList(prop.Name, arr_strings, "int");
                        }

                    }
                    else if (prop.GetValue(linkedSimulatorClass).GetType().GetElementType() == typeof(string))
                    {
                        string[] arr_strings = prop.GetValue(linkedSimulatorClass) as string[];
                        addEditorInputList(prop.Name, arr_strings, "string");
                    }
                    else if (prop.GetValue(linkedSimulatorClass).GetType().GetElementType().IsSubclassOf(typeof(SimulatorClass)))
                    {
                        SimulatorClass[] arr = prop.GetValue(linkedSimulatorClass) as SimulatorClass[];
                        addEditorEntitiesList(prop.Name, arr, prop.GetValue(linkedSimulatorClass).GetType().GetElementType());
                    }
                    
                    else
                        Debug.Log("Type not found: "+prop.GetValue(linkedSimulatorClass).GetType().ToString());
                }
                
            }
        }
    }

    private EditorInputLine addEditorInputLine(string name, string value, string dataType)
    {
        GameObject newEditorInputLine = Instantiate(editorInputLinePrefab) as GameObject;
        newEditorInputLine.transform.parent = this.transform;
        newEditorInputLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, loopHeight);
        newEditorInputLine.GetComponent<EditorInputLine>().initEditorInputLine(dataType, mainContentManager, (isEditorEntityPartOfAList ? -UIViewConfigurations.listHeaderWidth : 0f));
        newEditorInputLine.GetComponent<EditorInputLine>().setDisplayName(name);
        newEditorInputLine.GetComponent<EditorInputLine>().setValue(value);
        editorLines.Add(newEditorInputLine.GetComponent<EditorLine>());
        loopHeight -= editorLineHeight;
        return newEditorInputLine.GetComponent<EditorInputLine>();
    }

    private EditorEntitiesList addEditorEntitiesList(string name, SimulatorClass[] SimulatorClasss, Type elementsType)
    {
        GameObject newEditorEntitiesList = Instantiate(editorEntityListPrefab) as GameObject;
        newEditorEntitiesList.transform.parent = this.transform;
        //newEditorEntitiesList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, loopHeight);
        newEditorEntitiesList.GetComponent<EditorEntitiesList>().initEditorEntitiesList(SimulatorClasss, elementsType, mainContentManager, (isEditorEntityPartOfAList ? -UIViewConfigurations.listHeaderWidth : 0f));
        newEditorEntitiesList.GetComponent<EditorEntitiesList>().setDisplayName(name);
        editorLines.Add(newEditorEntitiesList.GetComponent<EditorLine>());
        loopHeight -= editorLineHeight * (SimulatorClasss.Length + 1);
        return newEditorEntitiesList.GetComponent<EditorEntitiesList>();
    }

    private EditorInputList addEditorInputList(string name, string[] values, string dataType)
    {
        GameObject newEditorInputList = Instantiate(editorInputListPrefab) as GameObject;
        newEditorInputList.transform.parent = this.transform;
        //newEditorInputList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, loopHeight);
        newEditorInputList.GetComponent<EditorInputList>().initEditorInputList(dataType, mainContentManager, (isEditorEntityPartOfAList ? -UIViewConfigurations.listHeaderWidth : 0f));
        newEditorInputList.GetComponent<EditorInputList>().setDisplayName(name);
        for (int i=0; i<values.Length; i++)
        {
            newEditorInputList.GetComponent<EditorInputList>().addEditorInputLineToList(values[i]);
        }
        editorLines.Add(newEditorInputList.GetComponent<EditorLine>());
        loopHeight -= editorLineHeight*(values.Length + 1);
        return newEditorInputList.GetComponent<EditorInputList>();
    }

    private EditorInputList addEditorIndexInputDropdownList(string name, MainContentManager.SimulatorTypes simulatorCategory, int[] values)
    {
        GameObject newEditorInputList = Instantiate(editorInputListPrefab) as GameObject;
        newEditorInputList.transform.parent = this.transform;
        //newEditorInputList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, loopHeight);
        newEditorInputList.GetComponent<EditorInputList>().initEditorInputList("int", mainContentManager, (isEditorEntityPartOfAList ? -UIViewConfigurations.listHeaderWidth : 0f));
        newEditorInputList.GetComponent<EditorInputList>().setAsListOfDropdownIndices(simulatorCategory);
        newEditorInputList.GetComponent<EditorInputList>().setDisplayName(name);
        for (int i = 0; i < values.Length; i++)
        {
            newEditorInputList.GetComponent<EditorInputList>().addEditorInputDropdownToList(values[i]);
        }
        editorLines.Add(newEditorInputList.GetComponent<EditorLine>());
        loopHeight -= editorLineHeight * (values.Length + 1);
        return newEditorInputList.GetComponent<EditorInputList>();
    }

    public void saveElement()
    {
        FieldInfo[] props = linkedSimulatorClass.GetType().GetFields();
        if (props.Length == editorLines.Count)
        {
            for (int propIndex=0; propIndex < props.Length; propIndex++)
            {
                FieldInfo prop = props[propIndex];
                EditorLine editorLine = editorLines[propIndex];
                if (prop.GetValue(linkedSimulatorClass) != null)
                {
                    if (prop.GetValue(linkedSimulatorClass).GetType() == typeof(int))
                        prop.SetValue(linkedSimulatorClass, Convert.ToInt32((editorLine as EditorInputLine).getValue()));
                    else if (prop.GetValue(linkedSimulatorClass).GetType() == typeof(string))
                        prop.SetValue(linkedSimulatorClass, (editorLine as EditorInputLine).getValue());
                    else if (prop.GetValue(linkedSimulatorClass).GetType() == typeof(float))
                        prop.SetValue(linkedSimulatorClass, float.Parse((editorLine as EditorInputLine).getValue()));
                    else if (prop.GetValue(linkedSimulatorClass).GetType().IsArray)
                    {
                        
                        if (prop.GetValue(linkedSimulatorClass).GetType().GetElementType() == typeof(int))
                        {
                            EditorInputList editorInputList = editorLine as EditorInputList;
                            int elementsInList = editorInputList.getNumberOfElementsInList();
                            int[] arr = new int[elementsInList];
                            for (int i = 0; i < elementsInList; i++)
                                arr[i] = Convert.ToInt32(editorInputList.getValueInLineIndex(i));

                            prop.SetValue(linkedSimulatorClass, arr);
                        }
                        else if (prop.GetValue(linkedSimulatorClass).GetType().GetElementType() == typeof(string))
                        {
                            EditorInputList editorInputList = editorLine as EditorInputList;
                            int elementsInList = editorInputList.getNumberOfElementsInList();
                            string[] arr = new string[elementsInList];
                            for (int i = 0; i < elementsInList; i++)
                                arr[i] = editorInputList.getValueInLineIndex(i);
                            prop.SetValue(linkedSimulatorClass, arr);
                        }
                        else if (prop.GetValue(linkedSimulatorClass).GetType().GetElementType().IsSubclassOf(typeof(SimulatorClass)))
                        {
                            (editorLine as EditorEntitiesList).saveEditorEntities();
                            SimulatorClass[] resultArr = (editorLine as EditorEntitiesList).GetSimulatorClassesArr();
                            switch ((editorLine as EditorEntitiesList).getSimulatorClassElementsTypeToString())
                            {
                                case "Threat+ThreatLock": prop.SetValue(linkedSimulatorClass, Array.ConvertAll(resultArr, item => (Threat.ThreatLock)item)); break;
                                default: Debug.Log("Add new type to switch case : " + (editorLine as EditorEntitiesList).getSimulatorClassElementsTypeToString()); break;
                            }  
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Could not save! unbalanced props (" + props.Length + ") vs editor lines (" + editorLines.Count + ")");
        }
    }

    public override void reorderEditorElement()
    {
        this.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, baseHeight);
        float loopHeight = 0f;
        foreach (EditorLine loopEditorLine in editorLines)
        {
            loopEditorLine.setBaseHeight(loopHeight);
            loopEditorLine.reorderEditorElement();
            loopHeight += loopEditorLine.getHeightOfEditorElement();
        }
        if (isEditorEntityPartOfAList)
        {
            listHeader.sizeDelta = new Vector2(listHeader.sizeDelta.x, -loopHeight - UIViewConfigurations.spacingBetweenLinesHeight);
            listHeader.anchoredPosition = new Vector2(0f, 0f);
        }
        
        editorElementHeight = loopHeight;
    }

    public void setNameInList(string name)
    {
        listHeaderText.text = name;
    }

    public void deleteEditorEntity()
    {
        if (editorEntitiesList != null)
            editorEntitiesList.deleteEditorEntityInList(this);
    }

    public SimulatorClass getLinkedSimulatorClass()
    {
        return linkedSimulatorClass;
    }
}
