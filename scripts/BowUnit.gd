extends AllyUnit


# Called when the node enters the scene tree for the first time.
func _ready():
	super();

	self.unitName = "Blue Bow";

	self.movement = 4;
	self.maxHp = 6;
	self.currentHp = self.maxHp
	self.atk = 4;
	self.minAtk = 2;
	self.atkRange = 2;

	self.healthBar.max_value = self.maxHp;
	self.healthBar.value = self.currentHp;
