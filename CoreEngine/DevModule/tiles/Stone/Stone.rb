include CoreEngine::Entities
include CoreEngine::Scripting

class Stone < MapTile
	def	register
		super
		self.SetEntityProperties({ draw: true, update: true })
		self.DrawOnLayer(2, :second_draw)
		p "Registering 1"
	end

	def create
		super
		@iterations = 0
	end

	def draw
		super
		Canvas.draw_string(CoreFont.System, @iterations.to_s, position.x, position.y, CoreColor.White)
	end

	def update
		@iterations += 1
	end

	def second_draw
		Canvas.DrawFilledRect(position.x, position.y, 4, 4, CoreColor.Black)
	end
end