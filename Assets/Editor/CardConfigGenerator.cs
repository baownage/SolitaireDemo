using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class CardConfigGenerator : EditorWindow
{
    private string _outputFolder = "Assets/Configs/Default";
    private string _spriteSheetPath;
    private Sprite _faceSprites;

    private List<Sprite> _allSprites;

    [MenuItem("Tools/Generate Card Configs")]
    public static void ShowWindow()
    {
        GetWindow<CardConfigGenerator>("Card Config Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate 52 Card Configs", EditorStyles.boldLabel);

        _outputFolder = EditorGUILayout.TextField("Output Folder", _outputFolder);
        _spriteSheetPath = EditorGUILayout.TextField("Sprite Sheet Path", _spriteSheetPath);
        _faceSprites = (Sprite)EditorGUILayout.ObjectField("Face Sprite Sheet", _faceSprites, typeof(Sprite), false);

        if (GUILayout.Button("Generate All 52 Configs"))
        {
            GenerateConfigs();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Single Card"))  // Bonus: For testing one at a time
        {
            GenerateSingleCard();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Debug load sprite sheet"))  // Bonus: For testing one at a time
        {
            LoadAllSpritesFromSheet();
        }
    }

    private void GenerateConfigs()
    {
        // Ensure output folder exists
        if (!Directory.Exists(_outputFolder))
        {
            Directory.CreateDirectory(_outputFolder);
        }

        LoadAllSpritesFromSheet();

        int created = 0;
        foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
        {
            if (rank == Rank.Undefined) continue;

            foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
            {
                if (suit == Suit.Undefined) continue;

                CreateConfig(rank, suit);
                created++;
            }
        }

        Debug.Log($"Generated {created} CardConfig assets in {_outputFolder}");
        AssetDatabase.Refresh();  // Refresh Project window
    }

    private void CreateConfig(Rank rank, Suit suit)
    {
        // Create unique filename
        string suitShort = suit switch
        {
            Suit.Hearts => "Hearts",
            Suit.Diamonds => "Diamonds",
            Suit.Clubs => "Clubs",
            Suit.Spades => "Spades",
            _ => "Unknown"
        };
        string rankShort = ((int)rank).ToString();
        string fileName = $"{rankShort}_{suitShort}.asset";
        string fullPath = Path.Combine(_outputFolder, fileName);

        int hearts = 0;
        int clubs = 1;
        int diamonds = 2;
        int spades = 3;

        int startIndex = suit switch
        {
            Suit.Hearts => hearts * 13,
            Suit.Clubs => clubs * 13,
            Suit.Diamonds => diamonds * 13,
            Suit.Spades => spades * 13
        };

        int spriteIndex;

        if (rank == Rank.Ace)
        {
            spriteIndex = startIndex + 12;
        }
        else
        {
            spriteIndex = startIndex + (int)rank - 2;
        }

        Sprite cardSprite = _allSprites[spriteIndex];

        // Create the asset
        CardConfig newConfig = CreateInstance<CardConfig>();
        newConfig.Rank = rank;
        newConfig.Suit = suit;
        newConfig.CardSprite = cardSprite;

        // Save
        AssetDatabase.CreateAsset(newConfig, fullPath);
        AssetDatabase.SaveAssets();
    }

    private void GenerateSingleCard()  // For quick testing
    {
        Rank selectedRank = Rank.Ace;
        Suit selectedSuit = Suit.Hearts;
        CreateConfig(selectedRank, selectedSuit);
    }

    private void LoadAllSpritesFromSheet()
    {
        // Unity 6.0 fix: Load as Object[], then filter to Sprite[]
        Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(_spriteSheetPath);
        _allSprites = allObjects.OfType<Sprite>().ToList();
        Debug.Log($"Loaded {_allSprites.Count} sprites from {_spriteSheetPath}. Ensure it has 52!");

        foreach (var sprite in _allSprites)
        {
            Debug.Log(sprite.name);
        }
    }
}