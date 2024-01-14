--[[
  Description: Updated Deliver and clean up script for using deliveroo and visland.
  Author: McVaxius
  Link: https://discord.com/channels/1162031769403543643/1162799234874093661/1190858719546835065
]]

--arrays so we can easily transfer between PC and configure for other lists of retainers
local folderPath = "D:/FF14/!gil/"
--filename will be FCranks

local chars_FCBUFF = {
 'firstname lastname@server',
 'firstname lastname@server'
}

local chars_gridania = {
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server'
}

local chars_uldah = {
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server'
}

local chars_toilet = {
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server',
 'firstname lastname@server'
}

local charFCS = {
	['firstname lastname'] = 'FCNEM',
	['firstname lastname'] = 'FCNEM',
	['firstname lastname'] = 'FCNEM',
	['firstname lastname'] = 'FCNEM',
	['firstname lastname'] = 'FCNEM',
	['firstname lastname'] = 'FCNEM',
	['firstname lastname'] = 'FCNEM',
	['firstname lastname'] = 'FCNEM'
}

--total retainer abusers and starting the counter at 1
total_rcucks = 22
rcuck_count = 1

--do we bother with fc buffs? 0 = no 1 = yes
process_fc_buffs = 1
process_gridania = 1
process_uldah = 1
process_toilet = 1

function recordFCRANK()
	local file = io.open(folderPath .. "FCranks.txt", "a")
    local currentTime = os.date("*t")
    local formattedTime = string.format("%04d-%02d-%02d %02d:%02d:%02d", currentTime.year, currentTime.month, currentTime.day, currentTime.hour, currentTime.min, currentTime.sec)
	if file then
		-- Write text to the file
		--file:write("Hello, this is some text written to a file using Lua!\n")
		file:write(formattedTime.." - Char:"..GetCharacterName().." - FC: "..charFCS[GetCharacterName()].." - FC Rank - "..GetFCRank().."\n")
		-- Close the file handle
		file:close()
		yield("/echo Text has been written to '" .. folderPath .. "FCranks.txt'")
	else
		yield("/echo Error: Unable to open file for writing")
	end
end

function Final_GC_Cleaning()
	--turn around in case we aren't facing the correct way
	--this attempts to target serpent or flame personnel or even storm.  assuming you have a separate line in a hotkey for each type.
	--the purpose of this section is to get your char to face the npcs and orient the camera properly. otherwise the rest of the script might die

	--no targeting needed with deliveroo
	--yield("/send KEY_3")
	--yield("/wait 1")
	--yield("/lockon on")
	--yield("/automove on")
	--yield("/wait 1")

	--deliveroo i choose you
	yield("/deliveroo enable")
	yield("/wait 1")

	--loop until deliveroo done
	dellyroo = true
	dellyroo = DeliverooIsTurnInRunning()
	dellycount = 0
	while dellyroo do
		yield("/wait 5")
		dellyroo = DeliverooIsTurnInRunning()
		dellycount = dellycount + 1
		yield("/echo Processing Retainer Abuser "..rcuck_count.."/"..total_rcucks)
		if dellycount > 100 then
			--do some stuff like turning off deliveroo and targeting and untargeting an npc
			--i think we just need to target the quartermaster and open the dialog with him
			--this will solve getting stuck on deliveroo doing nothing
			dellycount = 0
		end
	end

	--added 5 second wait here because sometimes they get stuck. altho its been biological life form so far....
	yield("/wait 5")
	yield("/tp Estate Hall")
	yield("/wait 1")
	--yield("/waitaddon Nowloading <maxwait.15>")
	yield("/wait 15")
	yield("/waitaddon NamePlate <maxwait.600><wait.5>")

    yield("/hold W <wait.1.0>")
    yield("/release W")
	yield("/target Entrance <wait.1>")
	yield("/lockon on")
	yield("/automove on <wait.2.5>")
	yield("/automove off <wait.1.5>")
	yield("/hold Q <wait.2.0>")
    yield("/release Q")

	--Code for opening venture coffers
	yield("/wait 3")
	yield("/echo Number of Venture Coffers to open: "..GetItemCount(32161))
	VCnum = GetItemCount(32161)
	while (VCnum > 0) do
		--this is no longer reliable
		--yield("/item Venture Coffer")
		yield("/send X")
		yield("/wait 6")
		VCnum = GetItemCount(32161)
		yield("/echo Number of Venture Coffers left: "..GetItemCount(32161))
	end
	--yield("/autorun off")
	--Code for opening FC menu so allagan tools can pull the FC points
	yield("/freecompanycmd")
	recordFCRANK()
	yield("/wait 3")
end

