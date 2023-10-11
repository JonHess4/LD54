using Godot;

public abstract class Unit {
  protected Texture2D _sprite;
  public Equipable equipable;
  public string group;
  public int move;
  public int range;

  public abstract Texture2D getSprite();

}