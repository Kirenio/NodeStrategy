using UnityEngine;
using System.Collections.Generic;

public class ConstructionSite : Building {
    Controls controls;
    string BuildingType;
    Building FinalBuilding;

    // Use this for initialization
    protected override void Awake () {
        base.Awake();
        string content = "";
        foreach (KeyValuePair<Resource, float> entry in Stored)
        {
            content += string.Format("{0} {1}; ", entry.Value, entry.Key);
        }
        Debug.LogWarning(content);
    }

	public void SetOrder(Controls value)
    {
        controls = value;
        BuildingType = controls.BuildingToBuild;
        SetFinalBuilding();
        Debug.Log(FinalBuilding.GetType().ToString());
        InventoryChangedInternal += CheckConditions;

        ResourcesIncoming = new Dictionary<Resource, float>(Stored);
        foreach (KeyValuePair<Resource, float> entry in Stored)
        {
            ResourcesIncoming[entry.Key] = 0;
        }
    }

	// Update is called once per frame
	void CheckConditions() {
        if (StoredAmount >= Capacity)
        {
            Instantiate(FinalBuilding,transform.position, FinalBuilding.transform.rotation);

            // (!) Implement pooling of Construction sites!

            //gameObject.SetActive(false);
            InventoryChangedInternal -= CheckConditions;
            Destroy(gameObject);
        }
	}

    void SetFinalBuilding()
    {
        switch(BuildingType)
        {
            case "Extractor":
                FinalBuilding = controls.ExtractorPrefab;
                Stored = new Dictionary<Resource, float>(GameData.Prices["Extractor"]);
                break;
            case "Silo":
                FinalBuilding = controls.SiloPrefab;
                Stored = new Dictionary<Resource, float>(GameData.Prices["Silo"]);
                break;
            case "Manufactory":
                FinalBuilding = controls.ManufactoryPrefab;
                Stored = new Dictionary<Resource, float>(GameData.Prices["Manufactory"]);
                break;
            case "Warehouse":
                FinalBuilding = controls.WarehousePrefab;
                Stored = new Dictionary<Resource, float>(GameData.Prices["Warehouse"]);
                break;
            case "BlackSandExtractor":
                FinalBuilding = controls.BlackSandExtractorPreafab;
                Stored = new Dictionary<Resource, float>(GameData.Prices["BlackSandExtractor"]);
                break;
            default:
                Debug.LogWarning("FinalBuilding was not set!");
                break;
        }

        // Calculating StoredAmount (Total resources we need to deliver)
        foreach(KeyValuePair<Resource, float> pair in Stored)
        {
            Capacity += pair.Value;
        }
    }

    public override float Ship(Resource type, float amount)
    {
        //if (Stored.ContainsKey(type))
        //{
        //    if (amount < Stored[type])
        //    {
        //        float rValue = Stored[type];
        //        StoredAmount += Stored[type];
        //        Stored[type] = 0;
        //        OnInventoryChanged();
        //        return amount;
        //    }
        //    else
        //    {
        //        Stored[type] += amount;
        //        StoredAmount -= amount;
        //        OnInventoryChanged();
        //        return amount;
        //    }
        //}
        //else
            return 0;
    }

    public override float Recieve(Cargo cargo)
    {
        if (Stored.ContainsKey(cargo.Type))
        {
            if (Stored[cargo.Type] - cargo.Amount >= 0)
            {
                Debug.Log(Stored[cargo.Type] - cargo.Amount + " " + cargo.Type + " was higher or equal to 0");
                Stored[cargo.Type] -= cargo.Amount;
                StoredAmount += cargo.Amount;

                if (LogActivity) LogRecieved(cargo);

                OnInventoryChanged();
                OnInventoryChangedInternal();
                return 0;
            }
            else if (Stored[cargo.Type] - cargo.Amount < 0)
            {
                Debug.Log(Stored[cargo.Type] - cargo.Amount + " " + cargo.Type + " was less than 0");
                float rValue = cargo.Amount - Stored[cargo.Type];
                StoredAmount += Stored[cargo.Type];

                if (LogActivity) LogRecieved(cargo.Type, Stored[cargo.Type]);

                Stored[cargo.Type] = 0;

                OnInventoryChanged();
                OnInventoryChangedInternal();
                return rValue;
            }
        }
        return cargo.Amount;
    }

    public override float Quota(Resource resource, float amount)
    {
        Debug.LogWarning("Calculating Quota!");
        if (Stored.ContainsKey(resource))
        {
            Debug.LogWarning(string.Format("Key:" + resource + " s:"+ Stored[resource]+" a:" + amount + " ra:"+ResourcesIncoming[resource]));
            return Stored[resource] + amount - ResourcesIncoming[resource];
        }
        else
        {
            Debug.LogWarning(name + " Resource Key was missing!");
            return 0;
        }
        Debug.LogWarning("Did not catch!");
    }

    public override string GetContentString()
    {
        string content = "Stored materials\n";
        foreach (KeyValuePair<Resource, float> entry in Stored)
        {
            Debug.Log(entry.Value + " / " + GameData.Prices[BuildingType][entry.Key]  + " "+ entry.Key);
            if (GameData.Prices[BuildingType][entry.Key] - entry.Value > 0)
                content += string.Format("{0} {1}\n", (GameData.Prices[BuildingType][entry.Key] - entry.Value), entry.Key);
        }
        return content;
    }

    public override string GetStatsString()
    {
        string rValue = string.Format("HP:\t{0}/{1}\nConstruction\nProgress:\t{2}/{3}\nRequired\n", CurrentHealth, MaxHealth, StoredAmount, Capacity);
        foreach (KeyValuePair<Resource, float> entry in Stored)
        {
            if (entry.Value > 0)
                rValue += string.Format("{0} {1}\n", entry.Value, entry.Key);
        }
        return rValue;
    }
}
