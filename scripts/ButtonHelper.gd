extends Button
class_name ButtonHelper

var unitName: String;
var tilemap: TileMap;
var unit: PackedScene;

# Called when the node enters the scene tree for the first time.
func _ready():
	self.tilemap = self.get_node("../TileMap");

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if (GameEngine.gameOver):
		return;
	
	if (GameEngine.isBuyPhase && !self.visible):
		self.visible = true;
	elif (!GameEngine.isBuyPhase && self.visible):
		self.visible = false;

func spawn() -> void:
	var unitSprite: Sprite2D = self.unit.instantiate();
	var usedCells: Array[Vector2i] = self.tilemap.get_used_cells(0);
	usedCells.sort_custom(func(a, b): return abs(a.x) + abs(a.y) < abs(b.x) + abs(b.y));
	
	for cell in usedCells:
		if (AStar.isOccupied(self.tilemap, cell, null) == null):
			unitSprite.position = self.tilemap.map_to_local(cell);
			get_parent().add_child(unitSprite);
			GameEngine.endBuyPhase();
			break;
			
func _pressed():
	self.spawn();
