include CoreEngine::Entities
include CoreEngine::Scripting

class Stone < MapTile
	def	register
		super
	end

	def create
		super
	end

	def draw
		super
		position = self.get_position
		Canvas.draw_string(CoreFont.System, position.x.to_s , position.x * 32, position.y * 32, CoreColor.White)
	end
end