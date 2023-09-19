extends Label

var score: int = 0
var tween: Tween = null


func _ready() -> void:
	__update_score_label(true)


func _on_entity_slain() -> void:
	score += 1
	__update_score_label()


func __update_score_label(skip_animation: bool = false) -> void:
	self.text = "Score: %d" % score

	if skip_animation:
		return

	# Animation effect
	if is_instance_valid(tween) && tween.is_running():
		self.scale = Vector2.ONE
		tween.stop()

	# Keep control centred
	self.pivot_offset = 0.5 * self.size

	tween = create_tween()
	tween.tween_property(self, "scale", Vector2.ONE * 1.15, 0.1)
	tween.tween_property(self, "scale", Vector2.ONE, 0.25)
