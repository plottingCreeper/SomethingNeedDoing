--[[
  Description: Send all retainers to do gc stuff, return home and turn multi back on
  Author: McVaxius
  Link: https://discord.com/channels/1162031769403543643/1162799234874093661/1164355152661184582
]]

--first turn on FC buffs
chars = {
 'asdfasdfs@Halicarnassus',
 "asdfasdfs'gil@Maduin",
 'asdfasdfs@Marilith',
 'asdfasdfs@Seraph'
}
FirstRun = 1
for _, char in ipairs(chars) do
 if FirstRun==0 then
	yield("/echo "..char)
	yield("/ays relog " ..char)
	yield("<wait.45.0>")
	--yield("/waitaddon NowLoading <maxwait.15>")
	yield("/waitaddon NamePlate <maxwait.600><wait.5>")
 end
FirstRun = 0
yield("/freecompanycmd <wait.1>")
yield("/pcall FreeCompany false 0 4u <wait.1>")
yield("/pcall FreeCompanyAction false 1 0u <wait.1>")
yield("/pcall ContextMenu true 0 0 1u 0 0 <wait.1>")
yield("/pcall SelectYesno true 0 <wait.1>")
--Then you need to /pyes the "Execute"
 end
yield("<wait.3.0>")

--next run everyone to GC desk
chars = {
 'asdfasdfs',
 "asdfasdfs'form@Halicarnassus",
 'Fuel Goblin@Halicarnassus'
}

--FirstRun = 1
for _, char in ipairs(chars) do
-- if FirstRun==0 then
	yield("/echo "..char)
	yield("/ays relog " ..char)
	yield("<wait.45.0>")
	--yield("/waitaddon NowLoading <maxwait.15>")
	yield("/waitaddon NamePlate <maxwait.600><wait.5>")
 --end
 --FirstRun = 0
 yield("/tp New Gridania <wait.8>")
 yield("/waitaddon NamePlate <wait.1>")
 yield("/target Aetheryte <wait.1>")
 yield("/lockon")
 yield("/automove")

 yield("/pinteract <wait.1>")
 yield("/pcall SelectString true 0")
 yield("/pcall TelepotTown false 11 1u")
 yield("/pcall TelepotTown false 11 1u")

 yield("<wait.5>")
 yield("/ac Sprint")
 yield("/ac Sprint")
 yield("/ac Sprint")
 yield("/visland execonce GCgrid")
 yield("/equipguud")
 yield("<wait.55>")
 --yield("/waitaddon SelectString <maxwait.120>")
 yield("/visland stop")
end

chars = {
 'asdfasdf@Halicarnassus',
 'ffffff@Maduin',
 'Pfffffm@Seraph'
}

for _, char in ipairs(chars) do
 yield("/echo "..char)
 yield("/ays relog " ..char)
 yield("<wait.65.0>")
 --yield("/waitaddon NowLoading <maxwait.15>")
 yield("/waitaddon NamePlate <maxwait.600><wait.5>")

 yield("/tp Ul'dah - Steps of Nald <wait.8>")
 yield("/waitaddon NamePlate <wait.1>")
 yield("/target Aetheryte <wait.1>")
 yield("/lockon")
 yield("/automove")

 yield("/pinteract <wait.1>")
 yield("/pcall SelectString true 0")
 yield("/pcall TelepotTown false 11 2u")
 yield("/pcall TelepotTown false 11 2u")

 yield("<wait.5>")
 yield("/ac Sprint")
 yield("/ac Sprint")
 yield("/ac Sprint")
 yield("/visland execonce GCuld")
 --simple tweaks
 --/tweaks
 --auto equip -> alias /equipguud
 yield("/equipguud")
 yield("<wait.80>")
--yield("/waitaddon SelectString <maxwait.120>")
 yield("/visland stop")
end


---time to do the turnins
------------------------------------------------
chars = {
 'asdfasdf@Halicarnassus',
 'asdfasdfasdf@Halicarnassus',
 "asdfasdf'form@Halicarnassus",
 'Fuel Goblin@Halicarnassus',
 'Gerolt Woodcutter@Halicarnassus'
}


-- Main loop
for _, char in ipairs(chars) do
--put in escape if we finished processing chars
        yield("/echo "..char)
        yield("/ays relog " .. char)
        yield("<wait.45.0>")
        --yield("/waitaddon NowLoading <maxwait.15>")
        yield("/waitaddon NamePlate <maxwait.600><wait.5.0>")

yield("/waitaddon NamePlate <maxwait.600>")
yield("/echo AutoED is starting...")
step = "Startup"

--turn around in case we aren't facing the correct way
--this attempts to target serpent or flame personnel or even storm.  assuming you have a separate line in a hotkey for each type.
--the purpose of this section is to get your char to face the npcs and orient the camera properly. otherwise the rest of the script might die

--no targeting needed with deliveroo
--yield("/send KEY_3")
--yield("<wait.1.0>")
--yield("/lockon on")
--yield("/automove on")
--yield("<wait.1.0>")

--deliveroo i choose you
yield("/deliveroo enable")
yield("<wait.1.0>")

--loop until deliveroo done
dellyroo = true
dellyroo = DeliverooIsTurnInRunning()
while dellyroo do
	yield("<wait.5.0>")
	dellyroo = DeliverooIsTurnInRunning()
end

yield("/tp Estate Hall")
yield("<wait.1>")
--yield("/waitaddon Nowloading <maxwait.15>")
yield("<wait.15>")
yield("/waitaddon NamePlate <maxwait.600><wait.5>")

--walk back to entrance properly
local islanders = {
	['asdfasdf'] = '/visland execonce FChalicarnassus',
	["Acasdfasdf'gil"] = '/visland execonce FCmaduin',
	['asdfasasdfdf'] = '/visland execonce FCmarilith'
}

if islanders[GetCharacterName()] then
  yield(islanders[GetCharacterName()])
end
yield("<wait.5>")
yield("/visland stop")

--Code for opening venture coffers
yield("<wait.3.0>")
yield("/echo Number of Venture Coffers to open: "..GetItemCount(32161))
VCnum = GetItemCount(32161)
while (VCnum > 0) do
	--this is no longer reliable
	--yield("/item Venture Coffer")
	yield("/send X")
	yield("<wait.6.0>")
	VCnum = GetItemCount(32161)
	yield("/echo Number of Venture Coffers left: "..GetItemCount(32161))
end
--yield("/autorun off")
--Code for opening FC menu so allagan tools can pull the FC points
yield("/freecompanycmd")
yield("<wait.3.0>")

end

--last one out turn off the lights
yield("/ays multi")
yield("/pcraft stop")