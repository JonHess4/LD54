using Godot;

public abstract class Unit {
  protected Texture2D _sprite;
  public Equipable equipable;
  public string group;

  public abstract Texture2D getSprite();

}