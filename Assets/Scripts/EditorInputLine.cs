using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditorInputLine : EditorLine
{
    public TextMeshProUGUI displayNameText;
    public TMP_InputField inputField;
    private EditorInputList editorInputList;
    public GameObject deleteButton;

    public void initEditorInputLine(string dataType, MainContentManager mainContentManager, float xOffset)
    {
        this.mainContentManager = mainContentManager;
        this.xOffset = xOffset;
        switch (dataType)
        {
            case "int": inputField.contentType = TMP_InputField.ContentType.IntegerNumber; break;
            case "float": inputField.contentType = TMP_InputField.ContentType.DecimalNumber; break;
            default: inputField.contentType = TMP_InputField.ContentType.Standard; break;
        }
        editorElementHeight = -UIViewConfigurations.dataLineHeight;
    }

    public void setDisplayName(string name)
    {
        displayNameText.text = name;
    }
    public void setValue(string value)
    {
        inputField.text = value;
    }
    public string getValue()
    {
        return inputField.text;
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
        inputField.interactable = false;
    }

    public override void reorderEditorElement()
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, baseHeight);
    }

}
