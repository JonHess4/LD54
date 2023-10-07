extends Unit
class_name AllyUnit

var isSelected: bool;
var tilemap2: TileMap;

# Called when the node enters the scene tree for the first time.
func _ready():
	super();
	
	self.isEnemy = false;
	self.isTurn = true;
	self.isSelected = false;
	self.isTeamTurn = true;
	
	self.tilemap2 = self.get_node("../TileMap2");


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta);
	
	if (self.damageText.text != null && self.damageText.text.length() > 0):
		self.damageTextDisplayTime += delta;
		if (self.damageTextDisplayTime >= 1):
			self.damageTextDisplayTime = 0;
			self.damageText.text = "";
			
	var mosPos: Vector2 = self.get_global_mouse_position();
	if (abs(mosPos.x - self.position.x) < 16 && abs(mosPos.y - self.position.y) < 16):
		self.showUnitDetails();
		
	if (Input.is_action_just_pressed("right_click")):
		if (abs(mosPos.x - self.position.x) < 16 && abs(mosPos.y - self.position.y) < 16):
			self.endTurn();
	
	if (Input.is_action_just_pressed("left_click") && self.isTurn):
		if (abs(mosPos.x - self.position.x) < 16 && abs(mosPos.y - self.position.y) < 16):
			self.isSelected = true;
			self.path = [];
			self.priorityCells = [];
	elif (self.isSelected && Input.is_action_just_released("left_click")):
		self.isSelected = false;
		
		self.position = self.tilemap.map_to_local(self.targetCellPos) + self.posMod;
		
		if (self.target != null):
			self.attack(self.target);
			self.target.setIsTargeted((false));
			self.endTurn();
		elif (self.oldCellPos != self.targetCellPos):
			self.endTurn();
			
		self.oldCellPos = self.targetCellPos;
		self.clearPath(self.path, self.tilemap2);
	
	if (self.isSelected):
		self.position = mosPos;
		
		var newTargetCellPos: Vector2i = self.tilemap.local_to_map(self.position);
		
		if ((newTargetCellPos != self.targetCellPos && (self.target == null || self.tilemap.local_to_map(self.target.position) != newTargetCellPos)) || self.path.size() <= 0):
			var newCellIndex: int = self.priorityCells.find(newTargetCellPos);
			if (self.priorityCells.size() < self.movement && newCellIndex < 0):
				self.priorityCells.append(newTargetCellPos);
			elif (newCellIndex >= 0):
				self.priorityCells = self.priorityCells.slice(0, newCellIndex);
			
			var isTraversable: bool = AStar.isTraversable(self.tilemap, newTargetCellPos, self);
			var occupantUnit: Unit = AStar.isOccupied(self.tilemap, newTargetCellPos, self);
			if (isTraversable && occupantUnit == null):
				self.clearPath(self.path, self.tilemap2);
				self.targetCellPos = newTargetCellPos;
				
				self.path = AStar.findPath(self.oldCellPos, self.targetCellPos, self.tilemap, self, self.priorityCells).slice(0, self.movement);
				self.targetCellPos = self.path.back() if self.path.size() >= 0 else self.oldCellPos;
				
				self.drawPath(self.path, self.tilemap2);
			elif (occupantUnit != null):
				var atkPosCells: Array[Vector2i] = self.getAttackPosCells(newTargetCellPos);
				self.clearPath(self.path, self.tilemap2);
				
				for atkPos in atkPosCells:
					self.path = AStar.findPath(oldCellPos, atkPos, self.tilemap, self, self.priorityCells).slice(0, self.movement);
					if (self.path.size() > 0):
						self.targetCellPos = self.path.back();
						self.drawPath(self.path, self.tilemap2);
						break;
				if (self.path.size() <= 0):
					self.targetCellPos = self.oldCellPos;
			
			self.oldTarget = self.target;
			self.target = null;
			var dist: int = abs(self.targetCellPos.x - newTargetCellPos.x) + abs(self.targetCellPos.y - newTargetCellPos.y);
			if (dist <= self.atkRange && occupantUnit != null && (occupantUnit.isEnemy != self.isEnemy || self.canHeal)):
				self.target = occupantUnit;
				occupantUnit.setIsTargeted(true);
				
				var atkPosCells: Array[Vector2i] = self.getAttackPosCells(newTargetCellPos);
				self.clearPath(self.path, self.tilemap2);
				for atkPos in atkPosCells:
					self.path = AStar.findPath(self.oldCellPos, atkPos, self.tilemap, self, self.priorityCells).slice(0, self.movement);
					if (self.path.size() > 0):
						self.targetCellPos = self.path.back();
						self.drawPath(self.path, self.tilemap2);
						break;
				
			if (self.oldTarget != null && self.oldTarget != self.target):
				self.oldTarget.setIsTargeted(false);


