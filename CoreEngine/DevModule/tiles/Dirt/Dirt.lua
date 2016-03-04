function onDraw() 
	local green = CoreColor.__new(0, 255, 0, 255)
	Canvas.DrawFilledRect(0, 0, 64, 64, CoreColor.White);
	Canvas.DrawRect(0, 0, 64, 64, green);
end