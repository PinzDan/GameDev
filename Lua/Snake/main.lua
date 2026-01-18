require 'gameManager'
require 'Splash'

local direction = 'RIGHT'
local Delay = 0.1
local MoveTimer = 0


function love.load()
   Load()
end

function love.draw()
    Draw()
end

function love.update(dt)
    

   Head = GetHead() 

   if Eat(Head) then
    AddPiece()
    CreateMeal()
    AddScore()
    LevelUp()
   end
   if(Collision(Head)) then
    ChangeState('LOSE')
   end
   
--[Vertical Controls]
    if Head.y  < 0 then
        ResetY(WINDOW_HEIGHT) --see GameManger.lua
    end
    if Head.y > WINDOW_HEIGHT  then
        ResetY()
        
    end
 --[Horizontal Controls]
    if Head.x < 0 then
        ResetX(WINDOW_WIDTH) --see GameManger.lua
    end
    if Head.x > WINDOW_WIDTH then
        ResetX()
        
    end

--[Movement]
    
    MoveTimer = MoveTimer+dt
    if(MoveTimer >= Delay)
    then
        MoveTimer = MoveTimer-Delay
        MoveSnakeHor(dt,direction)
    end
       
       
 --[MEAL]
    if not MEAL_EXIST then
        CreateMeal()
    end
    PulseTime = PulseTime + dt
end

function love.keypressed(key)

    if key == "escape" then
        love.event.quit();
    end
    if key == 'enter' or key == 'return' then
        ChangeState('PLAY')
    end

    --[Movement]--
    if key == "d" and direction ~= 'LEFT' then
        direction = 'RIGHT'
    elseif key == "a" and direction ~= 'RIGHT'then
        direction = 'LEFT'
    
    elseif key == "s" and direction ~= 'UP'then
        direction = "DOWN"
    elseif key == "w" and direction ~= 'DOWN' then
        direction = "UP"
    end
end