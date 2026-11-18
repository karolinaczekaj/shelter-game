namespace Schron.Core
{
    // Nowe wartości dopisujemy tylko NA KOŃCU listy.

    public enum ResourceType
    {
        Food,
        Water,
        Energy,
        Scrap,
        Money,
        Morale
    }

    public enum ResidentState
    {
        Idle,
        Working,
        Moving,
        Rest,
        Recovery,
        OnExpedition
    }
}
