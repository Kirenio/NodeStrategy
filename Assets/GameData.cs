using UnityEngine;
using System.Collections.Generic;

public static class GameData
{
    public static Dictionary<string, Dictionary<Resource, float>> Prices = new Dictionary<string, Dictionary<Resource, float>>
    {
        {
            "Extractor", new Dictionary<Resource, float> { { Resource.Composites, 45 }, { Resource.Electronics, 15 } }
        },
        {
            "Silo", new Dictionary<Resource, float> { { Resource.Composites, 50 }, { Resource.Electronics, 5 } }
        },
        {
            "Manufactory", new Dictionary<Resource, float> { { Resource.Composites, 100 }, { Resource.Electronics, 30 } }
        },
        {
            "Warehouse", new Dictionary<Resource, float> { { Resource.Composites, 80 }, { Resource.Electronics, 20 } }
        },
        {
            "BlackSandExtractor", new Dictionary<Resource, float> { { Resource.Composites, 90 }, { Resource.Electronics, 25 } }
        },
        {
            "1", new Dictionary<Resource, float> { { Resource.Composites, 150 }, { Resource.Electronics, 5 } }
        },
        {
            "2", new Dictionary<Resource, float> { { Resource.Composites, 150 }, { Resource.Electronics, 5 } }
        },
        {
            "3", new Dictionary<Resource, float> { { Resource.Composites, 150 }, { Resource.Electronics, 5 } }
        },
    };
}
    