using System.Collections.Generic;
using Godot;

public class MapFactory {
  // key: color HexCode
  // value: atlas coordinates for tile in tileset
  private static Dictionary<string, Vector2I> _hexAtlasMap;
  public static Dictionary<string, Vector2I> getHexAtlasMap() {
    if (_hexAtlasMap == null) {
      _hexAtlasMap = new Dictionary<string, Vector2I>();

      _hexAtlasMap.Add("88923d", new Vector2I(1, 1));
    }

    return _hexAtlasMap;
  }

  public static Dictionary<string, List<Vector2I>> initMap(Image mapData, TileMap tilemap) {
    Dictionary<string, List<Vector2I>> posMap = new Dictionary<string, List<Vector2I>>();
    posMap.Add("allySpawns", new List<Vector2I>());
    posMap.Add("enemySpawns", new List<Vector2I>());
    posMap.Add("neutralSpawns", new List<Vector2I>());
    posMap.Add("camSpawns", new List<Vector2I>());

    Vector2I mapSize = mapData.GetSize();
    Dictionary<string, Vector2I> hexAtlasMap = getHexAtlasMap();

    for (int i = 0; i < mapSize.X; i++) {
      for (int j = 0; j < mapSize.Y; j++) {
        Color color = mapData.GetPixel(i, j);
        if (color.A8 > 10) {
          Vector2I atlasCoords = hexAtlasMap[color.ToHtml(false)];
          Vector2I cellPos = new Vector2I(i, j);
          tilemap.SetCell(0, cellPos, 0, atlasCoords);
          addSpawnPoint(color, cellPos, posMap);
        }
      }
    }

    return posMap;
  }

  public static void addSpawnPoint(Color color, Vector2I cellPos, Dictionary<string, List<Vector2I>> posMap) {
    if (color.A8 < 125) {
      posMap["camSpawns"].Add(cellPos);
    } else if (color.A8 < 175) {
      posMap["enemySpawns"].Add(cellPos);
    } else if (color.A8 < 225) {
      posMap["allySpawns"].Add(cellPos);
    } else if (color.A8 < 255) {
      posMap["neutralSpawns"].Add(cellPos);
    }
  }
}