using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public static ushort currentIdValue = 0;
    public static Player LocalPlayer;
    public ushort teamId = 0;
    public static Character[] GetAllCharactersOfType(CharacterType type)
    {
        List<Character> characters = new List<Character>();
        foreach (Player player in list.Values)
        {
            if (player.character.IsCharacterOfType(type))
            {
                characters.Add(player.character);
            }
        }

        return characters.ToArray();
    }

    public List<IDynamicSelector> selectors = new List<IDynamicSelector>();

    public Character character;

    public Deck deck;

    public bool isLocal;

    private void Awake()
    {
        list.Add(currentIdValue, this);
        currentIdValue++;

        if (isLocal)
        {
            LocalPlayer = this;
        }
    }

    public void Update()
    {
        if (isLocal)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    for (int i = 0; i < selectors.Count; i++)
                    {
                        selectors[i].OnClickHit(hit);
                    }
                }
            }
        }
    }

    public void OnLeaveLocation(LocationProcess location)
    {
        

        //destroy character
    }
}
