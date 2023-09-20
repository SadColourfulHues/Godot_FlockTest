class_name UnitVisualSyncDelegate
extends Resource


func update_data(
	sprite: AnimatedSprite2D,
	position: Vector2,
	flipped: bool) -> void:

	sprite.position = position
	sprite.flip_h = flipped


func spawn(
	sprite: AnimatedSprite2D,
	frames: SpriteFrames,
	position: Vector2) -> void:

	sprite.sprite_frames = frames
	sprite.position = position

	sprite.play()

	# In tween
	sprite.modulate = Color.TRANSPARENT

	var in_tween := sprite.create_tween()
	in_tween.tween_property(sprite, "modulate", Color.WHITE, 0.5)


func flash(sprite: AnimatedSprite2D) -> void:
	sprite.modulate = Color.WHITE

	var tween := sprite.create_tween()

	tween.tween_property(sprite, "modulate", Color.RED, 0.1)
	tween.tween_property(sprite, "modulate", Color.WHITE, 0.15)

