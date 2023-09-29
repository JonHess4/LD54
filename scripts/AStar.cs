using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class AStar {
  public static List<Vector2I> findPath(Vector2I startCellPos, Vector2I targetCellPos, TileMap tilemap, Unit movingUnit) {
    List<Vector2I> path = new List<Vector2I>();

    Tile start = new Tile();
    start.Y = startCellPos.Y;
    start.X = startCellPos.X;

    Tile finish = new Tile();
    finish.Y = targetCellPos.Y;
    finish.X = targetCellPos.X;

    start.SetDistance(finish.X, finish.Y);

    var activeTiles = new List<Tile>();
    activeTiles.Add(start);
    var visitedTiles = new List<Tile>();

    while (activeTiles.Any()) {
      Tile checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

      if (checkTile.X == finish.X && checkTile.Y == finish.Y) {
        path = new List<Vector2I>();
        //We found the destination and we can be sure (Because the the OrderBy above)
        //That it's the most low cost option. 
        var tile = checkTile;
        // GD.Print("Retracing steps backwards...");
        while (true) {
          // GD.Print($"{tile.X} : {tile.Y}");
          // if (tilemap.GetCellTileData(0, new Vector2I(tile.X, tile.Y)) != null) {
          if (isTraversable(tilemap, new Vector2I(tile.X, tile.Y), movingUnit)) {
            path.Add(new Vector2I(tile.X, tile.Y));
          }
          tile = tile.Parent;
          if (tile == null) {
            path.Reverse();
            return path;
          }
        }
      }

      visitedTiles.Add(checkTile);
      activeTiles.Remove(checkTile);

      var walkableTiles = getWalkableTiles(tilemap, checkTile, finish, movingUnit);

      foreach (var walkableTile in walkableTiles) {
        //We have already visited this tile so we don't need to do so again!
        if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
          continue;

        //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
        if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y)) {
          var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
          if (existingTile.CostDistance > walkableTile.CostDistance) {
            activeTiles.Remove(existingTile);
            activeTiles.Add(walkableTile);
          }
        } else {
          //We've never seen this tile before so add it to the list. 
          activeTiles.Add(walkableTile);
        }
      }
    }

    // GD.Print("No Path Found!");
    return path;
  }

  private static List<Tile> getWalkableTiles(TileMap tilemap, Tile currentTile, Tile targetTile, Unit movingUnit) {
    var possibleTiles = new List<Tile>()
    {
    new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
    new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
    new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
    new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
    // new Tile { X = currentTile.X + 1, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
    // new Tile { X = currentTile.X + 1, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
    // new Tile { X = currentTile.X - 1, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
    // new Tile { X = currentTile.X - 1, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
  };

    possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

    return possibleTiles
        // TODO: figure out detecting traversable vs non-traversable tiles
        .Where(tile => isTraversable(tilemap, new Vector2I(tile.X, tile.Y), movingUnit)) // tilemap.GetCellTileData(0, new Vector2I(tile.X, tile.Y)) != null)
        .ToList();
  }

  private static Bounds calculateBounds(TileMap tilemap) {
    Bounds bounds = new Bounds();

    Array<Vector2I> usedCells = tilemap.GetUsedCells(0);
    bounds.minX = usedCells[0].X;
    bounds.maxX = usedCells[0].X;
    bounds.minY = usedCells[0].Y;
    bounds.maxY = usedCells[0].Y;
    foreach (Vector2I pos in usedCells) {
      if (pos.X < bounds.minX) {
        bounds.minX = pos.X;
      } else if (pos.X > bounds.maxX) {
        bounds.maxX = pos.X;
      }
      if (pos.Y < bounds.minY) {
        bounds.minY = pos.Y;
      } else if (pos.Y > bounds.maxY) {
        bounds.maxY = pos.Y;
      }
    }

    return bounds;
  }

  public static bool isTraversable(TileMap tilemap, Vector2I cellPos, Unit movingUnit) {
    bool isTraversable;

    isTraversable = tilemap.GetCellTileData(0, cellPos) != null;
    foreach (Unit unit in Engine.units) {
      isTraversable = isTraversable && (!(unit.isEnemy != movingUnit.isEnemy && cellPos == tilemap.LocalToMap(unit.Position)) || movingUnit.isEnemy);
      if (!isTraversable) {
        break;
      }
    }

    return isTraversable;
  }

  public static bool isOccupied(TileMap tilemap, Vector2I cellPos, Unit movingUnit) {
    bool isVacant = true;

    foreach (Unit unit in Engine.units) {
      isVacant = isVacant && (unit == movingUnit || !(cellPos == tilemap.LocalToMap(unit.Position)));
      if (!isVacant) {
        break;
      }
    }

    return !isVacant;
  }
}