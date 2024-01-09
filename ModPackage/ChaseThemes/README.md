# Description
Plays various chase themes when monsters are chasing you, randomised every round (25 themes included). Unique songs for some monsters to fit their personalities better. You can also add your own songs (supporting .ogg, .mp3 or .wav filetypes). Not all monsters have themes as we felt that for some it would go against the design of the monster. 

## Important Notes:
* All clients will need to have the same songs with the same names, otherwise you may get different random chase themes per stage.
* The game takes longer to open than normal due to loading large amounts of audio for the themes.
* Individual themes can be disabled in the config by setting the boolean value to 'false'.

### Monsters Supported
- Thumper
- Hoarding bug
- Bunker Spider (slightly buggy)
- Forest Giant (unique songs)
- Ghost Girl (unique songs)
- Hydrogere (unique songs)
- Nutcracker (unique songs)

### Songs Included
- Bee Gees - Stayin Alive
- Boney M - Rasputin
- Gourmet Race 64 - Super Smash Bros. Ultimate
- Initial D - Deja Vu
- Manuel - Gas Gas Gas
- USSR National Anthem
- Pirates Of The Caribbean Theme Song
- 'Enhanced' Thomas the Tank Engine Theme
- Wii Sports Theme
- Aqua - Barbie Girl
- Dance The Night (Barbie The Album)
- Ghostbusters
- Pink (Barbie The Album)
- I'm Just Ken (Barbie The Album)
- Benny Hill Theme
- Jaws Theme Song
- Undertale - Megalovania
- Sweet Home Alabama
- All I Want For Christmas Is You
- Star Wars - Imperial March
- America!
- How Bad Can I Be
- Let It Grow
- Thneedville

### How to add your own songs:
1. Download song.
2. Convert to compatible file format (.ogg, .mp3 or .wav).
3. You may want to normalize audio, otherwise may be too quiet/loud. There are free tools capable of this such as Reaper. All audio in the mod has been mixed to -14 LUFS-I using Reaper's 'Render' tool.
4. Open r2modman, Lethal Company and then go into settings.
5. Click on 'Browse profile folder'.
6. Find the folder BepInEx > plugins > LineLoad-ChaseThemes.
7. Put your song in here and it will automatically be detected when the game is launched.
8. Finally, give it a tag to match the monster you want to play on. Some monsters have unique tags, otherwise use the MAIN tag.
9. Songs are auto-added to the config when the game is launched so that you can disable them individually.

### Removing songs:
If you want to remove themes from the mod folder instead of simply disabling them with the config, then we suggest you also delete the config file so that it updates the list of themes appropriately. This can be done in r2modman by selecting Config editor > BepInEx\config\LineLoad.ChaseThemes.cfg > Delete.
