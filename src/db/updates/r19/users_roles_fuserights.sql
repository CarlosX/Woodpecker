/*
MySQL Data Transfer
Source Host: localhost
Source Database: woodpecker
Target Host: localhost
Target Database: woodpecker
Date: 18-11-2008 15:51:44
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for users_roles_fuserights
-- ----------------------------
DROP TABLE IF EXISTS `users_roles_fuserights`;
CREATE TABLE `users_roles_fuserights` (
  `minrole` int(2) NOT NULL,
  `fuseright` varchar(100) collate latin1_general_ci NOT NULL,
  PRIMARY KEY  (`fuseright`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `users_roles_fuserights` VALUES ('0', 'default');
INSERT INTO `users_roles_fuserights` VALUES ('1', 'fuse_login');
INSERT INTO `users_roles_fuserights` VALUES ('1', 'fuse_buy_credits');
INSERT INTO `users_roles_fuserights` VALUES ('1', 'fuse_trade');
INSERT INTO `users_roles_fuserights` VALUES ('1', 'fuse_room_queue_default');
INSERT INTO `users_roles_fuserights` VALUES ('2', 'fuse_enter_full_rooms');
INSERT INTO `users_roles_fuserights` VALUES ('3', 'fuse_enter_locked_rooms');
INSERT INTO `users_roles_fuserights` VALUES ('3', 'fuse_kick');
INSERT INTO `users_roles_fuserights` VALUES ('3', 'fuse_mute');
INSERT INTO `users_roles_fuserights` VALUES ('4', 'fuse_ban');
INSERT INTO `users_roles_fuserights` VALUES ('4', 'fuse_room_mute');
INSERT INTO `users_roles_fuserights` VALUES ('4', 'fuse_room_kick');
INSERT INTO `users_roles_fuserights` VALUES ('4', 'fuse_receive_calls_for_help');
INSERT INTO `users_roles_fuserights` VALUES ('4', 'fuse_remove_stickies');
INSERT INTO `users_roles_fuserights` VALUES ('5', 'fuse_mod');
INSERT INTO `users_roles_fuserights` VALUES ('5', 'fuse_superban');
INSERT INTO `users_roles_fuserights` VALUES ('5', 'fuse_pick_up_any_furni');
INSERT INTO `users_roles_fuserights` VALUES ('5', 'fuse_ignore_room_owner');
INSERT INTO `users_roles_fuserights` VALUES ('5', 'fuse_any_room_controller');
INSERT INTO `users_roles_fuserights` VALUES ('2', 'fuse_room_alert');
INSERT INTO `users_roles_fuserights` VALUES ('5', 'fuse_moderator_access');
INSERT INTO `users_roles_fuserights` VALUES ('6', 'fuse_administrator_access');
INSERT INTO `users_roles_fuserights` VALUES ('6', 'fuse_see_flat_ids');
INSERT INTO `users_roles_fuserights` VALUES ('6', 'fuse_performance');
INSERT INTO `users_roles_fuserights` VALUES ('4', 'fuse_enter_all_rooms');
INSERT INTO `users_roles_fuserights` VALUES ('5', 'fuse_hotelalert');
INSERT INTO `users_roles_fuserights` VALUES ('3', 'fuse_alert');
INSERT INTO `users_roles_fuserights` VALUES ('6', 'fuse_debug');
INSERT INTO `users_roles_fuserights` VALUES ('5', 'fuse_funchatcommands');
INSERT INTO `users_roles_fuserights` VALUES ('4', 'fuse_see_all_roomowners');
