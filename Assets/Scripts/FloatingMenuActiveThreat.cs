using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FloatingMenuActiveThreat : FloatingMenu, IPointerClickHandler
{
    private MapActiveThreat mapActiveThreat;
    public GameObject mainFloatingMenu;
    public GameObject editFloatingMenu;
    public MapEditorDropDown linkedThreatIndexDropdown;

    public void initFloatingMenuActiveThreat(MapScenarioManager mapScenarioManager, MapActiveThreat mapActiveThreat)
    {
        this.mapScenarioManager = mapScenarioManager;
        this.mapActiveThreat = mapActiveThreat;
    }

    public void editValues()
    {
        mainFloatingMenu.SetActive(false);
        editFloatingMenu.SetActive(true);
    }

    public void editThreatEvents()
    {
        mapActiveThreat.editThreatEvents();
    }

    public void editUserReponseToThreat()
    {
        
    }

    //public void editMapCircle()
    //{
    //    mainFloatingMenu.SetActive(false);
    //    editFloatingMenu.SetActive(true);

    //    linkedMapCircleIndexDropdown.initMapEditorIndputDropDown();
    //    linkedMapCircleIndexDropdown.buildEditorIndputDropDownBySimulatorCategory(mapScenarioManager.getSimulatorDatabase(), MainContentManager.SimulatorTypes.MapCircle, mapActiveCircle.getActiveMapCircleObject().mapCircleIndexLinkIndex);
    //}

    //public void duplicateMapCircle()
    //{
    //    mapScenarioManager.duplicateActiveMapCircle(mapActiveCircle);
    //    mapScenarioManager.closeAllFloatingMenus();
    //}

    //public void removeMapCircle()
    //{
    //    mapScenarioManager.removeActiveMapCircle(mapActiveCircle);
    //    mapScenarioManager.closeAllFloatingMenus();
    //}

    //public void linkedMapCircleChanged()
    //{
    //    mapActiveCircle.changeLinkedMapCircle(int.Parse(linkedMapCircleIndexDropdown.getValue()));
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
