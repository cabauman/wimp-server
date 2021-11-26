using System;
using System.Collections.Generic;
using WIMP_Server.Models;

namespace WIMP_Server.Data;

public interface IWimpRepository
{
    bool SaveChanges();

    void CreateIntel(Intel intel);
    void UpdateIntel(Intel intel);
    void CreateCharacter(Character character);
    void CreateShip(Ship ship);
    void CreateStarSystem(StarSystem starSystem);

    bool HasCharacter(int id);
    bool HasCharacter(string name);

    bool HasShip(int id);
    bool HasShip(string name);

    bool HasStarSystem(int id);
    bool HasStarSystem(string name);

    IEnumerable<Intel> GetIntel();

    Intel GetIntelById(int id);

    Intel GetMostRecentIntelForCharacterWithId(int id);

    Intel GetIntelForCharacterWithIdAtTimestamp(int id, DateTime timestamp);

    IEnumerable<Intel> GetIntelSinceTime(DateTime timestamp);

    Character GetCharacterWithId(int id);
    Character GetCharacterWithName(string name);

    Ship GetShipWithId(int id);
    Ship GetShipWithName(string name);
    IEnumerable<Ship> GetShips();
    IEnumerable<Ship> FindShipsWithNames(IEnumerable<string> names);

    IEnumerable<StarSystem> GetAllStarSystems();
    StarSystem GetStarSystemWithId(int id);
    StarSystem GetStarSystemWithName(string name);

    IEnumerable<Stargate> GetAllStargates();

    IEnumerable<Character> FindCharacters(string name);
    IEnumerable<StarSystem> FindStarSystems(string name);

    IEnumerable<Stargate> GetStargatesWithSourceSystemId(int id);
    IEnumerable<Stargate> GetStargatesWithDestinationSystemId(int id);
}
