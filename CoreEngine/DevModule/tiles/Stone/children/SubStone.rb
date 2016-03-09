require 'Stone'

include CoreEngine::Entities
include CoreEngine::Scripting

class SubStone < Stone
	def	register
		super
		p "Registered FrownStone";
		#self.unregister_draw_on_layer 2
	end

	def create
		@iterations = 0
	end

	def draw
		Canvas.draw_string(CoreFont.System, ":(", position.x + 14, position.y + 14, CoreColor.White)
	end

end