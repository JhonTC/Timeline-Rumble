using System.Collections.Generic;
using System.Linq;

public class Team
{
    public static Dictionary<ushort, Team> list = new Dictionary<ushort, Team>();
    public static ushort currentTeamId = 0;

    public ushort id;
    public List<Player> players = new List<Player>();

    public Team(Player[] _players)
    {
        players = _players.ToList();

        id = currentTeamId;
        currentTeamId++;

        for (int i = 0; i < _players.Length; i++)
        {
            _players[i].teamId = id;
        }

        list.Add(id, this);
    }

    public Team(Player _player)
    {
        players.Add(_player);

        id = currentTeamId;
        currentTeamId++;

        _player.teamId = id;

        list.Add(id, this);
    }
}
