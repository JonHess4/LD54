using Godot;

public partial class UnitScene : Sprite2D {
  public Unit unit;
  private RichTextLabel _dmgText;
  public RichTextLabel dmgText {
    get {
      if (this._dmgText == null) {
        this._dmgText = this.GetNode<RichTextLabel>("./DmgText");
      }
      return _dmgText;
    }
  }
  private ProgressBar _hpBar;
  public ProgressBar hpBar {
    get {
      if (this._hpBar == null) {
        this._hpBar = this.GetNode<ProgressBar>("./HpBar");
      }
      return _hpBar;
    }
  }
  private Sprite2D _hpBarBorder;
  public Sprite2D hpBarBorder {
    get {
      if (this._hpBarBorder == null) {
        this._hpBarBorder = this.GetNode<Sprite2D>("./HpBarBorder");
      }
      return _hpBarBorder;
    }
  }

    // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
  }

  public void setUnit(Unit newUnit) {
    GD.Print(newUnit.group);
    this.Texture = newUnit.getSprite();
    if (newUnit.group == "ally") {
      this.hpBarBorder.Modulate = new Color(0x5093a4ff);
    } else if (newUnit.group == "enemy") {
      this.hpBarBorder.Modulate = new Color(0x93272dff);
    }
  }
}