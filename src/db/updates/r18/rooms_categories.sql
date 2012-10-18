/*
MySQL Data Transfer
Source Host: localhost
Source Database: woodpecker
Target Host: localhost
Target Database: woodpecker
Date: 17-11-2008 22:00:48
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for rooms_categories
-- ----------------------------
DROP TABLE IF EXISTS `rooms_categories`;
CREATE TABLE `rooms_categories` (
  `id` int(4) NOT NULL,
  `orderid` int(11) NOT NULL,
  `parentid` int(4) NOT NULL,
  `isnode` enum('1','0') collate latin1_general_ci NOT NULL default '0',
  `name` varchar(100) collate latin1_general_ci NOT NULL,
  `publicspaces` enum('1','0') collate latin1_general_ci NOT NULL default '0',
  `allowtrading` enum('1','0') collate latin1_general_ci NOT NULL default '0',
  `minrole_access` int(1) NOT NULL default '1',
  `minrole_setflatcat` int(1) NOT NULL default '1',
  PRIMARY KEY  (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `rooms_categories` VALUES ('0', '0', '0', '0', 'No category', '0', '0', '1', '1');
INSERT INTO `rooms_categories` VALUES ('3', '0', '0', '1', 'Public Rooms', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('4', '0', '0', '1', 'Guest Rooms', '0', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('6', '0', '3', '0', 'Restaurants and cafes', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('8', '0', '3', '0', 'Club-only spaces', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('10', '0', '3', '0', 'Swimming Pools', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('9', '0', '3', '0', 'Parks and Gardens', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('11', '0', '3', '0', 'The Lobbies', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('12', '0', '3', '0', 'The Hallways', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('13', '0', '3', '0', 'Games', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('14', '0', '13', '0', 'BattleBall', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('15', '0', '13', '0', 'SnowStorm', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('5', '0', '3', '0', 'Entertainment', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('16', '0', '7', '0', 'Hotel Rooftop', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('17', '0', '7', '0', 'Oldskool Disco', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('18', '0', '7', '0', 'Aqua Disco', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('19', '0', '7', '0', 'Woodpecker Disco', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('7', '0', '3', '0', 'Lounges and Clubs', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('21', '0', '9', '0', 'The Green Heart', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('20', '0', '9', '0', 'The Laughing Lions Park', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('22', '0', '10', '0', 'The Lido', '1', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('101', '0', '4', '0', 'Staff HQ', '0', '1', '4', '5');
INSERT INTO `rooms_categories` VALUES ('102', '0', '4', '1', '<EVENT ROOT FLOOR>', '0', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('103', '0', '102', '0', '<STAFF MADE EVENT ROOMS>', '0', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('104', '0', '102', '0', '<EVENT USER FLATS>', '0', '0', '1', '1');
INSERT INTO `rooms_categories` VALUES ('105', '0', '102', '0', '<EVENT USER FLATS2>', '0', '0', '1', '1');
INSERT INTO `rooms_categories` VALUES ('107', '0', '4', '1', 'Staff floor', '0', '1', '1', '6');
INSERT INTO `rooms_categories` VALUES ('108', '0', '107', '0', 'Rooms of the week', '0', '1', '1', '6');
INSERT INTO `rooms_categories` VALUES ('109', '0', '107', '0', 'Staff-made rooms', '0', '1', '1', '4');
INSERT INTO `rooms_categories` VALUES ('111', '0', '107', '0', 'Items for display', '0', '0', '1', '6');
INSERT INTO `rooms_categories` VALUES ('112', '0', '4', '0', 'Restaurants, cafes and discos', '0', '0', '1', '1');
INSERT INTO `rooms_categories` VALUES ('113', '0', '4', '0', 'Trade floor', '0', '1', '1', '1');
INSERT INTO `rooms_categories` VALUES ('114', '0', '4', '0', 'Chill, chat and discussion', '0', '0', '1', '1');
INSERT INTO `rooms_categories` VALUES ('115', '0', '4', '0', 'Mafias, organizations and companies', '0', '1', '1', '1');
INSERT INTO `rooms_categories` VALUES ('116', '0', '4', '0', 'Maze floor', '0', '0', '1', '1');
INSERT INTO `rooms_categories` VALUES ('117', '0', '4', '0', 'Games and races', '0', '1', '1', '1');
INSERT INTO `rooms_categories` VALUES ('118', '0', '4', '0', 'Helpdesk floor (users help users)', '0', '0', '1', '1');
INSERT INTO `rooms_categories` VALUES ('120', '0', '4', '0', 'Miscallenous rooms floor', '0', '0', '1', '1');
