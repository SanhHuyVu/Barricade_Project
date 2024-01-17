using UnityEngine;

public enum CharacterEnum { Male1, Male2, Female }
public class CharacterSpawner : MonoBehaviour, IDataPersistence
{

    public static CharacterSpawner Instance { get; private set; }

    [SerializeField] private GameObject Male1;
    [SerializeField] private GameObject Male2;
    [SerializeField] private GameObject Female;


    public CharacterEnum NewCharacter { get; private set; } = CharacterEnum.Male1;
    public CharacterEnum CharacterToLoad { get; private set; } = CharacterEnum.Male1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetNewCharacter(CharacterEnum characterEnum)
    {
        NewCharacter = characterEnum;
        // Debug.Log($"Selected {characterEnum.ToString()}");
    }

    public void SetCharacterToLoad()
    {
        CharacterToLoad = NewCharacter;
    }

    private GameObject GetCharacterFromEnum(CharacterEnum characterEnum)
    {
        switch (characterEnum)
        {
            case CharacterEnum.Male1:
                return Male1;
            case CharacterEnum.Male2:
                return Male2;
            case CharacterEnum.Female:
                return Female;
            default:
                return null;
        }
    }

    public void SpawnPlayer()
    {
        var playerGO = Instantiate(GetCharacterFromEnum(CharacterToLoad));
    }

    public void LoadData(GameData data)
    {
        CharacterToLoad = data.SelectedCharacter;
    }

    public void SaveData(ref GameData data)
    {
        data.SelectedCharacter = CharacterToLoad;
    }
}
