extends AllyUnit


# Called when the node enters the scene tree for the first time.
func _ready():
	super();

	self.unitName = "Blue Sword";

	self.movement = 3;
	self.maxHp = 8;
	self.currentHp = self.maxHp
	self.atk = 6;
	self.minAtk = 3;
	self.atkRange = 1;

	self.healthBar.max_value = self.maxHp;
	self.healthBar.value = self.currentHp;
