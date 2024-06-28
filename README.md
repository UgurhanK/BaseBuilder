# CS2 BaseBuilder Plugin
A CS2 Plugin Made for Tesla[TR] Server.
## Requirements
- [MetaMod:Source](https://github.com/alliedmodders/metamod-source/)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
## Features
These features are what is currently working or planeed
- [x] Block Interaction
  - [x] Player Can Move Props With Using [E]
  - [x] Player Can Rotate Props With Using [R]
## Install
1. Install Metamod:Source and Counter Strike Sharp.
2. Copy `HideAndSeekPlugin` to `csgo/addons/counterstrikesharp/plugins/`.
3. After the first run, update the configuration file `HideAndSeekPlugin.json` as detailed below.
## Configuration
After first load, a configuration file will be created in 
`csgo/addons/counterstrikesharp/configs/plugins/HideAndSeekPlugin/HideAndSeekPlugin.json`.
### Game Settings

| Setting | Default | Description |
| --- | --- | --- |
| hns_prefix | "[HNS]" | Sets The prefix that the plugin uses for messages in chat. |
| hns_zombie_style | true | Sets the gmae to start with one seeker and everyone they kill joins them. (Currently Not used) |
| hns_min_players | 2 | Sets the Minimum players required to start. | 
| hns_t_instakills | true | Sets if the Seeker only needs to hit the ct's once to kill them (Currently Not used) |
| Hns _tr_t_speed | 1 | Sets the Multiplier for The Seekers Speed (Currently Not used) |
| hns_starting_ts | 1 | Sets the amount of seekers to start with, Zombie mode forces this to 1 (Currently Not used) |
