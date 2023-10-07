extends TileMap

var mainTilemap: TileMap;
var prevCell: Vector2i;
var hasPrevCell: bool;
var endPhase: bool;

# Called when the node enters the scene tree for the first time.
func _ready():
	self.mainTilemap = self.get_node("../TileMap");


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if (GameEngine.gameOver):
		return;
		
	if (GameEngine.isBuyPhase):
		var mosPos: Vector2 = self.get_global_mouse_position();
		var mosCellPos: Vector2i = self.local_to_map(mosPos);
		if (self.hasPrevCell && self.prevCell != mosCellPos):
			self.set_cell(0, self.prevCell, 0, Vector2i(2, 2));
			self.hasPrevCell = false;
		if (self.prevCell != mosCellPos && self.get_cell_tile_data(0, mosCellPos) != null):
			self.set_cell(0, mosCellPos, 0, Vector2i(1, 2));
			self.prevCell = mosCellPos;
			self.hasPrevCell = true;
		if (Input.is_action_just_pressed("left_click") && self.get_cell_tile_data(0, mosCellPos) != null):
			if (self.mainTilemap.get_cell_tile_data(0, mosCellPos) == null):
				self.mainTilemap.set_cell(0, mosCellPos, 0, Vector2i(1, 1));
				self.endPhase = true;
			
			var units: Array[Unit] = GameEngine.getUnits();
			var foundUnit: Unit;
			for unit in units:
				if (self.local_to_map(unit.position) == mosCellPos):
					foundUnit = unit;
					break;
			if (foundUnit != null):
				foundUnit.upgrade();
				self.endPhase = true;
				
			if (self.endPhase):
				self.endPhase = false;
				GameEngine.endBuyPhase();
