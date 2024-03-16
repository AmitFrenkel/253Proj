using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingMenu : MonoBehaviour
{
    protected MapView mapView;
    public TextMeshProUGUI header;

    public void setHeaderText(string text)
    {
        header.text = text;
    }
}
