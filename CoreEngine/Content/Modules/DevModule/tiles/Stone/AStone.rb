require 'DevModule/tiles/Stone/Stone'

include CoreEngine::Entities
include CoreEngine::Scripting

class AStone < Stone
	def	register
		p "Registered AStone";
		self.unregister_draw_on_layer 2
	end

	def draw
		super
		Canvas.draw_string(CoreFont.System, "A", position.x + 14, position.y + 14, CoreColor.White)
	end

end