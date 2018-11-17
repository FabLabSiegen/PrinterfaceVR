extends Spatial

var prusacombination = preload("res://Prusacombination.tscn")

func _ready():
	# Find the interface and initialise
	var arvr_interface = ARVRServer.find_interface("OpenVR")
	if arvr_interface and arvr_interface.initialize():
		# switch to ARVR mode
		get_viewport().arvr = true
		
		# workaround for OpenVR not supporting RGBA16F buffers, not needed for GLES2 renderer.
		get_viewport().hdr = false
		
		# make sure vsync is disabled or we'll be limited to 60fps
		OS.vsync_enabled = false
		
		# up our physics to 90fps to get in sync with our rendering
		Engine.target_fps = 90
	
	# just for testing, list what models are available
	var ovr_model = preload("res://addons/godot-openvr/OpenVRRenderModel.gdns").new()
	var model_names = ovr_model.model_names()
	print("models: " + str(model_names))
	var config = File.new();
	config.open("res://printers.conf",File.READ)
	var printername=""
	var address=""
	var key=""
	while !config.eof_reached():
		var line=config.get_line()
		if(line.begins_with("name")):
			if(printername!=""):
				addPrinter(printername,address,key)
			printername=line.split("=")[1]
		if(line.begins_with("address")):
			address=line.split("=")[1]
		if(line.begins_with("key")):
			key=line.split("=")[1]
	if(printername!=""):
		addPrinter(printername,address,key)

func addPrinter(printername,address,key):
	var newprinter = prusacombination.instance()
	newprinter.address=address
	newprinter.key=key
	newprinter.printername=printername
	$Printers.add_child(newprinter)
	newprinter.translation=Vector3(($Printers.get_children().size()-1)*4.5,1,-5)
	newprinter.scale=Vector3(0.01,0.01,0.01)
	pass

func _process(delta):
	# Test for escape to close application, space to reset our reference frame
	if (Input.is_key_pressed(KEY_ESCAPE)):
		get_tree().quit()
	elif (Input.is_key_pressed(KEY_SPACE)):
		# Calling center_on_hmd will cause the ARVRServer to adjust all tracking data so the player is centered on the origin point looking forward
		ARVRServer.center_on_hmd(true, true)
