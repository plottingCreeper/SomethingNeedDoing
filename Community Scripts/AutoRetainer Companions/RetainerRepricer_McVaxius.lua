--[[
  Description: reprice all chars.
    requires marketbuddyu. turn off price history on click in settings
    here it is. configure your chars.
    it requires you to be outside of a fc house with a bell on the inside somewhere
    first char you load up manually and rest will be cycled. ays multi turned on at end
  Author: McVaxius
  Link: https://discord.com/channels/1162031769403543643/1162799234874093661/1165657736609021952
]]

chars = {
 'charname@server',
 'charname@server',
 'charname@server',
 'charname@server',
 'charname@server',
 'charname@server',
 'charname@server',
 'charname@server'
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
    yield("/target Summoning Bell <wait.0.1>")
    if IsAddonVisible("_TargetCursor")==false then
        yield("/ays het")
        yield("<wait.15.0>")
        --yield("/waitaddon NowLoading <maxwait.15>")
        yield("/waitaddon NamePlate <maxwait.15><wait.5>")
        yield("/target Summoning Bell <wait.1>")
        yield("/lockon on")
        yield("/automove on <wait.2>")
    end
    yield("/send NUMPAD0")

    for retainers = 1, 10 do
        yield("/waitaddon RetainerList")
        yield("/click select_retainer"..retainers.." <wait.1>")
        if IsAddonVisible("SelectString")==false then yield("/click talk <wait.1>") end
        if IsAddonVisible("SelectString")==false then yield("/click talk <wait.1>") end
        yield("/waitaddon SelectString")
        yield("/click select_string3")

        for list = 0, 19 do
            yield("/waitaddon RetainerSellList")
            yield("/pcall RetainerSellList true 0 "..list.." 1 <wait.0.1>")
            if IsAddonVisible("ContextMenu") then yield("/pcall ContextMenu true 0 0 <wait.2.5>") else break end
        yield("/send NUMPAD0")
        yield("/send NUMPAD0")
        end

        yield("/pcall RetainerSellList true -2")
        yield("/pcall RetainerSellList true -1")
        yield("/waitaddon SelectString")
        yield("/pcall SelectString true -1 <wait.1>")
        yield("/click talk")
    end

    yield("/pcall RetainerList false -2")
    yield("/pcall RetainerList true -1 <wait.2>")
    yield("/pcall RetainerList false -2")
    yield("/pcall RetainerList true -1 <wait.2>")
end
yield ("/ays multi")