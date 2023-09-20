class_name EnemySpawner
extends Marker2D

@export var path_enemy_manager: NodePath

var enemy_manager: EnemyManager

@onready var spawn_position := self.global_position


func _ready() -> void:
	enemy_manager = get_node(path_enemy_manager)

	await get_tree().process_frame

	var timer := Timer.new()
	add_child(timer)

	timer.wait_time = randf_range(0.5, 1.25)
	timer.one_shot = false
	timer.start()

	timer.connect("timeout", self._on_timer_elapsed)


func _on_timer_elapsed() -> void:
	enemy_manager.Spawn(spawn_position)
