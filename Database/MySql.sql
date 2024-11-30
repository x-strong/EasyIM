
SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for ChatMessageRecord
-- ----------------------------
DROP TABLE IF EXISTS `ChatMessageRecord`;
CREATE TABLE `ChatMessageRecord` (
  `AutoID` bigint(20) NOT NULL AUTO_INCREMENT,
  `SpeakerID` varchar(20) NOT NULL,
  `AudienceID` varchar(20) NOT NULL,
  `IsGroupChat` bit NOT NULL,
  `Content` longblob NOT NULL,
  `OccureTime` datetime NOT NULL,
  PRIMARY KEY (`AutoID`),
  KEY `IX_ChatMessageRecord` (`SpeakerID`,`AudienceID`,`OccureTime`) USING BTREE,
  KEY `IX_ChatMessageRecord_1` (`AudienceID`,`OccureTime`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ChatMessageRecord
-- ----------------------------

-- ----------------------------
-- Table structure for IMConfiguration
-- ----------------------------
DROP TABLE IF EXISTS `IMConfiguration`;
CREATE TABLE `IMConfiguration` (
  `Key` varchar(20) NOT NULL,
  `Value` varchar(1000) NOT NULL,
  PRIMARY KEY (`Key`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of IMConfiguration
-- ----------------------------
INSERT INTO `IMConfiguration` VALUES ('IMVersion', '1');

-- ----------------------------
-- Table structure for IMGroup
-- ----------------------------
DROP TABLE IF EXISTS `IMGroup`;
CREATE TABLE `IMGroup` (
  `GroupID` varchar(20) NOT NULL,
  `Name` varchar(20) NOT NULL,
  `CreatorID` varchar(20) NOT NULL,
  `Announce` varchar(200) NOT NULL,
  `Members` varchar(4000) NOT NULL,
  `IsPrivate` tinyint NOT NULL,
  `CreateTime` datetime NOT NULL,
  `Version` int(255) NOT NULL,
  PRIMARY KEY (`GroupID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of IMGroup
-- ----------------------------

-- ----------------------------
-- Table structure for IMUser
-- ----------------------------
DROP TABLE IF EXISTS `IMUser`;
CREATE TABLE `IMUser` (
  `UserID` varchar(50) NOT NULL,
  `PasswordMD5` varchar(100) NOT NULL,
  `Phone` varchar(20) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Friends` varchar(4000) NOT NULL,
  `CommentNames` varchar(4000) NOT NULL,
  `OrgID` varchar(20) NOT NULL,
  `Signature` varchar(100) NOT NULL,
  `HeadImageIndex` int(255) NOT NULL,
  `HeadImageData` mediumblob,
  `Groups` varchar(1000) NOT NULL,
 `UserState` int(255) NOT NULL,
 `MobileOfflineTime` datetime NOT NULL,
 `PcOfflineTime` datetime NOT NULL,
  `CreateTime` datetime NOT NULL,
  `Version` int(255) NOT NULL,
  PRIMARY KEY (`UserID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of IMUser
-- ----------------------------
DROP TABLE IF EXISTS `OfflineMessage`;
CREATE TABLE `OfflineMessage`(
  	`AutoID` int(20) NOT NULL AUTO_INCREMENT,
	`SourceUserID` varchar(50) NOT NULL,
	`DestUserID` varchar(50) NOT NULL,
	`SourceType` int(255) NOT NULL,
	`GroupID` varchar(50) NOT NULL,
	`InformationType` int(255) NOT NULL,
	`Information` longblob ,
	`Tag` varchar(100) NOT NULL,
	`TimeTransfer` datetime NOT NULL,  
	PRIMARY KEY (`AutoID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `OfflineFileItem`;
CREATE TABLE `OfflineFileItem`(
  	`AutoID` int(20) NOT NULL AUTO_INCREMENT,
	`FileName` nvarchar(100) NOT NULL,
	`FileLength` int(255)  NOT NULL,
	`SenderID` varchar(50) NOT NULL,
	`SenderType` int(255) NOT NULL,
	`AccepterType` int(255) NOT NULL,
	`AccepterID` varchar(50) NOT NULL,
	`RelayFilePath` varchar(300) NOT NULL,
	PRIMARY KEY (`AutoID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `GroupBan`;
CREATE TABLE `GroupBan`(
  	`AutoID` int(20) NOT NULL AUTO_INCREMENT,
	`GroupID` varchar(20) NOT NULL,
	`OperatorID` varchar(20) NOT NULL,
	`UserID` varchar(20) NOT NULL,
	`Comment2` varchar(50) NOT NULL,
	`EnableTime` datetime NOT NULL,
	`CreateTime` datetime NOT NULL,
	PRIMARY KEY (`AutoID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `AddGroupRequest`;
CREATE TABLE `AddGroupRequest`(
  	`AutoID` int(20) NOT NULL AUTO_INCREMENT,
	`RequesterID` varchar(20) NOT NULL,
	`GroupID` varchar(20) NOT NULL,
	`AccepterID` varchar(20) NOT NULL,
	`Comment2` varchar(500) NOT NULL,
	`State` int NOT NULL,
	`Notified` bit NOT NULL,
	`CreateTime` datetime NOT NULL,
	PRIMARY KEY (`AutoID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `AddFriendRequest`;
CREATE TABLE  `AddFriendRequest`(
	`AutoID` int(20) NOT NULL AUTO_INCREMENT,
	`RequesterID` varchar(50) NOT NULL,
	`AccepterID` varchar(50) NOT NULL,
	`RequesterCatalogName` varchar(20) NOT NULL,
	`AccepterCatalogName` varchar(20) NOT NULL,
	`Comment2` varchar(500) NOT NULL,
	`State` int NOT NULL,
	`Notified` bit NOT NULL,
	`CreateTime` datetime NOT NULL,
	PRIMARY KEY (`AutoID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
