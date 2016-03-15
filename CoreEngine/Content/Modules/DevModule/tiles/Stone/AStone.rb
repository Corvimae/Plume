require 'DevModule/tiles/Stone/Stone'

include CoreEngine::Entities
include CoreEngine::Scripting

class AStone < Stone
	def	register
		p "Registered AStone";
		self.unregister_draw_on_layer 2
		set_entity_properties({ 
			draw: true, 
			update: false, 
			click: true 
		})
		@clicked = false
	end

	def on_click(event_bundle)
		@clicked = true
	end

	def draw
		super
		click_color = @clicked ? CoreColor.Black : CoreColor.White
		Canvas.draw_string(CoreFont.System, "A", position.x + 14, position.y + 14, click_color)
	end

end