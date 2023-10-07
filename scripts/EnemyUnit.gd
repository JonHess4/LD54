extends Unit
class_name EnemyUnit

var idleTexture: Texture2D;
var targetedTexture: Texture2D;
var tickRate: float = 0.3;
var tickTimer: float = 0;
var isTurnStarted: bool = false;
var isReachedNextTile: bool = false;

# Called when the node enters the scene tree for the first time.
func _ready():
	super();
	
	self.isTeamTurn = false;
	self.isEnemy = true;
	self.idleTexture = self.texture;
	self.isTurn = false;

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
		
	if (self.isTurn):
		if (!self.isTurnStarted):
			self.isTurnStarted = true;
			self.isReachedNextTile = false;
			self.findTargetAndPath();
		if (self.path == null || self.path.size() <= 0):
			print("No Path: " + self.unitName);
			self.isTurnStarted = false;
			self.endTurn();
			return;
			
		self.tickTimer += delta;
		if (self.tickTimer >= self.tickRate):
			print("Tick: " + str(self.tickTimer));
			self.tickTimer = 0;
			self.isReachedNextTile = true;
			self.position = self.tilemap.map_to_local(self.path.front()) + self.posMod;
			
		var destPos: Vector2 = self.tilemap.map_to_local(self.path.front()) + self.posMod;
		if (self.position != destPos):
			print("GoGo");
			if (self.position.distance_to(destPos) <= 3):
				print("Close Enough");
				self.position = destPos;
				self.isReachedNextTile = true;
				self.tickTimer = 0;
			else:
				print("Math");
				var oldPos: Vector2 = self.tilemap.map_to_local(self.oldCellPos);
				self.position = oldPos + ((destPos - oldPos) * (self.tickTimer / self.tickRate));
		else:
			self.isReachedNextTile = true;
			
		if (self.isReachedNextTile):
			print("TileReached");
			self.oldCellPos = self.tilemap.local_to_map(self.position);
			self.isReachedNextTile = false;
			if (self.path.size() == 1):
				print("One Tile Left");
				self.targetCellPos = self.path.back();
				if (self.target != null && is_instance_valid(self.target)):
					var newTargetCellPos: Vector2i = self.tilemap.local_to_map(self.target.position);
					if (abs(self.targetCellPos.x - newTargetCellPos.x) + abs(self.targetCellPos.y - newTargetCellPos.y) <= self.atkRange):
						self.attack(self.target);
					else:
						self.attack(null);
			self.path.remove_at(0);
			if (self.path.size() <= 0):
				print("End Turn");
				self.isTurnStarted = false;
				self.endTurn();

func getTargetedTexture() -> Texture2D:
	if (self.targetedTexture == null):
		self.targetedTexture = load("res://sprites/them-targeted.png");
	return self.targetedTexture;

func setIsTargeted(newIsTargeted: bool) -> void:
	if (self.isTargeted != newIsTargeted):
		if (newIsTargeted):
			self.texture = self.getTargetedTexture();
		else:
			self.texture = self.idleTexture;
	self.isTargeted = newIsTargeted;

func findTargetAndPath() -> void:
	var dist: float = 1000;
	var units: Array[Unit] = GameEngine.getUnits();
	for unit in units:
		if (unit.isEnemy != self.isEnemy):
			var otherDist: float = self.position.distance_to(unit.position);
			if (otherDist < dist):
				dist = otherDist;
				self.target = unit;
	
	if (self.target != null):
		self.path = AStar.findPath(self.tilemap.local_to_map(self.position), self.tilemap.local_to_map(self.target.position), self.tilemap, self, self.priorityCells).slice(0, self.movement);
		while (self.path.size() > 1):
			self.path.remove_at(self.path.size() - 1);
			var occupantUnit: Unit = AStar.isOccupied(self.tilemap, self.path.back(), self);
			if (occupantUnit == null):
				break;
				
func attack(atkTarget: Unit) -> void:
	super(atkTarget);
