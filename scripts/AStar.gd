class_name AStar

static func findPath(startCellPos: Vector2i, targetCellPos: Vector2i, tilemap: TileMap, movingUnit: Unit, priorityCells: Array[Vector2i]) -> Array[Vector2i]:
	var path: Array[Vector2i] = [];
	
	if (GameEngine.gameOver):
		return path;
		
	var start: Tile = Tile.new();
	start.x = startCellPos.x;
	start.y = startCellPos.y;
	
	var finish: Tile = Tile.new();
	finish.x = targetCellPos.x;
	finish.y = targetCellPos.y;
	
	start.setDistance(finish.x, finish.y);
	
	var activeTiles: Array[Tile] = [];
	activeTiles.append(start);
	var visitedTiles: Array[Tile] = [];
	
	while(activeTiles.size() > 0):
		activeTiles.sort_custom(func(a, b): return a.costDistance() < b.costDistance());
		var checkTile: Tile = activeTiles.front();
		
		if (checkTile.x == finish.x && checkTile.y == finish.y):
			path = [];
			var tile: Tile = checkTile;
			
			while(true):
				var pos: Vector2i = Vector2i(tile.x, tile.y);
				if (isTraversable(tilemap, pos, movingUnit)):
					path.append(pos);
				tile = tile.parent;
				if (tile == null):
					path.reverse();
					return path;
		
		visitedTiles.append(checkTile);
		activeTiles.remove_at(activeTiles.find(checkTile));
		
		var walkableTiles: Array[Tile] = getWalkableTiles(tilemap, checkTile, finish, movingUnit, priorityCells);
		
		for walkableTile in walkableTiles:
			if (visitedTiles.any(func(tile): return tile.x == walkableTile.x && tile.y == walkableTile.y)):
				continue;
				
			if (activeTiles.has(walkableTile)):
				var ind: int = activeTiles.find(walkableTile);
				var existingTile: Tile = activeTiles[ind];
				if (existingTile.costDistance() > walkableTile.costDistance()):
					activeTiles.remove_at(ind);
					activeTiles.append(walkableTile);
			else:
				activeTiles.append(walkableTile);
	
	return path;
	
static func getWalkableTiles(tilemap: TileMap, currentTile: Tile, targetTile: Tile, movingUnit: Unit, priorityCells: Array[Vector2i]) -> Array[Tile]:
	var tileCords: Array[Vector2i] = [];
	tileCords.append(Vector2i(currentTile.x, currentTile.y + 1));
	tileCords.append(Vector2i(currentTile.x, currentTile.y - 1));
	tileCords.append(Vector2i(currentTile.x + 1, currentTile.y));
	tileCords.append(Vector2i(currentTile.x - 1, currentTile.y));
	
	var possibleTiles: Array[Tile] = [];
	
	for tileCord in tileCords:
		var cost: int = currentTile.cost + 1;
		if (priorityCells != null && priorityCells.any(func(cell): return cell == tileCord)):
			cost -= 2;
		var newTile: Tile = Tile.new();
		newTile.x = tileCord.x;
		newTile.y = tileCord.y;
		newTile.parent = currentTile;
		newTile.cost = cost;
		possibleTiles.append(newTile);
		
	for tile in possibleTiles:
		tile.setDistance(targetTile.x, targetTile.y);
	
	return possibleTiles.filter(func(tile): return isTraversable(tilemap, Vector2i(tile.x, tile.y), movingUnit));
	
static func isTraversable(tilemap: TileMap, cellPos: Vector2i, movingUnit: Unit) -> bool:
	var canTraverse: bool;
	
	canTraverse = tilemap.get_cell_tile_data(0, cellPos) != null;
	var units: Array[Unit] = GameEngine.getUnits();
	for unit in units:
		canTraverse = canTraverse && (!(unit.isEnemy != movingUnit.isEnemy && cellPos == tilemap.local_to_map(unit.position)) || movingUnit.isEnemy);
		if (!canTraverse):
			break;	
	
	return canTraverse;
	
static func isOccupied(tilemap: TileMap, cellPos: Vector2i, movingUnit: Unit) -> Unit:
	var foundUnit: Unit= null;
	
	var units: Array[Unit] = GameEngine.getUnits();
	for unit in units:
		if (unit != movingUnit && cellPos == tilemap.local_to_map(unit.position)):
			foundUnit = unit;
			break;
	
	return foundUnit;
