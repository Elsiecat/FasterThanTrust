using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;

public class TileGenerator
{
    [MenuItem("Tools/Generate Tiles from Selected Sprites")]
    static void GenerateTiles()
    {
        Object[] sprites = Selection.GetFiltered(typeof(Sprite), SelectionMode.DeepAssets);

        string outputPath = "Assets/Tiles/Generated";
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        foreach (Object obj in sprites)
        {
            Sprite sprite = obj as Sprite;
            string path = $"{outputPath}/{sprite.name}.asset";
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            AssetDatabase.CreateAsset(tile, path);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("타일 생성 완료!");
    }
}
