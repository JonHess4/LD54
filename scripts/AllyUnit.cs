using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class AllyUnit : Unit {
  public bool isSelected;
  public TileMap tilemap2;
  public List<Vector2I> priorityCells;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    base._Ready();

    this.isEnemy = false;
    this.isTurn = true;
    this.isSelected = false;
    this.isTeamTurn = true;
    
    this.tilemap2 = GetNode<TileMap>("../TileMap2");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    base._Process(delta);

    if (this.damageText.Text != null && this.damageText.Text.Length > 0) {
      this.damageTextDisplayTime += delta;
      if (this.damageTextDisplayTime > 1) {
        this.damageTextDisplayTime = 0;
        this.damageText.Text = "";
      }
    }

    Vector2 mosPos = GetGlobalMousePosition();
    if (Math.Abs(mosPos.X - this.Position.X) < 16 && Math.Abs(mosPos.Y - this.Position.Y) < 16) {
      this.showUnitDetails();
    }
    if (Input.IsActionJustPressed("right_click")) {
      if (Math.Abs(mosPos.X - this.Position.X) < 16 && Math.Abs(mosPos.Y - this.Position.Y) < 16) {
        this.endTurn();
      }
    }
    if (Input.IsActionJustPressed("left_click") && this.isTurn) {
      if (Math.Abs(mosPos.X - this.Position.X) < 16 && Math.Abs(mosPos.Y - this.Position.Y) < 16) {
        this.isSelected = true;
        this.path = new List<Vector2I>();
        this.priorityCells = new List<Vector2I>();
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

      if ((newTargetCellPos != this.targetCellPos && (this.target == null || this.tilemap.LocalToMap(this.target.Position) != newTargetCellPos)) || this.path.Count <= 0) {

        if (!this.priorityCells.Contains(newTargetCellPos)) {
          this.priorityCells.Add(newTargetCellPos);
          if (this.priorityCells.Count > 6) {
            this.priorityCells.Reverse();
            this.priorityCells = this.priorityCells.Take(6).ToList();
            this.priorityCells.Reverse();
          }
        }

        bool isTraversable = AStar.isTraversable(this.tilemap, newTargetCellPos, this);
        Unit occupantUnit = AStar.isOccupied(this.tilemap, newTargetCellPos, this);
        // try to path to the new target cell pos
        if (isTraversable && occupantUnit == null) {
          this.clearPath(this.path, this.tilemap2);
          this.targetCellPos = newTargetCellPos;

          this.path = AStar.findPath(oldCellPos, this.targetCellPos, this.tilemap, this, this.priorityCells).Take(this.movement).ToList();

          this.targetCellPos = this.path.Count > 0 ? this.path.Last() : this.oldCellPos;

          this.drawPath(this.path, this.tilemap2);
        } else if (occupantUnit != null) {

          List<Vector2I> atkPosCells = this.getAtkPosCells(newTargetCellPos);
          this.clearPath(this.path, this.tilemap2);
          foreach (Vector2I atkPos in atkPosCells) {
            this.path = AStar.findPath(oldCellPos, atkPos, this.tilemap, this, this.priorityCells).Take(this.movement).ToList();
            if (this.path.Count > 0) {
              this.targetCellPos = this.path.Last();
              this.drawPath(this.path, this.tilemap2);
              break;
            }
          }
          if (this.path.Count <= 0) {
            this.targetCellPos = this.oldCellPos;
          }
        }

        // see if newTargetCellPos is targeting an enemy
        this.oldTarget = this.target;
        this.target = null;
        int dist = Math.Abs(this.targetCellPos.X - newTargetCellPos.X) + Math.Abs(this.targetCellPos.Y - newTargetCellPos.Y);
        if (dist <= this.atkRange && occupantUnit != null && (occupantUnit.isEnemy != this.isEnemy || this.canHeal)) {
          this.target = occupantUnit;
          occupantUnit.setIsTargeted(true);

          List<Vector2I> atkPosCells = this.getAtkPosCells(newTargetCellPos);
          this.clearPath(this.path, this.tilemap2);
          foreach (Vector2I atkPos in atkPosCells) {
            this.path = AStar.findPath(oldCellPos, atkPos, this.tilemap, this, this.priorityCells);
            this.path = this.path.Take(this.movement).ToList();
            if (this.path.Count > 0) {
              this.targetCellPos = this.path.Last();
              this.drawPath(this.path, this.tilemap2);
              break;
            }
          }
        }

        if (this.oldTarget != null && this.oldTarget != this.target) {
          this.oldTarget.setIsTargeted(false);
        }
      }
    }
  }

  void drawPath(List<Vector2I> cellCords, TileMap pathMap) {
    foreach (Vector2I cellPos in cellCords) {
      if (cellPos == cellCords.Last()) {
        pathMap.SetCell(0, cellPos, 0, new Vector2I(1, 3));
      } else {
        pathMap.SetCell(0, cellPos, 0, new Vector2I(2, 3));
      }
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

  List<Vector2I> getAtkPosCells(Vector2I targetPos) {
    List<Vector2I> atkPosCells = new List<Vector2I>();

    if (this.atkRange == 1) {
      atkPosCells.Add(new Vector2I(targetPos.X + 1, targetPos.Y));
      atkPosCells.Add(new Vector2I(targetPos.X - 1, targetPos.Y));
      atkPosCells.Add(new Vector2I(targetPos.X, targetPos.Y + 1));
      atkPosCells.Add(new Vector2I(targetPos.X, targetPos.Y - 1));
    } else if (this.atkRange == 2) {
      atkPosCells.Add(new Vector2I(targetPos.X + 2, targetPos.Y));
      atkPosCells.Add(new Vector2I(targetPos.X - 2, targetPos.Y));
      atkPosCells.Add(new Vector2I(targetPos.X, targetPos.Y + 2));
      atkPosCells.Add(new Vector2I(targetPos.X, targetPos.Y - 2));
      atkPosCells.Add(new Vector2I(targetPos.X + 1, targetPos.Y + 1));
      atkPosCells.Add(new Vector2I(targetPos.X - 1, targetPos.Y - 1));
      atkPosCells.Add(new Vector2I(targetPos.X + 1, targetPos.Y - 1));
      atkPosCells.Add(new Vector2I(targetPos.X - 1, targetPos.Y + 1));
    }

    atkPosCells = atkPosCells.OrderBy(x => {
      double dist = Math.Sqrt(((x.X - this.targetCellPos.X) * (x.X - this.targetCellPos.X)) + ((x.Y - this.targetCellPos.Y) * (x.Y - this.targetCellPos.Y)));
      if (this.priorityCells != null && this.priorityCells.Contains(x)) {
        dist -= this.priorityCells.IndexOf(x) + 1;
      }
      return dist;
    }
    ).ToList();

    int count = atkPosCells.Count;

    List<Vector2I> cellsToRemove = new List<Vector2I>();
    foreach (Vector2I atkCell in atkPosCells) {
      if (AStar.isOccupied(this.tilemap, atkCell, this) != null || Math.Abs(atkCell.X - this.oldCellPos.X) + Math.Abs(atkCell.Y - this.oldCellPos.Y) > this.movement) {
        cellsToRemove.Add(atkCell);
      }
    }

    if (cellsToRemove.Count > 0) {
      foreach (Vector2I remove in cellsToRemove) {
        atkPosCells.Remove(remove);
      }
    }

    return atkPosCells;
  }
}
