
using UnityEngine;

public class Silo : Building
{
    public int Sections = 1;
    float SectionSize;

    protected override void Awake()
    {
        base.Awake();
        SectionSize = SectionCalculateSize();
    }

    public override float Recieve(Cargo cargo)
    {
        if (Stored.ContainsKey(cargo.Type) && Stored[cargo.Type] != 0)
        {
            if (Stored[cargo.Type] + cargo.Amount <= SectionSize)
            {
                Stored[cargo.Type] += cargo.Amount;
                StoredAmount += cargo.Amount;

                if (LogActivity) LogRecieved(cargo);

                OnInventoryChanged();
                return 0;
            }
            else
            {
                float spaceFree = SectionSize - Stored[cargo.Type];
                if(spaceFree > 0)
                {
                    Stored[cargo.Type] += spaceFree;
                    StoredAmount += spaceFree;

                    if (LogActivity) LogRecieved(cargo.Type, spaceFree);

                    OnInventoryChanged();
                    return cargo.Amount - spaceFree;
                }
                return cargo.Amount;
            }
        }
        else if (SectionsOccopied() < Sections)
        {
            if (Stored.ContainsKey(cargo.Type))
                Stored[cargo.Type] += cargo.Amount;
            else
                Stored.Add(cargo.Type, cargo.Amount);

            StoredAmount += cargo.Amount;

            if (LogActivity) LogRecieved(cargo);

            OnInventoryChanged();
            return 0;
        }
        return cargo.Amount;
    }

    int SectionsOccopied()
    {
        int value = 0;
        foreach(float entry in Stored.Values)
        {
            if (entry > 0) value++;
        }
        return value;
    }

    float SectionCalculateSize()
    {
        return Capacity / Sections;
    }
}
