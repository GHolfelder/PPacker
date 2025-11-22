<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.10" tiledversion="1.10.2" name="grass_tileset" tilewidth="32" tileheight="32" tilecount="9" columns="3">
 <properties>
  <property name="description" value="A simple grass tileset for testing"/>
 </properties>
 <image source="grass_tiles.png" width="96" height="96"/>
 <tile id="0">
  <properties>
   <property name="type" value="grass"/>
   <property name="walkable" type="bool" value="true"/>
  </properties>
 </tile>
 <tile id="1">
  <properties>
   <property name="type" value="grass_flower"/>
   <property name="walkable" type="bool" value="true"/>
  </properties>
 </tile>
 <tile id="4">
  <properties>
   <property name="type" value="stone"/>
   <property name="walkable" type="bool" value="false"/>
  </properties>
 </tile>
 <tile id="8">
  <animation>
   <frame tileid="8" duration="500"/>
   <frame tileid="7" duration="500"/>
   <frame tileid="6" duration="500"/>
  </animation>
  <properties>
   <property name="type" value="water"/>
   <property name="walkable" type="bool" value="false"/>
  </properties>
 </tile>
</tileset>