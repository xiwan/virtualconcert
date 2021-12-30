# virtualconcert

## description

This is a live doc, may be edited at any moment in the future. 

a Unity based project (Metaverse) to show the cabablity of making high-density population scene. My vision is : this scene is about a virtual concert, where people could dance and text with each other. Moreover, there is a messenger/hoster (idol) arranging the whole show.

Rendering layer used some drop-to-used assets from polyperfect.

Networking layer leveraged Mirror, for more detail, plz refer its official site.

### role definition

crowd: normal users connected from different terminals.

messenger: the hoster supporting the show including observation (admin).

star: the idol leading the crowd to interaction. default is priest and bgm is thriller :)

## How to use it?

### Game manager

![game manager](https://github.com/xiwan/virtualconcert/blob/main/Static/image03.jpg?raw=true "game manager")

 - Reset Camera: Camera enters free mode, look at center stage and rotate around it.
 - Pick Any Player: Camera enters following mode, pick a random player and controlled by WSAD.
 - Spawn Radius: the area to generate npcs
 - Spawn Amount: how many npcs for each generating action (right click Game Manager componnet, click "spawn animals");

### Add new character & animation

TBD

### Networking

For purpose of utilizing Mirror, a special prefab called "NetworkAttach" was created.
It was pretty easy, attaching it to the npc gameobject in the runtime would do the networking trick.

![network attach](https://github.com/xiwan/virtualconcert/blob/main/Static/image05.jpg?raw=true  "network attach")

## screenshots

### free mode
![free camera](https://github.com/xiwan/virtualconcert/blob/main/Static/image04.jpg?raw=true  "free camera")


### following mode
![follow camera](https://github.com/xiwan/virtualconcert/blob/main/Static/image02.jpg?raw=true "follow camera")


## dependency

[polyperfect](https://assetstore.unity.com/packages/3d/characters/humanoids/low-poly-animated-people-156748)

[mixamo](https://www.mixamo.com/)




