using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLine : MonoBehaviour
{

    protected MainContentManager mainContentManager = null;
    protected float xOffset = 0f;
    protected float baseHeight = 0f;
    protected float editorElementHeight = 0f;

    //public void setMainContentManager(MainContentManager mainContentManager)
    //{
    //    this.mainContentManager = mainContentManager;
    //}

    public void reportEditorLineModified()
    {
        if (mainContentManager != null)
            mainContentManager.setEditorLineModified();
    }

    public void reportEditorLineChangedHeight()
    {
        if (mainContentManager != null)
            mainContentManager.setEditorLineChangedHeight();
    }

    public virtual void reorderEditorElement()
    {
        // implemented in any inheritanced class differently
    }

    public float getHeightOfEditorElement()
    {
        return editorElementHeight;
    }

    public void setBaseHeight(float newHeight)
    {
        baseHeight = newHeight;
    }

    public void setXOffset(float newOffset)
    {
        xOffset = newOffset;
    }
}
