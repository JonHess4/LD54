extends RichTextLabel


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if (GameEngine.gameOver):
		return;
	
	if (GameEngine.isBuyPhase && self.visible):
		self.visible = false;
	else if (!GameEngine.isBuyPhase && !self.visible):
		self.visible = true;
