 --[[
  Description: party leader farm trial
  Author: McVaxius
]]

--just kind of proof of concept farming script for easy duties that vbm AI mode can solve
--make sure you go into settings and disable the snd targeting
--you need following plugins
--visland, SND (croizat fork), pandora
--simpletweaks -> turn on maincommand
--turn on auto interact on pandora set distance to 5 dist 5 height
--turn on vbm. click ai mode -> click follow. then form the group.
--turn on rotation solver if you like, set your lazyloot to /fulf need/green/pass/off etc
--preselect port decumana in the duty finder menu.
--meant for premade party but could be used for duty support
--enjoy

--you could concieveably just change "The Ultima Weapon" to another target if you were say.. wanting to farm a different trial.
--target stuff
local char_snake = "The Ultima Weapon" -- the target to follow.  4 means character slot 4, slot 1 is us, dont use 1
local repeat_trial = 1 --number of times to repeat the trial.  we could also contemplate repeating until current char reaches a specific level also...
local repeated_trial = 0 --counter init
local repeat_type = 0 --0 is premade, 1 is trust

--distance stuf
local snake_deest = 5 -- distance max to the specific char so we can decide when to start moving
local enemy_deest = 1 -- distance max to the specific enemy to beeline to the enemy using navmesh. set this to a higher value than snake_deest if you want it to never follow the enemy.
local meh_deest = 40 -- distance max to char_snake where we stop trying to follow or do anything. maybe look for interaction points or exits?
local enemeh_deest = 5 -- distance max to battle target

--placeholder for target location
local currentLocX = 1
local currentLocY = 1
local currentLocZ = 1

--our current location
local mecurrentLocX = GetPlayerRawXPos(tostring(1))
local mecurrentLocY = GetPlayerRawYPos(tostring(1))
local mecurrentLocZ = GetPlayerRawZPos(tostring(1))

--our current area
local we_are_in = 666
local we_were_in = 666

--declaring distance var
local dist_between_points = 500

--how long to lock on for. we only do it once every lockon_wait seconds. this will interfere less with VBM dodging mechanics
local dont_lockon = 0
local lockon_wait = 5

local neverstop = true
local i = 0

local function distance(x1, y1, z1, x2, y2, z2)
    return math.sqrt((x2 - x1)^2 + (y2 - y1)^2 + (z2 - z1)^2)
end

local function setdeest()
	if currentLocX and currentLocY and currentLocZ and mecurrentLocX and mecurrentLocY and mecurrentLocZ then
		dist_between_points = distance(currentLocX, currentLocY, currentLocZ, mecurrentLocX, mecurrentLocY, mecurrentLocZ)
		-- dist_between_points will contain the distance between the two points
		yield("/echo Distance between char_snake and point 1: " .. dist_between_points)
	else
		yield("/echo Failed to retrieve coordinates for one or both points.")
		dist_between_points = 500 -- default value haha
	end
end

yield("/echo starting.....")

while repeated_trial < repeat_trial do
	currentLocX = GetPlayerRawXPos(tostring(char_snake))
	currentLocY = GetPlayerRawYPos(tostring(char_snake))
	currentLocZ = GetPlayerRawZPos(tostring(char_snake))
	mecurrentLocX = GetPlayerRawXPos(tostring(1))
	mecurrentLocY = GetPlayerRawYPos(tostring(1))
	mecurrentLocZ = GetPlayerRawZPos(tostring(1))
	yield("/targetenemy") --this will trigger RS to do stuff.
	if GetCharacterCondition(34)==false then --if we are not in a duty --try to restart duty
		yield("/visland stop")
		yield("/wait 2")
		yield("/echo We seem to be outside of the duty.. let us enter!")
		yield("/wait 5")	
		if repeat_type == 0 then --4 Real players (or scripts haha) using duty finder
			yield("/finder")
			yield("/echo attempting to trigger duty finder")
			yield("/pcall ContentsFinder true 12 0")
		end
		if repeat_type == 1 then --just you using duty support
			--("/pcall DawnStory true 20") open the window.. how?
			--we use simpletweaks
			yield("/maincommand Duty Support")
			yield("/wait 2")
			yield("/echo attempting to trigger duty support")
			yield("/pcall DawnStory true 11 0") --change tab to first tab
			yield("/pcall DawnStory true 12 35")--select port decumana
			yield("/wait 2")
			yield("/pcall DawnStory true 14") --START THE DUTY
		end
		repeated_trial = repeated_trial + 1
		yield("/echo Total Trials triggered for "..char_snake..": "..repeated_trial)
		yield("/wait 10")
	end
	if GetCharacterCondition(26)==false and GetCharacterCondition(34)==true then --if we are not in combat AND we are in a duty then we will look for an exit
		yield("/target exit")
		yield("/wait 0.1")
		if GetTargetName()=="Exit" then --get out ! assuming pandora setup for auto interaction
			yield("/lockon on")
			yield("/automove on")
			yield("/wait 10")
		end
	end
	--test dist to the intended party leader
	if GetCharacterCondition(34)==true then --if we are in a duty
		setdeest()
		if dist_between_points > snake_deest and dist_between_points < meh_deest then
				--yield("/vnavmesh moveto "..currentLocX.." "..currentLocY.." "..currentLocZ)
				yield("/visland moveto "..currentLocX.." "..currentLocY.." "..currentLocZ)
				--yield("/echo vnavmesh moveto "..math.ceil(currentLocX).." "..math.ceil(currentLocY).." "..math.ceil(currentLocZ))
		end
	end
	--test dist to the intended party leader
	i = 0
	if GetCharacterCondition(28)==true then --if we are bound by qte
		if GetCharacterCondition(29)==true then --if we are bound by qte
			while i < 150 do
				i = i + 1
				yield("/send SPACE")
				yield("/send SPACE")
				yield("/wait 0.1")
				if GetCharacterCondition(28)==false then --if we are not bound by qte get out of the space bar spamming so we can resume following or whatever
					i = 150
				end
			end
		end
	end
	yield("/wait 1")
end