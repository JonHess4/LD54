extends Sprite2D
class_name Unit;

var maxHp: int;
var currentHp: int;
var minAtk: int;
var atk: int;
var movement: int;
var healthBar: ProgressBar;
var isTurn: bool;
var atkRange: int;
var canHeal: bool = false;
var unitDetails: RichTextLabel;

var unitName: String;
var tilemap: TileMap;
var posMod: Vector2 = Vector2i(0, 0);
var oldCellPos: Vector2i;
var targetCellPos: Vector2i;
var path: Array[Vector2i];
var target: Unit;
var isTargeted: bool;
var isEnemy: bool;
var oldTarget: Unit;
var isTeamTurn: bool = true;
var damageText: RichTextLabel;
var damageTextDisplayTime: float;
var priorityCells: Array[Vector2i] = [];
var rand: RandomNumberGenerator;

# Called when the node enters the scene tree for the first time.
func _ready():
	GameEngine.addUnit(self);
	self.tilemap = self.get_node("../TileMap");
	self.healthBar = self.get_node("./HealthBar");
	self.unitDetails = self.get_node("../UnitDetails");
	self.damageText = self.get_node("./DamageText");
	var sbf: StyleBoxFlat = StyleBoxFlat.new();
	self.healthBar.add_theme_stylebox_override("fill", sbf);
	sbf.bg_color = Color("C0483D");
	
	self.oldCellPos = self.tilemap.local_to_map(self.position);
	self.targetCellPos = oldCellPos;
	self.position = self.tilemap.map_to_local(oldCellPos) + self.posMod;
	
	self.rand = RandomNumberGenerator.new();

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if (GameEngine.gameOver):
		return;
		
func setIsTargeted(newIsTargeted: bool) -> void:
	self.isTargeted = newIsTargeted;

func getIsTargeted() -> bool:
	return self.isTargeted;
	
func modHp(mod: int) -> void:
	self.currentHp += mod;
	self.healthBar.value = self.currentHp;
	self.damageText.text = str(mod);
	if (self.currentHp <= 0):
		GameEngine.removeUnit(self);
		self.queue_free();
		
func attack(atkTarget: Unit) -> void:
	if (atkTarget != null):
		var mod: int = self.rand.randi_range(self.minAtk, self.atk) * -1;
		if (self.canHeal && atkTarget.isEnemy == self.isEnemy):
			mod = mod * -1;
		atkTarget.modHp(mod);
		
func endTurn() -> void:
	self.disable();
	GameEngine.checkEndTurn(self.isEnemy);
	
func readyTurn() -> void:
	self.enable();

func resetColor() -> void:
	modulate = Color(1, 1, 1);
	
func disable() -> void:
	self.isTurn = false;
	self.modulate = Color(0.5, 0.5, 0.5);
	
func enable() -> void:
	self.resetColor();
	self.isTurn = true;
	
func showUnitDetails() -> void:
	self.unitDetails.text = "Unit:\t\t" + self.unitName + " (?)" + \
	"\nHP:\t\t" + str(self.currentHp) + "/" + str(self.maxHp) + \
	"\nAtk:\t\t" + str(self.minAtk) + "-" + str(self.atk) + \
	"\nHeal:\t" + (str(self.minAtk) + "-" + str(self.atk) if self.canHeal else "-") + \
	"\nMove:\t" + (str(self.movement - 2) if self.isEnemy else str(1)) + \
	"\nRange:\t" + str(self.atkRange) + \
	"\nTurn:\t" + ("Ready" if self.isTurn else ("Taken" if self.isTeamTurn else "Waiting"));
	if (self.unitName.to_lower().contains("sword")):
		self.unitDetails.tooltip_text = "Swords are slow, tanky, and hit hard.";
	elif (self.unitName.to_lower().contains("tome")):
		self.unitDetails.tooltip_text = "Tomes can either attack enemies or heal allies.";
	elif (self.unitName.to_lower().contains("bow")):
		self.unitDetails.tooltip_text = "Bows can attack at range.";
	elif (self.unitName.to_lower().contains("dagger")):
		self.unitDetails.tooltip_text = "Daggers are highly mobile, with low attack and hp.";
	elif (self.unitName.to_lower().contains("bomb")):
		self.unitDetails.tooltip_text = "Bombs destroy one random adjacent tile each turn,\nalong with any unit that may be standing on it at the time.";

func upgrade() -> void:
	self.maxHp += self.rand.randi_range(0, 3);
	self.currentHp = max(self.maxHp, self.currentHp + self.rand.randi_range(0, 3));
	self.minAtk += self.rand.randi_range(0, 2);
	self.atk = max(self.minAtk, self.atk + self.rand.randi_range(0, 2));
	self.movement += self.rand.randi_range(0, 1);
	self.healthBar.max_value = self.maxHp;
	self.healthBar.value = self.currentHp;

