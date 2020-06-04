# PersonalGeoDatabase2Shapefile
Convert ESRI Personal Geodatabase to Shapefile,Support point, multipoint, line, and polygon types.
Personal GeoDatabase is a Microsoft Access database with a set of tables defined by ESRI for holding geodatabase metadata, and with geometry for features held in a BLOB column in a custom format (essentially Shapefile geometry fragments). 
Read Personal Geodatabase by using Microsoft Access Engine;
Convert geometry Field by c#
Write Shapefile by using Dotspatial2.0(https://github.com/DotSpatial/DotSpatial/wiki/Switching-from-DotSpatial-1.9-to-2.0)


--------------------------------------------------------------------
本程序实现ESRI 个人地理数据库向Shapefile转换，支持点、多点、线和面数据类型；个人地理数据库实质上Access数据库（扩展名为mdb），在windows上访问mdb文件，需要确保安装了Access Engine插件；
个人地理数据库的几何字段是二进制存储，格式与Shapefile的几何字段一致。
本程序使用Dotspatial写Shapefile文件。
