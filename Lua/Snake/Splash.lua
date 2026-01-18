local DefaultFont = love.graphics.newFont('font.ttf', 72)
local SmallFont = love.graphics.newFont('font.ttf',20)

local Welcome = "Welcome to Snake"


local alpha = 1
local amplitude = 0.9
PulseTime = 0

function SplashDraw()
    love.graphics.setFont(DefaultFont)
    love.graphics.setColor(1,1,1,alpha+amplitude*math.sin(PulseTime*2)+0.1)
    love.graphics.printf(Welcome, 0 , WINDOW_HEIGHT/4, WINDOW_WIDTH, 'center')
    
    love.graphics.setFont(SmallFont)
    love.graphics.printf("By DanPinz", 0, WINDOW_HEIGHT/3 +30,WINDOW_WIDTH,'center')
    
    love.graphics.printf("Press Enter Key . . .", 0, WINDOW_HEIGHT/2 +30,WINDOW_WIDTH,'center')
    
    
end
