using UnityEngine;
using UnityEditor;

public class AutoSpriteSlicer : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;

        // 타겟 파일명 조건 지정 (예: 특정 폴더 아래 PNG만 적용)
        if (!assetPath.Contains("Sprites/Tilesets") || !assetPath.EndsWith(".png"))
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = 32; // ← 여기가 타일 기준

        // 자르기 정보 설정
        int tileWidth = 32;
        int tileHeight = 32;

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (tex == null) return;

        int hCount = tex.width / tileWidth;
        int vCount = tex.height / tileHeight;

        SpriteMetaData[] metas = new SpriteMetaData[hCount * vCount];
        int index = 0;

        for (int y = 0; y < vCount; y++)
        {
            for (int x = 0; x < hCount; x++)
            {
                SpriteMetaData meta = new SpriteMetaData();
                meta.name = $"tile_{x}_{vCount - 1 - y}";
                meta.rect = new Rect(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                meta.pivot = new Vector2(0.5f, 0.5f);
                meta.alignment = (int)SpriteAlignment.Center;
                metas[index++] = meta;
            }
        }

        importer.spritesheet = metas;
    }
}
