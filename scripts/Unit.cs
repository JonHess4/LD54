using System;
using System.Collections.Generic;
using Godot;

public partial class Unit : Sprite2D {
  public TileMap tilemap;
  public Vector2I posMod = new Vector2I(0, 0);
  public Vector2I oldCellPos;
  public Vector2I targetCellPos;
  public List<Vector2I> path;
  public Unit target;
  protected bool isTargeted;
  public bool isEnemy;
  public int movement;
  public int maxHp;
  public int currentHp;
  public int atk;
  public int atkRange;
  public Unit oldTarget;
  public bool canHeal = false;
  public bool isTurn;
  public ProgressBar healthbar;
  public virtual void setIsTargeted(bool isTargeted) {
    this.isTargeted = isTargeted;
  }
  public virtual bool getIsTargeted() {
    return this.isTargeted;
  }

  public void modHp(int mod) {
    this.currentHp += mod;
    this.healthbar.Value = this.currentHp;
    if (currentHp <= 0) {
      Engine.removeUnit(this);
      QueueFree();
    }
  }

  public void attack(Unit target) {
    int mod = new Random().Next(1, this.atk + 1) * -1;
    if (this.canHeal && target.isEnemy == this.isEnemy) {
      mod = mod * -1;
    }
    target.modHp(mod);
  }

  public void endTurn() {
    this.isTurn = false;
    Engine.checkEndTurn(this.isEnemy);
  }

}
