using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    private bool isWaitingToSave;
    public MainContentManager mainContentManager;

    public void initSaveButton()
    {
        setAsSaved();
    }

    public void setAsNeedToSave()
    {
        isWaitingToSave = true;
        this.transform.GetComponent<Image>().color = new Color(0.033f, 0.675f, 1f);
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Save";
    }

    public void setAsSaved()
    {
        isWaitingToSave = false;
        this.transform.GetComponent<Image>().color = UIViewConfigurations.selectedButtonColor;
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Saved!";
    }

    public void saveElement()
    {
        if (isWaitingToSave)
        {
            mainContentManager.saveElement();
            setAsSaved();
        }
            
    }
}
