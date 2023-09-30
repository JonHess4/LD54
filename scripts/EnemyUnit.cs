using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class EnemyUnit : Unit {
  // Called when the node enters the scene tree for the first time.
  public Texture2D idleTexture;
  public static Texture2D targetedTexture;
  public List<Vector2I> priorityCells = new List<Vector2I>();
  public double tickRate = 0.3;
  public double tickTimer = 0;
  public bool isTurnStarted = false;
  public bool isReachedNextTile = false;
  public override void _Ready() {
    Engine.addUnit(this);
    this.isTeamTurn = false;
    this.tilemap = GetNode<TileMap>("../TileMap");
    this.healthbar = GetNode<ProgressBar>("./HealthBar");
    this.unitDetails = GetNode<RichTextLabel>("../UnitDetails");
    this.damageText = GetNode<RichTextLabel>("./DamageText");
    StyleBoxFlat sbf = new StyleBoxFlat();
    healthbar.AddThemeStyleboxOverride("fill", sbf);
    sbf.BgColor = new Color("C0483D");
    this.isEnemy = true;
    this.movement = 6;
    this.maxHp = 5;
    this.currentHp = this.maxHp;
    this.atk = 4;
    this.minAtk = 2;
    this.idleTexture = this.Texture;
    this.isTurn = false;
    this.atkRange = 1;
    this.healthbar.MaxValue = this.maxHp;
    this.healthbar.Value = this.currentHp;
    this.oldCellPos = this.tilemap.LocalToMap(this.Position);
    this.targetCellPos = this.tilemap.LocalToMap(this.Position);
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

    if (this.isTurn) {
      if (!this.isTurnStarted) {
        this.isTurnStarted = true;
        this.isReachedNextTile = false;
        this.findTargetAndPath();
        foreach (Vector2I pos in this.path) {
          GD.Print("Path: " + pos);
        }
      }
      this.tickTimer += delta;
      if (this.tickTimer >= this.tickRate) {
        this.tickTimer = 0;
        this.isReachedNextTile = true;
        this.Position = this.tilemap.MapToLocal(this.path.First()) + this.posMod;
      }

      Vector2 destPos = this.tilemap.MapToLocal(this.path.First()) + this.posMod;
      if (this.Position != destPos) {
        if (this.Position.DistanceTo(destPos) <= 3) {
          this.Position = destPos;
          this.isReachedNextTile = true;
          this.tickTimer = 0;
        } else {
          Vector2 oldPos = this.tilemap.MapToLocal(this.oldCellPos);
          this.Position = oldPos + ((destPos - oldPos) * (float)(this.tickTimer / this.tickRate));
        }
      } else {
        this.isReachedNextTile = true;
      }

      if (this.isReachedNextTile) {
        this.oldCellPos = this.tilemap.LocalToMap(this.Position);
        this.isReachedNextTile = false;
        if (this.path.Count == 1) {
          this.targetCellPos = this.path.Last();
          if (this.target != null && IsInstanceValid(this.target)) {
            Vector2I newTargetCellPos = this.tilemap.LocalToMap(this.target.Position);
            if (Math.Abs(this.targetCellPos.X - newTargetCellPos.X) + Math.Abs(this.targetCellPos.Y - newTargetCellPos.Y) <= this.atkRange) {
              this.attack(this.target);
            }
          }
        }
        this.path.RemoveAt(0);
        if (this.path.Count <= 0) {
          this.isTurnStarted = false;
          this.endTurn();
        }
      }
    }
  }

  public void findTargetAndPath() {
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
    if (this.target != null) {
      this.path = AStar.findPath(this.tilemap.LocalToMap(this.Position), this.tilemap.LocalToMap(this.target.Position), this.tilemap, this, this.priorityCells).Take(this.movement).ToList();
      while (this.path.Count > 1) {
        this.path.RemoveAt(this.path.Count - 1);
        Unit occupantUnit = AStar.isOccupied(this.tilemap, this.path.Last(), this);
        if (occupantUnit == null) {
          break;
        }
      }
      GD.Print("PathCount: " + this.path.Count);
    }
  }
}