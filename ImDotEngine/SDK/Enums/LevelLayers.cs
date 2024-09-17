enum LevelLayers : uint
{
    Background, // for background objects (like visual mountains ect)
    BackBlocks, // blocks or objects behind physical chunks like factories & containers
    ForeBlocks, // physical blocks like dirt
    Entities, // entities like enemies npcs or tile entities (stuff like physical containers)
    Players, // actual players of the game 
    UI, // UI like menus debug text ect
    Count, // this is here for debugging reasons
}