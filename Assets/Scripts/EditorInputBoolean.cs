using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EditorInputBoolean : EditorLine
{
    public TextMeshProUGUI displayNameText;
    public Toggle toggle;
    private EditorInputList editorInputList;
    public GameObject deleteButton;

    public void initEditorInputBoolean(MainContentManager mainContentManager, float xOffset)
    {
        this.mainContentManager = mainContentManager;
        this.xOffset = xOffset;
        editorElementHeight = -UIViewConfigurations.dataLineHeight;
    }

    public void setDisplayName(string name)
    {
        displayNameText.text = name;
    }
    public void setValue(bool value)
    {
        toggle.isOn = value;
    }
    public bool getValue()
    {
        return toggle.isOn;
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
        toggle.interactable = false;
    }

    public override void reorderEditorElement()
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, baseHeight);
    }

}

