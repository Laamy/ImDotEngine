enum PacketType : byte
{
    None = 0,
    HANDSHAKE, // SERVER, contains info the client needs like the world seed & other settings..
    PLAYER_UPDATE, // CLIENT/SERVER, clients send this to the server to update it on their movements position velocity and other bits of player info
    PLAYER_ADD, // SERVER, sent to clients & contains info like the players UUID & position
    PLAYER_REMOVE, // SERVER, sent to the clients to remove a player from the scene
}