public enum Resource { Empty, Crystal, BlackSand, DataCrystal, Minerals, Composites, Electronics }

[System.Serializable]
public struct Cargo
{
    public Resource Type;
    public float Amount;

    public Cargo(Resource type, float amount)
    {
        Type = type;
        Amount = amount;
    }

    public void ChangeAmount(float amount)
    {
        Amount -= amount;
    }
}
