class_name Tile

var x: int;
var y: int;
var cost: int;
var distance: float;
var parent: Tile;

func costDistance() -> float:
	return self.cost + self.distance;

func setDistance(targetX: int, targetY: int) -> void:
	self.distance = sqrt(((targetX - self.x) * (targetX - self.x)) + ((targetY - self.y) * (targetY - self.y)));
