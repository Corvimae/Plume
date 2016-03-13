require 'Core/Entities/MapTile'

include CoreEngine::Entities
include CoreEngine::Scripting

class ExStone < MapTile
	def	register
		
	end

	def create(arguments)

	end

	def draw
		p "exstone draw"
	end
end