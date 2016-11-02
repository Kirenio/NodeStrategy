using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public bool CanBuild { get; private set; }
    public Controls controls;
    public Text StructuresList;
    string Name;
    List<Building> OwnedStructures = new List<Building>();

    void Awake()
    {
        controls.BuildingCreated += OnBuildingCreated;
    }

    void OnBuildingCreated(Building building)
    {
        OwnedStructures.Add(building);
        StructuresListUpdate();
    }

    void StructuresListUpdate()
    {
        StructuresList.text = "";
        for (int i = 0; i < OwnedStructures.Count; i++)
        {
            StructuresList.text += (i + 1) + "\t" + OwnedStructures[i].name + "\n";
        }
    }
}