function visland_stop_moving()
 yield("/equipguud")
 yield("/wait 3")
 muuv = 1
 muuvX = GetPlayerRawXPos()
 muuvY = GetPlayerRawYPos()
 muuvZ = GetPlayerRawZPos()
 while muuv == 1 do
	yield("/wait 1")
	if muuvX == GetPlayerRawXPos() and muuvY == GetPlayerRawYPos() and muuvZ == GetPlayerRawZPos() then
		muuv = 0
	end
	muuvX = GetPlayerRawXPos()
	muuvY = GetPlayerRawYPos()
	muuvZ = GetPlayerRawZPos()
 end
 yield("/echo movement stopped - time for GC turn ins or whatever")
 yield("/visland stop")
 yield("/wait 3")
end

function open_aetheryte()
 yield("/waitaddon NamePlate <wait.1>")
 yield("/wait 10")
 yield("/target Aetheryte <wait.1>")
 yield("/lockon")
 yield("/automove")
 yield("/send E")
 yield("/wait 0.5")
 yield("/send E")
 yield("/wait 0.5")
 yield("/send E")
 yield("/wait 0.5")
 yield("/send E")
 yield("/wait 0.5")
 yield("/send E")
 yield("/wait 0.5")
 yield("/pinteract <wait.1>")
 yield("/pcall SelectString true 0")
end

--first turn on FC buffs
if process_fc_buffs == 1 then
	for _, char in ipairs(chars_FCBUFF) do
		yield("/echo "..char)
		yield("/ays relog " ..char)
   	    yield("/echo 15 second wait")
	    yield("/wait 15")
		yield("/waitaddon NamePlate <maxwait.600><wait.10>")
		yield("/echo FC Seal Buff II")
		yield("/freecompanycmd <wait.1>")
		yield("/pcall FreeCompany false 0 4u <wait.1>")
		yield("/pcall FreeCompanyAction false 1 0u <wait.1>")
		yield("/pcall ContextMenu true 0 0 1u 0 0 <wait.1>")
		yield("/pcall SelectYesno true 0 <wait.1>")
	--Then you need to /pyes the "Execute"
	 end
	yield("/wait 3")
end
--gridania
if process_gridania == 1 then
	for _, char in ipairs(chars_gridania) do
	 yield("/echo "..char)
	 yield("/ays relog " ..char)
	 yield("/echo 15 second wait")
	 yield("/wait 15")
	 yield("/waitaddon NamePlate <maxwait.600> <wait.5>")
	 yield("/echo Processing Retainer Abuser "..rcuck_count.."/"..total_rcucks)
	 yield("/tp New Gridania <wait.8>")
	 open_aetheryte()
	 yield("/pcall TelepotTown false 11 1u")
	 yield("/pcall TelepotTown false 11 1u")
	 yield("/wait 5")
	 yield("/ac Sprint")
	 yield("/ac Sprint")
	 yield("/ac Sprint")
	 yield("/visland execonce GCgrid")
	 visland_stop_moving()
	 Final_GC_Cleaning()
	 rcuck_count = rcuck_count + 1
	end
end
--uldah
if process_uldah == 1 then
	for _, char in ipairs(chars_uldah) do
	 yield("/echo "..char)
	 yield("/ays relog " ..char)
	 yield("/echo 15 second wait")
	 yield("/wait 15")
	 yield("/waitaddon NamePlate <maxwait.600> <wait.5>")
	 yield("/echo Processing Retainer Abuser "..rcuck_count.."/"..total_rcucks)
	 rcuck_count = rcuck_count + 1
	 yield("/tp Ul'dah - Steps of Nald <wait.8>")
	 open_aetheryte()
	 yield("/pcall TelepotTown false 11 2u")
	 yield("/pcall TelepotTown false 11 2u")
	 yield("/wait 5")
	 yield("/ac Sprint")
	 yield("/ac Sprint")
	 yield("/ac Sprint")
	 yield("/visland execonce GCuld")
	 visland_stop_moving()
	 Final_GC_Cleaning()
	end
end
--limsa
if process_toilet == 1 then
	for _, char in ipairs(chars_toilet) do
	 yield("/echo "..char)
	 yield("/ays relog " ..char)
	 yield("/echo 15 second wait")
	 yield("/wait 15")
	 yield("/waitaddon NamePlate <maxwait.600> <wait.5>")
	 yield("/echo Processing Retainer Abuser "..rcuck_count.."/"..total_rcucks)
	 rcuck_count = rcuck_count + 1
	 yield("/tp Limsa Lominsa <wait.8>")
	 open_aetheryte()
	 yield("/pcall TelepotTown false 11 1u")
	 yield("/pcall TelepotTown false 11 1u")
	 yield("/wait 5")
	 yield("/ac Sprint")
	 yield("/ac Sprint")
	 yield("/ac Sprint")
	 yield("/visland execonce GClimsa")
	 visland_stop_moving()
	 Final_GC_Cleaning()
	end
end
--last one out turn off the lights
yield("/ays multi")
yield("/pcraft stop")