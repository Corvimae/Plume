include CoreEngine::Entities
include CoreEngine::Scripting
include CoreEngine::Events

class Main < CoreObject
	def before_load
		p "Module loading..."
		EventController.register_event("pause")
		p "Registering ExStone"
		register_texture("excavator", "excavator.png");
		register_animation("ex_anim", "excavator", 32, 64, 3, 6);

		call_on_event("pause", 0, :toggle_paused)

		@animation = get_animation_instance("ex_anim")
		@frame = 0
	end

	def after_load
		p "Module loaded!"
	end

	def update
		@frame += 1
	end

	def toggle_paused(bundle)
		if @animation.Paused
			@animation.resume
		else
			@animation.pause
		end
		return bundle
	end
	
	def draw
		Canvas.draw_animation(@animation, 0, 0, CoreColor.White)
		if @frame % 100 == 0
			@animation.FlipHorizontal = @animation.FlipHorizontal ? false : true
		end

		if @frame % 1000 == 0
			@animation.FlipVertical = @animation.FlipVertical ? false : true
		end
	end
end