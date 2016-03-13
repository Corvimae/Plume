include CoreEngine::Entities
include CoreEngine::Events

class Main < CoreObject
	def before_load
		EventController.register_event("test")
		call_on_event("test", 0, :test);
		call_on_event("test", 2, :test_first);
	end

	def after_load
		p "Core Module by AcceptableIce: Initialized"
		EventController.call("test", { test: "Test!" })
	end

	def update

	end
	
	def draw
		
	end

	def test(eventData)
		p "Test event!"
		p eventData.Content
	end

	def test_first(eventData)
		p "First!"
	end
end