using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapThreatEvent : MonoBehaviour
{
    public TextMeshProUGUI desc;

    public void setText(string text)
    {
        desc.text = text;
    }
}
