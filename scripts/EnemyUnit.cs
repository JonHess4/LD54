using System;
using System.Linq;
using Godot;

public partial class EnemyUnit : Unit {
  // Called when the node enters the scene tree for the first time.
  public Texture2D idleTexture;
  public static Texture2D targetedTexture;
  public override void _Ready() {
    Engine.addUnit(this);
    this.tilemap = GetNode<TileMap>("../TileMap");
    this.healthbar = GetNode<ProgressBar>("./HealthBar");
    StyleBoxFlat sbf = new StyleBoxFlat();
    healthbar.AddThemeStyleboxOverride("fill", sbf);
    sbf.BgColor = new Color("C0483D");
    this.isEnemy = true;
    this.movement = 6;
    this.maxHp = 6;
    this.currentHp = this.maxHp;
    this.atk = 6;
    this.idleTexture = this.Texture;
    this.isTurn = false;
    this.atkRange = 1;
    this.healthbar.MaxValue = this.maxHp;
    this.healthbar.Value = this.currentHp;
  }

  public static Texture2D getTargetedTexture() {
    if (targetedTexture == null) {
      targetedTexture = (Texture2D)GD.Load("res://sprites/them-targeted.png");
    }
    return targetedTexture;
  }

  public override void setIsTargeted(bool newTargetedVal) {
    if (this.isTargeted != newTargetedVal) {
      if (newTargetedVal) {
        this.Texture = getTargetedTexture();
      } else {
        this.Texture = this.idleTexture;
      }
    }
    this.isTargeted = newTargetedVal;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (this.isTurn) {
      double dist = 1000;
      foreach (Unit unit in Engine.units) {
        if (unit.isEnemy != this.isEnemy) {
          double otherdist = this.Position.DistanceTo(unit.Position);
          if (otherdist < dist) {
            dist = otherdist;
            this.target = unit;
          }
        }
      }

      this.path = AStar.findPath(this.tilemap.LocalToMap(this.Position), this.tilemap.LocalToMap(this.target.Position), this.tilemap, this).Take(this.movement).ToList();
      while (this.path.Count > 1) {
        this.path.RemoveAt(this.path.Count - 1);
        if (!AStar.isOccupied(this.tilemap, this.path.Last(), this)) {
          break;
        }
      }

      this.Position = this.tilemap.MapToLocal(this.path.Last());
      this.targetCellPos = this.path.Last();
      Vector2I newTargetCellPos = this.tilemap.LocalToMap(this.target.Position);
      GD.Print("CellPos: " + this.targetCellPos);
      GD.Print("TargetCellPos: " + newTargetCellPos);
      if (Math.Abs(this.targetCellPos.X - newTargetCellPos.X) + Math.Abs(this.targetCellPos.Y - newTargetCellPos.Y) <= this.atkRange) {
        GD.Print("Attacking");
        this.attack(this.target);
      }
      this.endTurn();
    }
  }
}