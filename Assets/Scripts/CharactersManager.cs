using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    [SerializeField] private int numberCharactersInGame = 10;
    [SerializeField] private GameObject prefCharacter = null;
    [Space]
    [SerializeField] private GameOver gameOver = null;
    [SerializeField] private HUD hud = null;
    [SerializeField] private GlobalRoute globalRoute = null;

    private PointRoute[] points = null;

    // Cache de personagens para evitar GetComponentsInChildren repetido
    private CharacterStatus[] cachedCharacters = null;

    private IA[] ias;
    public IA[] IAs {
        get
        {
            return ias;
        }
    }

    private void Awake()
    {
        points = globalRoute.GetComponentsInChildren<PointRoute>();
    }

    private void Start()
    {
        SpawnCharacters();
    }

    public int MaxCharactersinGame()
    {
        return numberCharactersInGame;
    }

    private void SpawnCharacters()
    {
        for (int i = 0; i < numberCharactersInGame; i++)
        {
            GameObject characterCurrent = Instantiate(prefCharacter, transform);
            characterCurrent.name = "Character_" + i;

            characterCurrent.transform.position = points[Random.Range(0, points.Length - 1)].transform.position;
        }

        // Cachear personagens após spawn
        RefreshCharacterCache();
        ias = GetComponentsInChildren<IA>();

        hud.UpdateHealthy();
        hud.UpdateSick();
        hud.UpdateDead();

        gameOver.EnableWinLose = true;
    }

    /// <summary>
    /// Atualiza o cache de personagens. Chamado após spawn.
    /// </summary>
    public void RefreshCharacterCache()
    {
        cachedCharacters = GetComponentsInChildren<CharacterStatus>();
    }

    public CharacterStatus[] Characters
    {
        get
        {
            if (cachedCharacters == null)
            {
                RefreshCharacterCache();
            }
            return cachedCharacters;
        }
    }

    public CharacterStatus[] CharactersHealthy
    {
        get
        {
            List<CharacterStatus> result = new List<CharacterStatus>();
            foreach (var c in Characters)
            {
                if (c.Health == HealthCondition.Healthy)
                    result.Add(c);
            }
            return result.ToArray();
        }
    }

    public CharacterStatus[] CharactersSick
    {
        get
        {
            List<CharacterStatus> result = new List<CharacterStatus>();
            foreach (var c in Characters)
            {
                if (c.Health == HealthCondition.Sick)
                    result.Add(c);
            }
            return result.ToArray();
        }
    }

    public CharacterStatus[] CharactersDead
    {
        get
        {
            List<CharacterStatus> result = new List<CharacterStatus>();
            foreach (var c in Characters)
            {
                if (c.Health == HealthCondition.Dead)
                    result.Add(c);
            }
            return result.ToArray();
        }
    }
}
