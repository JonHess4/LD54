extends AllyUnit


# Called when the node enters the scene tree for the first time.
func _ready():
	super();

	self.unitName = "Blue Tome";

	self.movement = 4;
	self.maxHp = 4;
	self.currentHp = self.maxHp
	self.atk = 3;
	self.minAtk = 2;
	self.atkRange = 2;
	self.canHeal = true;

	self.healthBar.max_value = self.maxHp;
	self.healthBar.value = self.currentHp;
