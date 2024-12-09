# CrimsonClans
`Server side only` extension mod for the Clan system.

Features:
- Limit Clans to X number of Castle Hearts
- Limit ability to do clan operations (join, create, leave, kick, edit, invite, etc) during raid window
- Limit ability to do clan operations prior and after the raid window (raid buffers)
- Add a cooldown to joining clans after leaving one

## Installation
* Install [BepInEx](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/)
* Extract _CrimsonClans.dll_ into _(VRising server folder)/BepInEx/plugins_
* Start server to generate the config file, modify it, and restart server

## Config

```
## The amount of castle hearts a clan can have.
# Setting type: Int32
# Default value: 1
HeartsPerClan = 1
```

```
## The number of minutes prior to the raid window to lock clan operation.
# Setting type: Int32
# Default value: 30
PreRaidBufferMins = 30

## The number of minutes past the raid window to lock clan operation.
# Setting type: Int32
# Default value: 0
PostRaidBufferMins = 0
```

```
## If this is set to true, clans will be unable to invite players to their clan during raid time.
# Setting type: Boolean
# Default value: true
Invite = true

## If this is set to true, clans will be unable to be created during raid time.
# Setting type: Boolean
# Default value: true
Create = true

## If this is set to true, clans will not be able to change their details during raid time.
# Setting type: Boolean
# Default value: true
Edit = true

## If this is set to true, clans will be unable to kick players from the clan during raid time.
# Setting type: Boolean
# Default value: false
Kick = false

## If this is set to true, players will be unable to leave clans during raid time.
# Setting type: Boolean
# Default value: false
Leave = false
```

```
## The number of minutes that a player must wait to join a clan after leaving a prior clan.
# Setting type: Int32
# Default value: 0
JoinCooldown = 30
```

## Support

Want to support my V Rising Mod development? 

Donations Accepted

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/skytech6)

Or buy/play my games! 

[Train Your Minibot](https://store.steampowered.com/app/713740/Train_Your_Minibot/) 

[Boring Movies](https://store.steampowered.com/app/1792500/Boring_Movies/) **FREE TO PLAY**

**This mod was a paid creation. If you are looking to hire someone to make a mod for any Unity game reach out to me on Discord! (skytech6)**
