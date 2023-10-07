extends EnemyUnit


# Called when the node enters the scene tree for the first time.
func _ready():
	super();

	self.unitName = "Red Sword";

	self.movement = 4;
	self.maxHp = 8;
	self.currentHp = self.maxHp
	self.atk = 5;
	self.minAtk = 2;
	self.atkRange = 1;

	self.healthBar.max_value = self.maxHp;
	self.healthBar.value = self.currentHp;
