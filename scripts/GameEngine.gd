class_name GameEngine;

static var endGameText: String = "";
static var _tilemap: TileMap;
static var numRound: int = 1;
static var prevRound: int = 1;
static var gameOver: bool;
static var isBuyPhase: bool;
static var _units: Array[Unit];

static var enemyDagger: PackedScene = load("res://scenes/red_dagger.tscn");
static var enemySword: PackedScene = load("res://scenes/red_sword.tscn");
static var enemyBomb: PackedScene = load("res://scenes/red_bomb.tscn");

static var enemieScenes: Array[PackedScene] = [
	enemyDagger,
	enemySword,
	enemyBomb,
	enemyBomb];
	
static func getTilemap() -> TileMap:
	if (_tilemap == null):
		var mainLoop: MainLoop = Engine.get_main_loop();
		var sceneTree: SceneTree = mainLoop as SceneTree;
		var nodes: Array[Node] = sceneTree.get_nodes_in_group("TileMap");
		_tilemap = nodes[0];
		
	return _tilemap;

static func getUnits() -> Array[Unit]:
	if (_units == null):
		_units = [];
	return _units;
	
static func addUnit(unit: Unit) -> void:
	getUnits().append(unit);
	
static func removeUnit(removedUnit: Unit) -> void:
	getUnits().remove_at(getUnits().find(removedUnit));
	var endCombat: bool = true;
	for unit in getUnits():
		if (unit.isEnemy == removedUnit.isEnemy):
			endCombat = false;
			break;
	if (endCombat):
		if (removedUnit.isEnemy):
			endCombatPhase();
		else:
			endGameText = "[center]Game Over: On round " + str(numRound) + ", Party was wiped out[/center]";
			onGameOver();
	
static func endBuyPhase() -> void:
	isBuyPhase = false;
	
	var units: Array[Unit] = getUnits();
	for unit in units:
		if (unit.isEnemy):
			unit.resetColor();
		else:
			unit.enable();
			
	var prev: int = numRound;
	numRound += 1;
	prevRound = prev;
	var rand: RandomNumberGenerator = RandomNumberGenerator.new();
	var tilemap: TileMap = getTilemap();
	var usedCells: Array[Vector2i] = tilemap.get_used_cells(0);
	usedCells.shuffle();
	var count: int = numRound + round(numRound / 4);
	for i in range(0, count):
		var dex: int = 3 if numRound == 2 else rand.randi_range(0, enemieScenes.size() - 1);
		var enemy: Sprite2D = enemieScenes[dex].instantiate();
		for cell in usedCells:
			if (AStar.isOccupied(tilemap, cell, null) == null):
				enemy.position = tilemap.map_to_local(cell);
				tilemap.get_parent().add_child(enemy);
				break;
		
	
static func endCombatPhase():
	isBuyPhase = true;
	
	for unit in getUnits():
		unit.disable();
		
static func checkEndTurn(isEnemy: bool) -> void:
	var turnFlip: bool = true;
	
	for unit in getUnits():
		if (unit.isEnemy == isEnemy):
			if (unit.isTurn):
				turnFlip = false;
				break;
	
	if (turnFlip):
		var tilemap: TileMap = getTilemap();
		for unit in getUnits():
			for targetUnit in getUnits():
				if (unit.isEnemy && !targetUnit.isEnemy):
					if (tilemap.get_cell_tile_data(0, tilemap.local_to_map(targetUnit.position)) == null):
						continue;
					else:
						var path: Array[Vector2i] = AStar.findPath(tilemap.local_to_map(unit.position), tilemap.local_to_map(targetUnit.position), tilemap, unit, []);
						if (path.size() <= 0):
							endGameText = "[center]Game Over: On round " + str(numRound) + ", unreachable enemy detected[/center]";
							onGameOver();
							return;
			
			if (unit.isEnemy != isEnemy):
				unit.readyTurn();
				unit.isTeamTurn = true;
			else:
				unit.isTeamTurn = false;
				unit.resetColor();
	
static func onGameOver():
	gameOver = true;
	
static func onReset() -> void:
	gameOver = false;
	_units = [];
	_tilemap = null;
	numRound = 1;
	prevRound = 1;
	var mainLoop: MainLoop = Engine.get_main_loop();
	var sceneTree: SceneTree = mainLoop as SceneTree;
	sceneTree.reload_current_scene();
