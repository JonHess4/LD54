extends RichTextLabel


# Called when the node enters the scene tree for the first time.
func _ready():
	self.text = GameEngine.endGameText;


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if (GameEngine.gameOver && !self.visible):
		self.text = GameEngine.endGameText;
		self.visible = true;
	elif (!GameEngine.gameOver && self.visible):
		self.visible = false;

