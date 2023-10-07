extends EnemyUnit

var adjacent: Array[Vector2i] = [
	Vector2i(0, 1),
	Vector2i(0, -1),
	Vector2i(1, 0),
	Vector2i(-1, 0)
]

# Called when the node enters the scene tree for the first time.
func _ready():
	super();

	self.unitName = "Red Bomb";

	self.movement = 3;
	self.maxHp = 2;
	self.currentHp = self.maxHp
	self.atk = 0;
	self.minAtk = 0;
	self.atkRange = 1;

	self.healthBar.max_value = self.maxHp;
	self.healthBar.value = self.currentHp;

func attack(_atkTarget: Unit) -> void:
	var num: int = self.rand.randi_range(0, self.adjacent.size() - 1);
	var occupiedUnit: Unit = AStar.isOccupied(self.tilemap, self.oldCellPos + self.adjacent[num], self);
	if (occupiedUnit != null):
		occupiedUnit.modHp(occupiedUnit.currentHp * -1);
	self.tilemap.set_cell(0, self.oldCellPos + self.adjacent[num]);
