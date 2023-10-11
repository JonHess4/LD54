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
  private Node2D _controller;
  public Node2D controller {
	get {
	  if (this._controller == null) {
		this._controller = this.GetNode<Node2D>("./Controller");
	  }
	  return _controller;
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
	Color color;
	ShaderMaterial mat = this.Material as ShaderMaterial;

	if (newUnit.group == "ally") {
	  color = new Color(0x5093a4ff);
	  this.Modulate = color;

	  mat.SetShaderParameter("outline_color", new Vector4(color.R, color.G, color.B, color.A));

	  this.controller.SetScript(GD.Load("res://scripts/AllyController.cs"));

	} else if (newUnit.group == "enemy") {
	  color = new Color(0x93272dff);
	  this.Modulate = color;

	  mat.SetShaderParameter("outline_color", new Vector4(color.R, color.G, color.B, color.A));

	  this.controller.SetScript(GD.Load("res://scripts/SwordsmanAi.cs"));
	}
  }
}
