using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIViewConfigurations
{
    public static Color notSelectedButtonColor = new Color(0.8f, 0.8f, 0.8f);
    public static Color selectedButtonColor = new Color(0.7f, 0.9f, 0.6f);

    public static float collapedVerticalPanelWidth = 0.05f;
    public static Color collapsedTextColor = selectedButtonColor;

    public static float firstHeightOfButtonsInCollapsiblePanels = 50f;
    public static float spacingBetweenButtonsInCollapsiblePanels = 40f;

    public static float MenuUIButtonPadding = 10f;
    public static float MenuUIButtonHeight = 30f;

    public static float dataLineHeight = 35f;
    public static float dataListXOffset = 30f;

    public static float headerWidth = 209f;
    public static float spacingBetweenLinesHeight = 5f;
    public static float listHeaderWidth = 35f;


}

public class MainUIManager : MonoBehaviour
{
    public List<MainVerticalPanel> mainVerticalPanels;

    // Start is called before the first frame update
    void Start()
    {
        initMainUIManager();
        this.gameObject.GetComponent<MainContentManager>().initMainContentManager();
    }

    public void initMainUIManager()
    {
        foreach (MainVerticalPanel loopPanel in mainVerticalPanels)
        {
            if ((loopPanel.GetComponent<MainVerticalPanel>() as CollapsibleButtonsMainVerticalPanel) != null)
                loopPanel.GetComponent<CollapsibleButtonsMainVerticalPanel>().initCollapsibleButtonsMainVerticalPanel();
            else
                loopPanel.initMainVerticalPanel();
        }

        rightCollapsableMainPanelsCollapedOrExpanded();
    }

    public void rightCollapsableMainPanelsCollapedOrExpanded()
    {
        float loopMaxAnchorX = 1f;
        foreach (MainVerticalPanel loopPanel in mainVerticalPanels)
        {
            if ((loopPanel.GetComponent<MainVerticalPanel>() as CollapsibleButtonsMainVerticalPanel) != null)
            {
                CollapsibleButtonsMainVerticalPanel loopCollapsablePanel = loopPanel.GetComponent<CollapsibleButtonsMainVerticalPanel>();
                float loopWidth = loopCollapsablePanel.getPanelWidth();
                float loopMinAnchorX = loopMaxAnchorX - loopWidth;
                loopPanel.setXAnchorMinAndMax(loopMinAnchorX, loopMaxAnchorX);
                loopMaxAnchorX = loopMinAnchorX;
            }
            else
            {
                loopPanel.setXAnchorMinAndMax(0f, loopMaxAnchorX);
            }   
        }
    }
}
