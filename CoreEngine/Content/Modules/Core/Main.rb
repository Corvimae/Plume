include CoreEngine::Entities
include CoreEngine::Events

class Main < CoreObject
	def before_load
		EventController.register_event("test")
		call_on_event("test", 0, :test)
		call_on_event("test", 2, :test_first)
		call_on_event("click", 0, :on_click)

		@clicks = 0
	end

	def after_load
		p "Core Module by AcceptableIce: Initialized"
		EventController.fire("test", { test: "Test!" })
	end

	def update

	end
	
	def draw
		
	end

	def on_click(bundle)
		p "Click fire"
		@clicks += 1
	end

	def test(event_bundle)
		p "Test event!"
		p event_bundle.Content
		return event_bundle
	end

	def test_first(event_bundle)
		p "First!"
		return event_bundle
	end
end