local gameState = 'SPLASH'
local X_START = 0;
local Y_START = 0;

X = WINDOW_WIDTH/2
Y = WINDOW_HEIGHT/2


local SnakesPieces = {}

S_WIDTH = 30
S_HEIGHT = 30

SPEED = 4

local Score = 0
local Level = 1

--[Visual Effect to Meal]
local alpha = 1
local amplitude = 0.9
PulseTime = 0


local DefaultFont = love.graphics.newFont('font.ttf', 32)

function Load()

    math.randomseed(os.time())
    CreateSnake()
    CreateMeal()
    
end


function Draw()

    if gameState == 'PLAY' then
        love.graphics.clear()
        --[Score]
        love.graphics.setFont(DefaultFont)
        love.graphics.print("Score: ".. GetScore(), X_START+20, Y_START+20,0)
        love.graphics.print("Level: ".. GetLevel(), X_START+20, Y_START+60,0)
        --[Create The Snake]
        love.graphics.setColor(1.0,0.35, 0.4, 1.0)
        for index, value in ipairs(SnakesPieces) do
            love.graphics.rectangle('fill',value.x,value.y, S_WIDTH,S_HEIGHT, 30);
        end
        
        


        --[CreateMeal]
        if MEAL_EXIST then
            love.graphics.setColor(0.4, 0.8, 0.2, alpha+amplitude*math.sin(PulseTime*4)+0.1) --Use a sine wave based on elapsed time to smoothly oscillate the alpha value,
            MEAL = love.graphics.rectangle('fill',MEAL_X,MEAL_Y, MEAL_WIDTH,MEAL_HEIGHT,30);
        end
    else
        SplashDraw()
    end
    
end

--[Change State]
function ChangeState(state)
    gameState = state
end

--[ Snake settings]

function CreateSnake()
    --Start with 3 pieces
    SnakesPieces = {
        {x = X, y = Y},
    }    
    AddPiece()
    AddPiece()
end

function GetHead()
    return { x = SnakesPieces[1].x, y = SnakesPieces[1].y }
end

function AddPiece() --pushLeft
    local head = SnakesPieces[#SnakesPieces]
    TailX = head.x -S_WIDTH
    TailY = head.y
    table.insert(SnakesPieces, {x=TailX, y=TailY})
end

function MoveSnakeHor(dt,direction)
            
        for i=#SnakesPieces,2,-1 do
            SnakesPieces[i].x = SnakesPieces[i-1].x 
            SnakesPieces[i].y = SnakesPieces[i-1].y
        end

        if(direction == 'LEFT') then
            SnakesPieces[1].x = SnakesPieces[1].x- S_WIDTH 
        elseif (direction == 'RIGHT') then
            SnakesPieces[1].x = SnakesPieces[1].x+S_WIDTH
        elseif direction=='DOWN' then
            SnakesPieces[1].y = SnakesPieces[1].y+S_HEIGHT
        else
            SnakesPieces[1].y = SnakesPieces[1].y-S_HEIGHT
        end
end


function Collision(Head)
    if #SnakesPieces> 2 then
		for i=2,#SnakesPieces do
			if SnakesPieces[i].x == Head.x and SnakesPieces[i].y == Head.y then
				love.event.quit()
				break
			end
		end
	end
-- nessuna collisione
end


function ResetY(new_Y)
   SnakesPieces[1].y = new_Y or Y_START
end

function ResetX(new_X)
     SnakesPieces[1].x = new_X or X_START
end

--[Meal]
MEAL_X = 0
MEAL_Y = 0

MEAL_WIDTH = 20
MEAL_HEIGHT = 20


local status = {
    EXIST = true,
    NOTEXIST = false
}

MEAL_EXIST = status.NOTEXIST
function CreateMeal()


    MEAL_X = math.random(X_START,WINDOW_WIDTH)
    MEAL_Y = math.random(Y_START,WINDOW_HEIGHT)
    
    MEAL_EXIST = status.EXIST
    
end

function Eat(Head)
    
    if(MEAL_X) > Head.x + S_WIDTH or Head.x > MEAL_X + MEAL_WIDTH then
        return false
    end
    if(MEAL_Y) > Head.y + S_HEIGHT or Head.y > MEAL_Y + MEAL_HEIGHT then
        return false
    end

    return true
end

function AddScore()
    Score = Score + Level*2
end

function LevelUp()
    Level = Level+1
    SPEED = SPEED *1.5
end

function GetScore()
    return Score
end

function GetLevel()
    return Level
end

