using System;
using System.Collections.Generic;
using Godot;

public partial class Unit : Sprite2D {
  public string unitName;
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
  public int minAtk;
  public int atkRange;
  public Unit oldTarget;
  public bool canHeal = false;
  public bool isTurn;
  public ProgressBar healthbar;
  public RichTextLabel unitDetails;
  public bool isTeamTurn = true;
  public RichTextLabel damageText;
  public double damageTextDisplayTime = 0;
  public List<Vector2I> priorityCells = new List<Vector2I>();


  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    Engine.addUnit(this);
    this.tilemap = GetNode<TileMap>("../TileMap");
    this.healthbar = GetNode<ProgressBar>("./HealthBar");
    this.unitDetails = GetNode<RichTextLabel>("../UnitDetails");
    this.damageText = GetNode<RichTextLabel>("./DamageText");
    StyleBoxFlat sbf = new StyleBoxFlat();
    healthbar.AddThemeStyleboxOverride("fill", sbf);
    sbf.BgColor = new Color("C0483D");

    this.oldCellPos = this.tilemap.LocalToMap(this.Position);
    this.Position = this.tilemap.MapToLocal(this.oldCellPos) + this.posMod;
    this.targetCellPos = this.tilemap.LocalToMap(this.Position);
  }
  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (Engine.gameOver) {
      return;
    }
  }

  public virtual void setIsTargeted(bool isTargeted) {
    this.isTargeted = isTargeted;
  }
  public virtual bool getIsTargeted() {
    return this.isTargeted;
  }

  public void modHp(int mod) {
    this.currentHp += mod;
    this.healthbar.Value = this.currentHp;
    this.damageText.Text = "" + mod;
    if (currentHp <= 0) {
      Engine.removeUnit(this);
      QueueFree();
    }
  }

  public virtual void attack(Unit target) {
    if (target != null) {
      int mod = new Random().Next(this.minAtk, this.atk + 1) * -1;
      if (this.canHeal && target.isEnemy == this.isEnemy) {
        mod = mod * -1;
      }
      target.modHp(mod);
    }

  }

  public void endTurn() {
    this.disable();
    Engine.checkEndTurn(this.isEnemy);
  }

  public void readyTurn() {
    this.enable();
  }

  public void resetColor() {
    this.Modulate = new Color(1, 1, 1);
  }

  public void disable() {
    this.isTurn = false;
    this.Modulate = new Color(0.5f, 0.5f, 0.5f);
  }

  public void enable() {
    this.Modulate = new Color(1, 1, 1);
    this.isTurn = true;
  }

  public void showUnitDetails() {
    this.unitDetails.Text = "Unit:\t\t" + this.unitName + " (?)" +
      "\nHP:\t\t" + this.currentHp + "/" + this.maxHp +
      "\nAtk:\t\t" + this.minAtk + "-" + this.atk +
      "\nHeal:\t" + (this.canHeal ? this.minAtk + "-" + this.atk : "-") +
      "\nMove:\t" + (this.movement - (this.isEnemy ? 2 : 1)) +
      "\nRange:\t" + this.atkRange +
      "\nTurn:\t" + (this.isTurn ? "Ready" : (this.isTeamTurn ? "Taken" : "Waiting"));
    if (this.unitName.ToLower().Contains("sword")) {
      this.unitDetails.TooltipText = "Swords are slow, tanky, and hit hard.";
    } else if (this.unitName.ToLower().Contains("tome")) {
      this.unitDetails.TooltipText = "Tomes can either attack enemies or heal allies.";
    } else if (this.unitName.ToLower().Contains("bow")) {
      this.unitDetails.TooltipText = "Bows can attack at range.";
    } else if (this.unitName.ToLower().Contains("dagger")) {
      this.unitDetails.TooltipText = "Daggers are highly mobile, with low attack and hp.";
    } else if (this.unitName.ToLower().Contains("bomb")) {
      this.unitDetails.TooltipText = "Bombs destroy one random adjacent tile each turn,\nalong with any unit that may be standing on it at the time.";
    }
  }

  public void upgrade() {
    Random rand = new Random();
    this.maxHp += rand.Next(0, 4);
    this.currentHp = Math.Max(this.maxHp, this.currentHp + rand.Next(0, 4));
    this.minAtk += rand.Next(0, 2);
    this.atk = Math.Max(this.minAtk + 1, this.atk + rand.Next(0, 3));
    this.movement += rand.Next(0, 2);
    this.healthbar.MaxValue = this.maxHp;
    this.healthbar.Value = this.currentHp;
  }

}
