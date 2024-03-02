using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollapsibleButtonsMainVerticalPanel : MainVerticalPanel
{
    public bool isExpended = true;
    public GameObject expandCollapseButton;
    private float collapsedWidth = UIViewConfigurations.collapedVerticalPanelWidth;
    public float expendedWidth = 0.4f;
    private float panelWidth;
    public TextMeshProUGUI collapsedTextNotSelected;
    public TextMeshProUGUI collapsedTextSelected;
    public List<UIMenuButton> menuButtons;
    public float buttonsStartHeight;
    public GameObject uiMenuButtonPrefab;
    private bool isAnyButtonSelected;


    public void initCollapsibleButtonsMainVerticalPanel()
    {
        isAnyButtonSelected = false;
        collapsedTextSelected.color = UIViewConfigurations.selectedButtonColor;
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        if (menuButtons != null)
            foreach (UIMenuButton loopUIMenuButton in menuButtons)
                if (loopUIMenuButton != null)
                    loopUIMenuButton.initUIMenuButton();
        if (menuButtons == null)
            menuButtons = new List<UIMenuButton>();
        updatePanelExpandOrCollapse();
        reorderUIMenuButtons();

    }

    public void expandOrCollapsePanel()
    {
        isExpended = !isExpended;
        updatePanelExpandOrCollapse();
        mainUIManager.rightCollapsableMainPanelsCollapedOrExpanded();
    }

    public void collapsePanel()
    {
        isExpended = false;
        updatePanelExpandOrCollapse();
        mainUIManager.rightCollapsableMainPanelsCollapedOrExpanded();
    }

    public void expandPanel()
    {
        isExpended = true;
        updatePanelExpandOrCollapse();
        mainUIManager.rightCollapsableMainPanelsCollapedOrExpanded();
    }

    private void updatePanelExpandOrCollapse()
    {
        panelWidth = isExpended ? expendedWidth : collapsedWidth;
        expandCollapseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (isExpended ? "-" : "+");
        if (isExpended)
        {
            contectHolder.SetActive(true);
            collapsedTextSelected.gameObject.SetActive(false);
            collapsedTextNotSelected.gameObject.SetActive(false);
        }
        else
        {
            contectHolder.SetActive(false);
            if (isAnyButtonSelected)
                collapsedTextSelected.gameObject.SetActive(true);
            else
                collapsedTextNotSelected.gameObject.SetActive(true);
        }
    }

    public void buttonFromGroupSelected(UIMenuButton selectedButton)
    {
        isAnyButtonSelected = true;
        collapsedTextSelected.text = selectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        if (isAnyButtonSelected)
        {
            foreach (UIMenuButton loopUIButton in menuButtons)
            {
                if (loopUIButton != selectedButton)
                    loopUIButton.unselectButton();
            }
        }
    }

    public float getPanelWidth()
    {
        return panelWidth;
    }

    public void addUIMenuButton(string buttonCategory, string buttonName, int buttonIndex, bool isSelected)
    {
        if (isSelected && isAnyButtonSelected)
        {
            foreach (UIMenuButton loopUIButton in menuButtons)
                loopUIButton.unselectButton();
        }

        GameObject newUIMenuButtonGameObject = Instantiate(uiMenuButtonPrefab) as GameObject;
        newUIMenuButtonGameObject.transform.parent = contectHolder.transform;
        UIMenuButton newUIMenuButton = newUIMenuButtonGameObject.GetComponent<UIMenuButton>();
        newUIMenuButton.initUIMenuButton();
        newUIMenuButton.fillContentInButton(buttonCategory, buttonName, buttonIndex, this, isSelected);
        menuButtons.Add(newUIMenuButton);
        if (isSelected)
            isAnyButtonSelected = true;
    }

    public void editButtonName(int buttonIndex, string buttonNewName)
    {
        foreach (UIMenuButton loopUIButton in menuButtons)
            if (loopUIButton.getIndexOfMeuButton() == buttonIndex)
                loopUIButton.setButtonName(buttonNewName);
    }

    public void removeUIMenuButton(int buttonIndex)
    {
        bool isRemovedButtonSelected = false;
        for (int i=0; i<menuButtons.Count; i++)
        {
            if (menuButtons[i].getIndexOfMeuButton() == buttonIndex)
            {
                isRemovedButtonSelected = menuButtons[i].getIfButtonSelected();
                Destroy(menuButtons[i].gameObject);
                menuButtons.RemoveAt(i);
                break;
            }
        }
        reorderUIMenuButtons();

        if (isRemovedButtonSelected)
        {
            isAnyButtonSelected = false;
            updatePanelExpandOrCollapse();
        }
    }

    public void reorderUIMenuButtons()
    {
        for (int i=0; i< menuButtons.Count; i++)
        {
            float loopHeight = buttonsStartHeight - (UIViewConfigurations.firstHeightOfButtonsInCollapsiblePanels + i * UIViewConfigurations.spacingBetweenButtonsInCollapsiblePanels);
            menuButtons[i].setHeightOfButton(loopHeight);
        }
    }

    public void clearButtonsForPanel()
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            Destroy(menuButtons[i].gameObject);
        }
        
        menuButtons = new List<UIMenuButton>();
        isAnyButtonSelected = false;
        updatePanelExpandOrCollapse();
    }
}
