using System;
using System.Collections.Generic;
using System.Linq;
using WIMP_Server.Models;

namespace WIMP_Server.Data;

public class WimpRepository : IWimpRepository
{
    private readonly WimpDbContext _dbContext;

    public WimpRepository(WimpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void CreateCharacter(Character character)
    {
        if (character == null)
        {
            throw new ArgumentNullException(nameof(character));
        }

        _dbContext.Characters.Add(character);
    }

    public void CreateIntel(Intel intel)
    {
        if (intel == null)
        {
            throw new ArgumentNullException(nameof(intel));
        }

        _dbContext.Intel.Add(intel);
    }

    public void CreateShip(Ship ship)
    {
        if (ship == null)
        {
            throw new ArgumentNullException(nameof(ship));
        }

        _dbContext.Ships.Add(ship);
    }

    public void CreateStarSystem(StarSystem starSystem)
    {
        if (starSystem == null)
        {
            throw new ArgumentNullException(nameof(starSystem));
        }

        _dbContext.StarSystems.Add(starSystem);
    }

    public IEnumerable<Character> FindCharacters(string name)
    {
        return _dbContext.Characters
            .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public IEnumerable<Ship> FindShipsWithNames(IEnumerable<string> names)
    {
        return _dbContext.Ships.ToList()
            .Where(s => names.Any(sn => string.Equals(sn, s.Name, StringComparison.InvariantCultureIgnoreCase)));
    }

    public IEnumerable<StarSystem> FindStarSystems(string name)
    {
        return _dbContext.StarSystems
            .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public IEnumerable<Stargate> GetAllStargates()
    {
        return _dbContext.Stargates.ToList();
    }

    public IEnumerable<StarSystem> GetAllStarSystems()
    {
        return _dbContext.StarSystems.ToList();
    }

    public Character GetCharacterWithId(int id)
    {
        return _dbContext.Characters.FirstOrDefault(c => c.CharacterId == id);
    }

    public Character GetCharacterWithName(string name)
    {
        return _dbContext.Characters.FirstOrDefault(c => c.Name == name);
    }

    public IEnumerable<Intel> GetIntel()
    {
        return _dbContext.Intel.ToList();
    }

    public Intel GetIntelById(int id)
    {
        return _dbContext.Intel.FirstOrDefault(i => i.Id == id);
    }

    public Intel GetIntelForCharacterWithIdAtTimestamp(int id, DateTime timestamp)
    {
        return _dbContext.Intel.FirstOrDefault(i => i.CharacterId == id && i.Timestamp == timestamp);
    }

    public IEnumerable<Intel> GetIntelSinceTime(DateTime timestamp)
    {
        return _dbContext.Intel
            .Where(i => i.Timestamp >= timestamp)
            .OrderByDescending(i => i.Timestamp)
            .ToList();
    }

    public Intel GetMostRecentIntelForCharacterWithId(int id)
    {
        return _dbContext.Intel
            .Where(i => i.CharacterId == id)
            .OrderByDescending(i => i.Timestamp)
            .FirstOrDefault();
    }

    public IEnumerable<Ship> GetShips()
    {
        return _dbContext.Ships.ToList();
    }

    public Ship GetShipWithId(int id)
    {
        return _dbContext.Ships.FirstOrDefault(s => s.ShipId == id);
    }

    public Ship GetShipWithName(string name)
    {
        return _dbContext.Ships.FirstOrDefault(s => s.Name == name);
    }

    public IEnumerable<Stargate> GetStargatesWithDestinationSystemId(int id)
    {
        return _dbContext.Stargates
            .Where(sg => sg.DstStarSystemId == id)
            .ToList();
    }

    public IEnumerable<Stargate> GetStargatesWithSourceSystemId(int id)
    {
        return _dbContext.Stargates
            .Where(sg => sg.SrcStarSystemId == id)
            .ToList();
    }

    public StarSystem GetStarSystemWithId(int id)
    {
        return _dbContext.StarSystems.FirstOrDefault(s => s.StarSystemId == id);
    }

    public StarSystem GetStarSystemWithName(string name)
    {
        return _dbContext.StarSystems.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasCharacter(string name)
    {
        return _dbContext.Characters.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasCharacter(int id)
    {
        return _dbContext.Characters.Any(c => c.CharacterId == id);
    }

    public bool HasShip(string name)
    {
        return _dbContext.Ships.Any(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasShip(int id)
    {
        return _dbContext.Ships.Any(s => s.ShipId == id);
    }

    public bool HasStarSystem(string name)
    {
        return _dbContext.StarSystems.Any(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasStarSystem(int id)
    {
        return _dbContext.StarSystems.Any(s => s.StarSystemId == id);
    }

    public bool SaveChanges()
    {
        return _dbContext.SaveChanges() > 0;
    }

    public void UpdateIntel(Intel intel)
    {
        if (intel == null)
        {
            throw new ArgumentNullException(nameof(intel));
        }

        _dbContext.Update(intel);
    }
}
