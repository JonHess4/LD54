using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;

public partial class EBombUnit : EnemyUnit {

  public List<Vector2I> adjacent;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    base._Ready();

    this.unitName = "Red Bomb";

    this.movement = 3;
    this.maxHp = 2;
    this.currentHp = this.maxHp;
    this.atk = 0;
    this.minAtk = 0;
    this.atkRange = 1;

    this.healthbar.MaxValue = this.maxHp;
    this.healthbar.Value = this.currentHp;

    this.adjacent = new List<Vector2I>() {
      new Vector2I(0, 1),
      new Vector2I(0, -1),
      new Vector2I(1, 0),
      new Vector2I(-1, 0)
    };
  }

  public override void attack(Unit target) {
    int rand = new Random().Next(0, this.adjacent.Count);
    Unit occupiedUnit = AStar.isOccupied(this.tilemap, this.oldCellPos + this.adjacent[rand], this);
    if (occupiedUnit != null) {
      occupiedUnit.modHp(occupiedUnit.currentHp * -1);
    }
    this.tilemap.SetCell(0, this.oldCellPos + this.adjacent[rand]);
  }
}