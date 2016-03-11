include CoreEngine::Entities
include CoreEngine::Scripting

class MapTile < BaseEntity
	def register
		register_texture("texture", self.Metadata.name + ".png")
		set_entity_properties({ draw: true, update: false })
	end

	def create(arguments)
		super
		@coordinates = Canvas.create_vector2(arguments[0], arguments[1])
		set_position(arguments[0] * 32, arguments[1] * 32)
		set_draw_dimensions(32, 32);
		@texture = get_texture("texture")
	end

	def draw
		position = get_position
		Canvas.draw_texture(@texture, position.X, position.Y, CoreColor.White)
	end

end