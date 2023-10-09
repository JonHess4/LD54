using Godot;

public class Swordsman : Unit{

  public override Texture2D getSprite() {
    if (this._sprite == null) {
      this._sprite = (Texture2D)GD.Load("res://sprites/swordsman.png");
    }
    return this._sprite;
  }
}