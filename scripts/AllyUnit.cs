using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AllyUnit : Unit {
  public bool isSelected;
  public TileMap tilemap2;
  public bool pass;

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    Vector2 mosPos = GetGlobalMousePosition();
    if (Input.IsActionJustPressed("left_click") && this.isTurn) {
      if (Math.Abs(mosPos.X - this.Position.X) < 8 && Math.Abs(mosPos.Y - this.Position.Y) < 8) {
        this.isSelected = true;
        this.path = new List<Vector2I>();
      }
    } else if (isSelected && Input.IsActionJustReleased("left_click")) {
      this.isSelected = false;

      // drop character back onto map
      this.Position = this.tilemap.MapToLocal(this.targetCellPos) + this.posMod;

      if (this.target != null) {
        this.attack(this.target);
        this.target.setIsTargeted(false);
        this.endTurn();
      } else if (this.oldCellPos != this.targetCellPos) {
        this.endTurn();
      }

      this.oldCellPos = this.targetCellPos;

      this.clearPath(this.path, this.tilemap2);

    }

    if (this.isSelected) {
      this.Position = mosPos;

      Vector2I newTargetCellPos = this.tilemap.LocalToMap(this.Position);
      if (newTargetCellPos != this.targetCellPos || this.path.Count <= 0) {

        // try to path to the new target cell pos
        if (AStar.isTraversable(this.tilemap, newTargetCellPos, this) && !AStar.isOccupied(this.tilemap, newTargetCellPos, this)) {
          this.clearPath(this.path, this.tilemap2);
          this.targetCellPos = newTargetCellPos;

          if (!this.path.Contains(newTargetCellPos) && !this.pass) {
            this.path.Add(newTargetCellPos);
            if (this.path.Count > this.movement) {
              this.path = AStar.findPath(oldCellPos, this.targetCellPos, this.tilemap, this);
            }
          } else {
            this.path = AStar.findPath(oldCellPos, this.targetCellPos, this.tilemap, this);
          }
          this.path = this.path.Take(this.movement).ToList();

          this.targetCellPos = this.path.Count > 0 ? this.path.Last() : this.oldCellPos;

          this.drawPath(this.path, this.tilemap2);

          this.pass = false;
        } else {
          this.pass = true;
        }

        // see if newTargetCellPos is targeting an enemy
        this.oldTarget = this.target;
        this.target = null;
        if (Math.Abs(this.targetCellPos.X - newTargetCellPos.X) + Math.Abs(this.targetCellPos.Y - newTargetCellPos.Y) <= this.atkRange) {
          foreach (Unit unit in Engine.units) {
            if (unit != this && newTargetCellPos == this.tilemap.LocalToMap(unit.Position)) {
              this.target = unit;
              unit.setIsTargeted(true);
            } else {
              unit.setIsTargeted(false);
            }
          }

          if (this.target != null && this.target != this.oldTarget) {
            if (this.atkRange > 1) {
              this.clearPath(this.path, this.tilemap2);
              int index = Math.Max(0, this.path.Count - this.atkRange + 1);
              int count = this.path.Count - index;

              GD.Print("Path: " + this.path.Count);
              GD.Print("Index: " + index);
              GD.Print("Count: " + count);

              this.path.RemoveRange(index, count);
              this.drawPath(this.path, this.tilemap2);
              this.targetCellPos = this.path.Count >= 1 ? this.path.Last() : this.oldCellPos;
            }
          }
        }

      }
    }
  }

  void drawPath(List<Vector2I> cellCords, TileMap pathMap) {
    foreach (Vector2I cellPos in cellCords) {
      pathMap.SetCell(0, cellPos, 0, new Vector2I(2, 3));
      if (cellCords.IndexOf(cellPos) >= this.movement) {
        break;
      }
    }
  }

  void clearPath(List<Vector2I> path, TileMap pathMap) {
    foreach (Vector2I cellPos in path) {
      pathMap.SetCell(0, cellPos);
    }
  }
}