func drawPath(cellCords: Array[Vector2i], pathMap: TileMap) -> void:
	var i: int = 0;
	for cellPos in cellCords:
		i = i + 1;
		if (cellPos == cellCords.back()):
			pathMap.set_cell(0, cellPos, 0, Vector2i(1, 3));
		else:
			pathMap.set_cell(0, cellPos, 0, Vector2i(2, 3));
		if (i >= self.movement):
			break;

func clearPath(_pth: Array[Vector2i], pathMap: TileMap) -> void:
	pathMap.clear();
	
func getAttackPosCells(targetPos: Vector2i) -> Array[Vector2i]:
	var atkPosCells: Array[Vector2i] = [];
	
	if (self.atkRange == 1):
		atkPosCells.append(Vector2i(targetPos.x + 1, targetPos.y));
		atkPosCells.append(Vector2i(targetPos.x - 1, targetPos.y));
		atkPosCells.append(Vector2i(targetPos.x, targetPos.y + 1));
		atkPosCells.append(Vector2i(targetPos.x, targetPos.y - 1));
	elif (self.atkRange == 2):
		atkPosCells.append(Vector2i(targetPos.x + 2, targetPos.y));
		atkPosCells.append(Vector2i(targetPos.x - 2, targetPos.y));
		atkPosCells.append(Vector2i(targetPos.x, targetPos.y + 2));
		atkPosCells.append(Vector2i(targetPos.x, targetPos.y - 2));
		atkPosCells.append(Vector2i(targetPos.x + 1, targetPos.y + 1));
		atkPosCells.append(Vector2i(targetPos.x - 1, targetPos.y - 1));
		atkPosCells.append(Vector2i(targetPos.x + 1, targetPos.y - 1));
		atkPosCells.append(Vector2i(targetPos.x - 1, targetPos.y + 1));


	atkPosCells.sort_custom(func(a, b):
		var distA: float = sqrt(((a.x - self.targetCellPos.x) * (a.x - self.targetCellPos.x)) + ((a.y - self.targetCellPos.y) * (a.y - self.targetCellPos.y)));
		var distB: float = sqrt(((b.x - self.targetCellPos.x) * (b.x - self.targetCellPos.x)) + ((b.y - self.targetCellPos.y) * (b.y - self.targetCellPos.y)));
		if (self.priorityCells != null):
			distA -= self.priorityCells.find(a);
			distB -= self.priorityCells.find(b);
			
		return distA < distB;
		);

	#var count: int = atkPosCells.size();

	var cellsToRemove: Array[Vector2i] = [];
	for atkCell in atkPosCells:
		if (AStar.isOccupied(self.tilemap, atkCell, self) != null || abs(atkCell.x - self.oldCellPos.x) + abs(atkCell.y - self.oldCellPos.y) > self.movement):
			cellsToRemove.append(atkCell);

	if (cellsToRemove.size() > 0):
		for remove in cellsToRemove:
			atkPosCells.remove_at(atkPosCells.find(remove));

	return atkPosCells;
