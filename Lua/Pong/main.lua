push = require 'push'

Class = require 'class'

require 'Paddle'

require 'Ball'

WINDOW_WIDTH = 1280
WINDOW_HEIGHT = 720

VIRTUAL_WIDTH = 432
VIRTUAL_HEIGHT = 243

PADDLE_SPEED = 150

defaultFont = love.graphics.newFont('font.ttf', 8)

scoreFont = love.graphics.newFont('font.ttf', 72)   

player1Score = 0
player2Score = 0

Winner = ''

PLAYER1_Y = 30
PLAYER2_Y = VIRTUAL_HEIGHT - 50

BALL_DX = VIRTUAL_WIDTH / 2 - 2;
BALL_DY = VIRTUAL_HEIGHT / 2 - 2;

CENTER_X = VIRTUAL_WIDTH / 2 - 2;
CENTER_Y = VIRTUAL_HEIGHT / 2 - 6;

gameState = 'start'

mainString = 'Welcome to Pong! Press Enter to Play'

function love.load()

    math.randomseed(os.time())

    

    love.window.setTitle('Pong! - by DanPinz')
    love.graphics.clear(1,1,1,1);
    love.graphics.setFont(defaultFont);

    sounds = {
        ['paddle_hit'] = love.audio.newSource('Sound/Bfxr/Pickup2.wav', 'static'),
        ['score'] = love.audio.newSource('Sound/Bfxr/PowerUp2.wav', 'static'),
        ['wall_hit'] = love.audio.newSource('Sound/Bfxr/Boom.wav', 'static')
    }

    push:setupScreen(
        VIRTUAL_WIDTH,
        VIRTUAL_HEIGHT,
        WINDOW_WIDTH,
        WINDOW_HEIGHT,
        {
            fullscreen = false,
            resizable = false,
            vsync = true
        }
    )


    Ball = Ball(0,0, 4, 4);
    Ball:reset();
    Paddle1 = Paddle(10, PLAYER1_Y, 5, 30);
    Paddle2 = Paddle(VIRTUAL_WIDTH - 10, PLAYER2_Y, 5, 30);
end

function love.draw()
    push:apply('start');
    love.graphics.clear(40/255, 45/255, 52/255, 255/255);

    love.graphics.setDefaultFilter('nearest', 'nearest');
    
    love.graphics.setFont(defaultFont);
    if(gameState == 'pause') then
        love.graphics.printf(
            'Game Paused. Press P to Resume',          
            0,                      
            CENTER_Y,  
            VIRTUAL_WIDTH,           
            'center'               
        )
    end

    if(gameState == 'done') then
        love.graphics.printf(
            Winner,          
            0,                      
            CENTER_Y - 20,  
            VIRTUAL_WIDTH,           
            'center'               
        )
    end 

    if(gameState == 'start') then
        love.graphics.printf(
            mainString,
            0,                      
            CENTER_Y,  
            VIRTUAL_WIDTH,           
            'center'               
        )
    

        love.graphics.setFont(scoreFont);
        love.graphics.print(
            tostring(player1Score),          
            40,                      
            VIRTUAL_HEIGHT/2          
        )
        love.graphics.print(
            tostring(player2Score),          
            VIRTUAL_WIDTH-80,                   
            VIRTUAL_HEIGHT/2            
        )

    
        love.graphics.printf(
            'Hello Pong!',          
            0,                      
            20,  
            VIRTUAL_WIDTH,           
            'center')              
        
    end

    -- Play state rendering
    if gameState == 'play' then
        mainString = 'Press enter to continue!'
        -- paddles
        Paddle1:render();
        Paddle2:render();

        -- ball
        Ball:render();
    end

    push:apply('end');
end

function love.keypressed(key)
    if key == 'escape' then
        love.event.quit()
    end

    if(key == 'p') then
        gameState = (gameState == 'pause') and 'play' or 'pause';
    end
   
    if key == 'enter' or key == 'return' then
        -- If we are at the start screen, Enter begins play.
        if gameState == 'start' then
            gameState = 'play'

        -- Winner check
        elseif gameState == 'done' then
            gameState = 'start'
            player1Score = 0
            player2Score = 0
            Winner = ''
            -- reset paddles to their initial Y positions
            Paddle1.y = PLAYER1_Y
            Paddle2.y = PLAYER2_Y
            Ball:reset()
        end
    end
end


function love.update(dt)


    
    if gameState == 'pause' then
        return;
    end
    -- player 1 movement
    if( love.keyboard.isDown('w')) then
        Paddle1.dy = -PADDLE_SPEED;
    elseif( love.keyboard.isDown('s')) then
        Paddle1.dy = PADDLE_SPEED;
    end

    if Ball:collison(Paddle1) then
        sounds['paddle_hit']:play();
        Ball.dx = -Ball.dx * 1.03;
        Ball.x = Paddle1.x + Paddle1.width;

        if Ball.dy < 0 then
            Ball.dy = -math.random(10, 150);
        else
            Ball.dy = math.random(10, 150);
        end
    end

    if Ball:collison(Paddle2) then
        sounds['paddle_hit']:play();
        Ball.dx = -Ball.dx * 1.03;
        Ball.x = Paddle2.x - Ball.width;

        if Ball.dy < 0 then
            Ball.dy = -math.random(10, 150);
        else
            Ball.dy = math.random(10, 150);
        end
    end

        if Ball.y <= 0 then
            sounds['wall_hit']:play();
            Ball.y = 0
            Ball.dy = -Ball.dy
        end

        -- -4 to account for the ball's size
        if Ball.y >= VIRTUAL_HEIGHT - 4 then
            sounds['wall_hit']:play();
            Ball.y = VIRTUAL_HEIGHT - 4
            Ball.dy = -Ball.dy
        end
    Paddle1:update(dt);



    -- player 2 movement
    if( love.keyboard.isDown('up')) then
        Paddle2.dy = -PADDLE_SPEED;
    elseif( love.keyboard.isDown('down')) then
        Paddle2.dy = PADDLE_SPEED;
    end
    Paddle2:update(dt);

    if gameState == 'play' then
        Ball:update(dt);

        if(Ball.x < 0) then
            sounds['score']:play();
            player2Score = player2Score + 1;
            Ball:reset();
            if(player2Score == 1) then
                gameState = 'done'
                Winner = 'Player 2 Wins!'
                return
            end
            gameState = 'start';
        end
        if(Ball.x > VIRTUAL_WIDTH) then
            sounds['score']:play();
            player1Score = player1Score + 1;
            Ball:reset();
            if(player1Score == 1) then
                gameState = 'done'
                Winner = 'Player 1 Wins!'
                return
            end
            gameState = 'start';
        end
    end


end





