namespace Schron.Core
{
    // ZASADA: nowe wartości dopisujemy tylko NA KOŃCU listy.

    public enum ResourceType
    {
        Food,           // jedzenie (farma)
        Water,          // woda (uzdatnianie wody)
        Energy,         // energia (generator)
        Scrap,          // złom (kopalnia czasem, później wyprawy)
        Ore,            // ruda (kopalnia)
        Components,     // komponenty (warsztat: złom + ruda)
        Morale          // morale schronu = średnia morale mieszkańców, liczone automatycznie
    }

    public enum RoomType
    {
        Quarters,       // kwatery
        Farm,           // farma
        WaterPlant,     // uzdatnianie wody
        Generator,      // generator
        Mine,           // kopalnia
        Workshop,       // warsztat
        Infirmary       // lecznica
    }

    public enum StatType
    {
        Strength,       // siła: kopalnia, generator
        Intelligence,   // inteligencja: warsztat, uzdatnianie wody, lecznica
        Agility         // zręczność: farma, wyprawy
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
