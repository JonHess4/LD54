extends ButtonHelper


# Called when the node enters the scene tree for the first time.
func _ready():
	super();
	self.unit = load("res://scenes/blue_dagger.tscn");
	self.unitName = "BlueDagger";
