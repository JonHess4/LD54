extends EnemyUnit


# Called when the node enters the scene tree for the first time.
func _ready():
	super();

	self.unitName = "Red Dagger";

	self.movement = 7;
	self.maxHp = 2;
	self.currentHp = self.maxHp
	self.atk = 2;
	self.minAtk = 2;
	self.atkRange = 1;

	self.healthBar.max_value = self.maxHp;
	self.healthBar.value = self.currentHp;