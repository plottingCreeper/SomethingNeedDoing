--[[
  Description: Diadem Re entry
  Author: Caeoltoiri
  Link: https://discord.com/channels/1162031769403543643/1162799234874093661/1192553288646787226
]]

routename = "noideaifspaceswork"

::Wait::
while not IsInZone(886) do
  yield("/wait 10")
end
yield("/wait 3")

::Enter::
yield("/target Aurvael")
yield("/pinteract")
yield("/wait 0.1")
if IsAddonVisible("Talk") then yield("/click talk") end
if IsAddonVisible("SelectString") then yield("/click select_string1") end
yield("/wait 0.1")
if IsAddonVisible("SelectYesno") then yield("/click select_yes") end
yield("/wait 1")
if IsAddonVisible("ContentsFinderConfirm") then yield("/click duty_commence") end
while GetCharacterCondition(35, false) do yield("/wait 1") end
while GetCharacterCondition(35) do yield("/wait 1") end
yield("/wait 3")

::Move::
yield("/mount \"Company Chocobo\"")
yield("/wait 3")
yield("/send SPACE")
yield("/visland movedir 0 20 0")
yield("/wait 1")
yield("/visland movedir 50 0 -50")
yield("/wait 1")
yield("/visland moveto -235 30 -435")
yield("/wait 1")
while IsMoving() do yield("/wait 1") end
yield("/visland exec "..routename)

goto Wait