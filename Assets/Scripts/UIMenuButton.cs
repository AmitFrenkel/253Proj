using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIMenuButton : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool isButtonSelected;
    public string buttonCategory;
    public string buttonName;
    public int buttonIndex;
    public CollapsibleButtonsMainVerticalPanel collapsibleButtonsMainVerticalPanel;

    public void initUIMenuButton()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        isButtonSelected = false;
        updateButtonColor();
    }

    public void fillContentInButton(string category, string name, int index, CollapsibleButtonsMainVerticalPanel containerPanel, bool isButtonInitSelected)
    {
        buttonCategory = category;
        buttonName = name;
        buttonIndex = index;
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        collapsibleButtonsMainVerticalPanel = containerPanel;
        isButtonSelected = isButtonInitSelected;
        updateButtonColor();
    }

    public void setButtonName(string name)
    {
        buttonName = name;
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
    }

    public int getIndexOfMeuButton()
    {
        return buttonIndex;
    }

    private void updateButtonColor()
    {
        this.GetComponent<Image>().color = isButtonSelected ? UIViewConfigurations.selectedButtonColor : UIViewConfigurations.notSelectedButtonColor;
    }

    public void selectButton()
    {
        if (!isButtonSelected)
        {
            isButtonSelected = true;
            updateButtonColor();
            collapsibleButtonsMainVerticalPanel.buttonFromGroupSelected(this);
            collapsibleButtonsMainVerticalPanel.mainUIManager.transform.GetComponent<MainContentManager>().menuButtonSelected(this);
        }
    }

    public bool getIfButtonSelected()
    {
        return isButtonSelected;
    }

    public void unselectButton()
    {
        isButtonSelected = false;
        updateButtonColor();
    }

    public void setHeightOfButton(float newHeight)
    {
        rectTransform.offsetMin = new Vector2(UIViewConfigurations.MenuUIButtonPadding, newHeight);
        rectTransform.offsetMax = new Vector2(-UIViewConfigurations.MenuUIButtonPadding, newHeight);
        rectTransform.sizeDelta = new Vector2(-2f* UIViewConfigurations.MenuUIButtonPadding, UIViewConfigurations.MenuUIButtonHeight);
    }
}
