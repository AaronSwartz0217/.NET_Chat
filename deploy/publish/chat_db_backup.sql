mysqldump : mysqldump: [Warning] Using a password on the command line interface
 can be insecure.
所在位置 行:1 字符: 432
+ ... ublish" -Force; mysqldump -u root -pZ2971762643z chat_db 2>&1 | Out-F ...
+                     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : NotSpecified: (mysqldump: [War...an be insecure. 
   :String) [], RemoteException
    + FullyQualifiedErrorId : NativeCommandError
 
-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: chat_db
-- ------------------------------------------------------
-- Server version	8.0.42

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `comments`
--

DROP TABLE IF EXISTS `comments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comments` (
  `id` int NOT NULL AUTO_INCREMENT,
  `post_id` int NOT NULL,
  `user_id` int NOT NULL,
  `content` varchar(2000) COLLATE utf8mb4_general_ci NOT NULL,
  `parent_id` int NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  `created_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comments`
--

LOCK TABLES `comments` WRITE;
/*!40000 ALTER TABLE `comments` DISABLE KEYS */;
/*!40000 ALTER TABLE `comments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `type` int NOT NULL,
  `title` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `content` varchar(500) COLLATE utf8mb4_general_ci NOT NULL,
  `related_id` int NOT NULL,
  `read` tinyint(1) NOT NULL,
  `created_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `post_favorites`
--

DROP TABLE IF EXISTS `post_favorites`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `post_favorites` (
  `user_id` int NOT NULL,
  `post_id` int NOT NULL,
  `created_time` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `post_favorites`
--

LOCK TABLES `post_favorites` WRITE;
/*!40000 ALTER TABLE `post_favorites` DISABLE KEYS */;
INSERT INTO `post_favorites` VALUES (3,7,'2026-06-19 02:19:46'),(3,2,'2026-06-19 02:23:10'),(3,4,'2026-06-19 03:04:35');
/*!40000 ALTER TABLE `post_favorites` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `post_likes`
--

DROP TABLE IF EXISTS `post_likes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `post_likes` (
  `user_id` int NOT NULL,
  `post_id` int NOT NULL,
  `created_time` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `post_likes`
--

LOCK TABLES `post_likes` WRITE;
/*!40000 ALTER TABLE `post_likes` DISABLE KEYS */;
INSERT INTO `post_likes` VALUES (3,5,'2026-06-18 08:21:05'),(3,7,'2026-06-19 02:19:45'),(3,4,'2026-06-19 03:04:34');
/*!40000 ALTER TABLE `post_likes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `posts`
--

DROP TABLE IF EXISTS `posts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `posts` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `board_id` int DEFAULT NULL,
  `title` varchar(255) COLLATE utf8mb4_general_ci NOT NULL,
  `content` text COLLATE utf8mb4_general_ci NOT NULL,
  `type` int NOT NULL,
  `status` int NOT NULL,
  `tags` varchar(200) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `images` varchar(1000) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `view_count` int NOT NULL,
  `like_count` int NOT NULL,
  `comment_count` int NOT NULL,
  `share_count` int NOT NULL,
  `last_comment_at` datetime DEFAULT NULL,
  `ip` varchar(45) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  `created_time` datetime NOT NULL,
  `updated_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `posts`
--

LOCK TABLES `posts` WRITE;
/*!40000 ALTER TABLE `posts` DISABLE KEYS */;
INSERT INTO `posts` VALUES (1,1,NULL,'1','1',0,0,NULL,NULL,4,0,0,0,NULL,NULL,1,'2026-06-11 11:06:41','2026-06-11 11:06:41'),(2,1,NULL,'test','test',0,0,NULL,NULL,14,0,0,0,NULL,NULL,0,'2026-06-11 11:08:34','2026-06-11 11:08:34'),(3,1,NULL,'1','1',0,0,NULL,NULL,1,0,0,0,NULL,NULL,1,'2026-06-11 11:09:54','2026-06-11 11:09:54'),(4,2,NULL,'GOOD','YES',0,0,NULL,NULL,14,1,0,0,NULL,NULL,0,'2026-06-11 11:34:52','2026-06-11 11:34:52'),(5,3,NULL,'1','2',0,0,NULL,NULL,2,1,0,0,NULL,NULL,0,'2026-06-17 14:32:07','2026-06-17 14:32:07'),(6,3,NULL,'618','618',0,0,NULL,NULL,1,0,0,0,NULL,NULL,0,'2026-06-18 06:46:05','2026-06-18 06:46:05'),(7,3,NULL,'涓嶆樉绀?,'涓嶆樉绀?,0,0,NULL,NULL,3,1,0,0,NULL,NULL,0,'2026-06-18 07:46:36','2026-06-18 07:46:36'),(8,3,NULL,'2','2',0,0,NULL,NULL,1,0,0,0,NULL,NULL,0,'2026-06-21 09:11:13','2026-06-21 09:11:13');
/*!40000 ALTER TABLE `posts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `refresh_tokens`
--

DROP TABLE IF EXISTS `refresh_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `refresh_tokens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `token` varchar(500) COLLATE utf8mb4_general_ci NOT NULL,
  `expires_at` datetime NOT NULL,
  `revoked` tinyint(1) NOT NULL,
  `created_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=117 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `refresh_tokens`
--

LOCK TABLES `refresh_tokens` WRITE;
/*!40000 ALTER TABLE `refresh_tokens` DISABLE KEYS */;
INSERT INTO `refresh_tokens` VALUES (1,1,'L6sF1FF3n36BpnJhjIyAzcKiIW/khHmJ0AhnvhRclck=','2026-06-18 04:34:53',0,'2026-06-11 04:34:53'),(2,1,'eGoPDFDZJtQngWdh1Ps2xvtYywzmH7o2omukOzA76gU=','2026-06-18 05:42:56',0,'2026-06-11 05:42:56'),(3,1,'EJBzC0+zuEQ+d5HOC+tC46f3m8UHMTIj28Eq3xmFw9k=','2026-06-18 05:48:05',0,'2026-06-11 05:48:05'),(4,1,'rzjkYi/4JH5AcDb5CXZM9DLoYvwgSke8gsg1Y3SCbzw=','2026-06-18 06:10:09',0,'2026-06-11 06:10:09'),(5,1,'Qs/3Sbd1gEECYGnUWifeKnJINRmCTS0jd37/r/OFLKw=','2026-06-18 06:10:43',0,'2026-06-11 06:10:43'),(6,1,'lt+Q8N6yvbUwVUz0BzdvSLw+c2h+PDus/RWEDcIeLAo=','2026-06-18 06:29:49',0,'2026-06-11 06:29:49'),(7,1,'G/2WKMt3HAsuq7JMl424f8NPNebMAINAkpaBx3EEWNM=','2026-06-18 06:36:10',0,'2026-06-11 06:36:10'),(8,1,'5mJy8BxEVghkIBRNeai/VPw3FulCegYedhum00NkAFA=','2026-06-18 06:42:08',0,'2026-06-11 06:42:08'),(9,1,'s+iR8nuoz5wNPUA+xOwvfVnbODxJ+cB9H/qQS5Gzpeg=','2026-06-18 06:47:48',0,'2026-06-11 06:47:48'),(10,1,'B4NjBQPkxKBLZ5xoFYJ1KXMR3EV9mMuNwCGL1J1ZZEA=','2026-06-18 06:48:04',0,'2026-06-11 06:48:04'),(11,1,'ybdXBWtb1A984c3UN1KvFQqb/iQuEkcF7SLERYEG8jQ=','2026-06-18 06:49:47',0,'2026-06-11 06:49:47'),(12,1,'j5cjfrobjE5S+ni36uhhyC1PNw13Q1Fb3Pw5jrxf1Ic=','2026-06-18 06:53:46',0,'2026-06-11 06:53:46'),(13,1,'OrStbIYqN9zt6m51jZH3CmPXZzX58mZnZe3O3+U3ROE=','2026-06-18 06:54:45',0,'2026-06-11 06:54:45'),(14,1,'IE1mZImvA/oBypQjV9uks9jdJEwG+nRLk3aIW9HWnks=','2026-06-18 06:58:42',0,'2026-06-11 06:58:42'),(15,1,'PbVRohCQcAxVh2GEfJ/VyXlLVZvh957yjt7I0dER8ZU=','2026-06-18 07:01:56',0,'2026-06-11 07:01:56'),(16,1,'YPq975b+Vv4ClPnvLt15UBrXjcWBFyqdko2NLb6NxHY=','2026-06-18 07:02:18',0,'2026-06-11 07:02:18'),(17,1,'94Z4xkUe9V1bvdkK9zgTv0asIbU6ZLRWBzfr8N54+80=','2026-06-18 07:17:16',0,'2026-06-11 07:17:16'),(18,1,'YanYSC6/Bx8yQdG4vl3VIT31NTnrYbhFiz7uQXcZ56Y=','2026-06-18 07:25:26',0,'2026-06-11 07:25:26'),(19,1,'4rc4O7G0cmMKGCJJoaciabKwct/H8lzEkG3InuUO5Og=','2026-06-18 07:26:21',0,'2026-06-11 07:26:21'),(20,1,'8LVdJyz9WITx5SvXcE/FNhcXaiRBw6FRhAO3s6iSRJI=','2026-06-18 07:33:41',0,'2026-06-11 07:33:41'),(21,1,'p19k6qCJwL8Xw4Nn085wKB0M0Vf65wWjliEmsPfd2YM=','2026-06-18 11:03:22',0,'2026-06-11 11:03:22'),(22,2,'GLwbIYBpMfYajV4j5OgcF5JrChDwj225FsDQMATLJNY=','2026-06-18 11:33:27',0,'2026-06-11 11:33:27'),(23,3,'+He8cOavhno/RHzvehdl5HzqcdMEMJ8n8bgJfaigBgs=','2026-06-18 12:00:52',0,'2026-06-11 12:00:52'),(24,3,'CAuMfVi86XafjRmR4TOjwIF5Rm/6wNLvhFVZl/7EZmM=','2026-06-20 07:18:24',0,'2026-06-13 07:18:24'),(25,1,'MRGUlAvOvxCMb6fqFr2iXlZcFk8t6LMb2ei5uKepCeo=','2026-06-20 07:33:57',0,'2026-06-13 07:33:57'),(26,3,'Mg4F6r4S1NcLYiPYr6XE/ETLrcvMTrydyhmWO213r1s=','2026-06-20 08:10:31',1,'2026-06-13 08:10:31'),(27,1,'onEuO0ktJKvrRZzjh6Vz4lyobNKR6T2FLMiy7KCGeSE=','2026-06-20 08:33:59',1,'2026-06-13 08:33:59'),(28,3,'b+3UmVAVsI0Bh62aacDc7onHUx2/I+76DHqCuEb0J30=','2026-06-22 15:57:39',0,'2026-06-15 15:57:39'),(29,3,'qoBoIZjKaS+xatRkGKuqbViZJDCVyJdKLqMwfgGIxuo=','2026-06-22 16:00:13',0,'2026-06-15 16:00:13'),(30,3,'kHm0TXu2v8NdtG3qkoMlOGG7rLhZS44AcjsPtbUaTeI=','2026-06-22 16:00:15',0,'2026-06-15 16:00:15'),(31,3,'yMHUZE0WZ445+LZv+uVN9qtd6YVT+ygHsCUMD/dzlqs=','2026-06-22 16:00:17',0,'2026-06-15 16:00:17'),(32,3,'0RSKApZQjCVqpHsavvJrqUbNWXuSl2OA75BzkVPH2tQ=','2026-06-22 16:01:45',0,'2026-06-15 16:01:45'),(33,3,'0phUxdZpDUuh7O/Nd4qy9HhTfP4N7lTpD3k5mTlr1mw=','2026-06-22 16:02:04',0,'2026-06-15 16:02:04'),(34,3,'X+umHmqmkqtLK3RHPuf2IxjsnI/8RtalOOsuWbqcX2Y=','2026-06-22 16:03:27',0,'2026-06-15 16:03:27'),(35,3,'MVbD7QF3geg6o8KrjzUUmdwUO+MG7Y9nOeeKYyrL5LU=','2026-06-22 16:10:53',0,'2026-06-15 16:10:53'),(36,3,'gqD6L5MRB2CPbInN3S2u+QMCcIJx8K2PX7zre8l+fG4=','2026-06-22 16:13:11',0,'2026-06-15 16:13:11'),(37,3,'fn7SKLq0wZPuuIyrPx1G/JinhIiADjaGRfvPTFSNPL8=','2026-06-24 01:03:28',0,'2026-06-17 01:03:28'),(38,3,'yIzk4/wgLr5laWs0SW7WBaa6YYqRVH9TlVMcXGTa0hc=','2026-06-24 01:11:25',0,'2026-06-17 01:11:25'),(39,3,'KKg4TI/aNELph9f9Vo7kKDSg2ALhdvt+jXWT3ID2liA=','2026-06-24 01:15:42',0,'2026-06-17 01:15:42'),(40,3,'9OEWrT7G1EeJLtAXv8EIDm8Ww0VAkHx/dM83V393Y6o=','2026-06-24 01:15:43',0,'2026-06-17 01:15:43'),(41,3,'rgmP20JxHX/sSiSzthBRx6FUGV9Hl9RRpGpTPLWbuSU=','2026-06-24 01:17:30',0,'2026-06-17 01:17:30'),(42,3,'STxNmFxLHrEX2t4FQ6tz929nu3JuFCHzKcEmHvSC0e4=','2026-06-24 01:17:31',0,'2026-06-17 01:17:31'),(43,3,'4eQd8Qp/rqvAgrITu4kQSSF6Wb+hFYfY7YLT/y8NITY=','2026-06-24 01:17:31',0,'2026-06-17 01:17:31'),(44,3,'AW8azZQ0+sm1eZF2MPUC/H6DABVL6LlLSgf/xiqHg7o=','2026-06-24 01:18:24',0,'2026-06-17 01:18:24'),(45,3,'noJapyujNPooloeyfI4/+5bU3qcc9LxI0fAodxaAj5o=','2026-06-24 01:22:06',0,'2026-06-17 01:22:06'),(46,3,'b77Y++IvZZRncLt14L8sYiIcGALpKkABMpvF6roXfjA=','2026-06-24 01:22:08',0,'2026-06-17 01:22:08'),(47,3,'WuzDllJGC6CoDKLKNs+1mCjfKuMbxBmidV2NA5KTLSc=','2026-06-24 10:39:46',0,'2026-06-17 10:39:46'),(48,3,'FOpoJQz02TIBRKi/+6bY7TrTQ0d7YavTd06Dtvd6/XM=','2026-06-24 10:39:48',0,'2026-06-17 10:39:48'),(49,3,'J4H8h6mViKOu95kqPLficLXK8nZUxyaCKrZTzShcJKU=','2026-06-24 10:39:49',0,'2026-06-17 10:39:49'),(50,3,'V9eawM1Jqyv3aUt4eYFqjQ/KZskTdRVvybH0yH2oNTs=','2026-06-24 10:41:43',0,'2026-06-17 10:41:43'),(51,3,'V5Z8bfPmlHPhf2Mp3T2AOTM1LnIcE4SC18t4zkbLqhA=','2026-06-24 10:44:04',0,'2026-06-17 10:44:04'),(52,3,'SI82UhUXpB7CBL1TFH4e1WZrcNfIDhHtoAozXcPfwPo=','2026-06-24 10:48:56',0,'2026-06-17 10:48:56'),(53,3,'1w+XZOrON9MI3RTRDYGoj/q+QkdQ59usL/zljM+hb3Q=','2026-06-24 10:51:26',0,'2026-06-17 10:51:26'),(54,3,'zKCnzF4HlpLeEjvTIe3JE8QYVxKLcl6388FBj44Tvz0=','2026-06-24 10:55:24',0,'2026-06-17 10:55:24'),(55,3,'nXtb+1ToUw55rkYWcGX7BU92rIyjMo53L0bGxYoXWS8=','2026-06-24 10:57:44',0,'2026-06-17 10:57:44'),(56,3,'gosEJy1w/7oz2XgweqNA11GfajKhNOD70RWehmYHAUs=','2026-06-24 11:00:24',0,'2026-06-17 11:00:24'),(57,3,'FCRz05rI1c/UbOUHU/p0RVkZUacgsjm1jMnaCa2TFUM=','2026-06-24 11:04:00',0,'2026-06-17 11:04:00'),(58,3,'LGveVwleEoe6GJwXDM67cVY2ZPOsNQPNPCQsfz7OmVU=','2026-06-24 11:11:49',0,'2026-06-17 11:11:49'),(59,1,'Un5ZKpKCUZ3lA5vE32U0KyA5KMrnN8vANifLFN2YhLY=','2026-06-24 11:12:14',1,'2026-06-17 11:12:14'),(60,1,'U6Gir6eZqS3xYQ/uvH28Y0C0+7j7Hhqg/iPdNzbp1nY=','2026-06-24 11:17:37',0,'2026-06-17 11:17:37'),(61,4,'ty+JWhq8LobkHa0Khwp/6s5ZEe/p4PLdV4TVWEWRTdU=','2026-06-24 11:18:20',1,'2026-06-17 11:18:20'),(62,3,'Dja/RB5yPWH8tYnUKk7hAbW2XngsXpM4CYMy/XTz7tU=','2026-06-24 11:19:48',0,'2026-06-17 11:19:48'),(63,3,'6BxEFNvNWBVZ4TthJQbxfq68Uxf+jh/TSBErNmEfzRs=','2026-06-24 11:53:22',0,'2026-06-17 11:53:22'),(64,3,'bYzEF3hBfoAR/5IT+Zk+ee8ih5B2LS7kx504drm6pvM=','2026-06-24 12:12:42',0,'2026-06-17 12:12:42'),(65,3,'l9Q1K470LSthqB6ExaKG6gNMFGWIKWQg7akVJd/E5G4=','2026-06-24 12:16:47',0,'2026-06-17 12:16:47'),(66,3,'ZDdNNg86VW7ris8Yt9ZwTinRrwVd0+fnhpP0AzbaZ50=','2026-06-24 14:21:10',0,'2026-06-17 14:21:10'),(67,3,'r4u/5X112aA21nKcxG0AaEeIrH3qND9r18Feco6BF+Y=','2026-06-24 14:25:50',0,'2026-06-17 14:25:50'),(68,3,'MVn+xJotBuwe51CSCgy8vDKfCCt9BlIWlLPWH+MQc60=','2026-06-24 14:31:51',0,'2026-06-17 14:31:51'),(69,3,'uP832Pt9LDw8yousi57N60TVv7NkiCCNZ+vgxhb+ods=','2026-06-24 14:44:11',0,'2026-06-17 14:44:11'),(70,3,'PfdwfIKYVX0HaFBPTLiYh5gD2jB3zHiFVyb2Iort4KI=','2026-06-24 14:48:51',0,'2026-06-17 14:48:51'),(71,3,'EybiBdZq1URc8TcRLI4UW6HWjLD0M4JBNCVMX5kEugI=','2026-06-24 14:51:47',0,'2026-06-17 14:51:47'),(72,3,'MhbKNO2tWYqMOpU6zpmQqDpVx+NFOwaQPAZZGXjx69Y=','2026-06-24 15:46:03',0,'2026-06-17 15:46:03'),(73,3,'wyA/2OH3WIK63N9dkngxo+u4aeEN8si7Z52QV9qYoKQ=','2026-06-25 02:08:15',0,'2026-06-18 02:08:15'),(74,4,'ALDokS9KbLQRFF9UbvfZ11k18Sndn6/yVl+mU0SlbX0=','2026-06-25 02:09:06',0,'2026-06-18 02:09:06'),(75,3,'gJ6XwVXtIxAsssgttQ00zIXRyAH4ZBSvYYOnOFlapWM=','2026-06-25 02:16:35',0,'2026-06-18 02:16:35'),(76,3,'xMF4yQeJAZdiP+gyPaH51Udyby1VqFVAG2Tj+bHKll0=','2026-06-25 02:36:01',0,'2026-06-18 02:36:01'),(77,3,'Kyh1MymZa75F0LgK4z2WfYZwi38poODWS6KRzWzJpxk=','2026-06-25 02:43:45',0,'2026-06-18 02:43:45'),(78,3,'TkAt7If/0EFqcCTDTSZY1jBxAf6JlWZRRffpSPcIX5I=','2026-06-25 02:50:02',0,'2026-06-18 02:50:02'),(79,3,'ZvvwFFiTrm2E0ryCzrF5GZC2aZmTnVNT79PLb68gSB4=','2026-06-25 03:10:08',0,'2026-06-18 03:10:08'),(80,3,'xWFCh2aRz14kN8zds4kvlfuR4ZvItwQz1D6BgHi9TFE=','2026-06-25 03:14:00',0,'2026-06-18 03:14:00'),(81,3,'VylQo/dC+a0RY76HZVTmBf5W5iOQgwx877yiGghFHfI=','2026-06-25 06:07:41',0,'2026-06-18 06:07:41'),(82,3,'36oDm78cpIh7hngry+9pLS+Z86Mgxzaid6QyaZOIkaE=','2026-06-25 06:33:10',0,'2026-06-18 06:33:10'),(83,3,'6xiERvIsIEuiyUVJlUInWz06AbwPAoCU4TK7g9Ljsys=','2026-06-25 06:36:25',0,'2026-06-18 06:36:25'),(84,3,'tRBGrZa4RU4h9jubcQw+knpzxhW6a4RTayo98561IuI=','2026-06-25 06:45:43',0,'2026-06-18 06:45:43'),(85,3,'ZEbv5ZcBDVq1bMhhIihh4FQg+vxM9VTXW+Q/gS+nAn8=','2026-06-25 07:20:07',0,'2026-06-18 07:20:07'),(86,3,'OKS8cwAYgmWfq1ko2FzcvfolU4nO7VWo6p++knNy/9A=','2026-06-25 07:28:20',0,'2026-06-18 07:28:20'),(87,3,'xGgPxL/w59W/qZRCd7CDfeL5R1A3MMSWZT03JYJa6dA=','2026-06-25 07:41:26',0,'2026-06-18 07:41:26'),(88,3,'dYHU1XkEnsZhktrxzV0sZko1uDO+WcxqsZrKygDPEvY=','2026-06-25 07:46:17',0,'2026-06-18 07:46:17'),(89,3,'lLc+9orX4bQT4g/rVlptRrmnB+bLy+6KEqO+DoWQ390=','2026-06-25 07:56:00',0,'2026-06-18 07:56:00'),(90,3,'npd7HGzRMHyOBDGZGPGlqbCFCCtoajtdlHfWp8AYnok=','2026-06-25 08:02:53',0,'2026-06-18 08:02:53'),(91,3,'kQcP3xbTKgoXqsIHhDoh4RtTECADjeDNeBgpa0+LpsU=','2026-06-25 08:13:52',0,'2026-06-18 08:13:52'),(92,3,'3lAq/xHapSI81I4v/FcwIIkL71HVOh0FhX+RKBSVVU8=','2026-06-25 08:20:52',0,'2026-06-18 08:20:52'),(93,3,'7uO91Kmnw3nSd1+QH9yQZOHbOeuH5TvDExRBSfDk9nE=','2026-06-25 08:24:52',0,'2026-06-18 08:24:52'),(94,4,'a2AubyvF7+Qe6omPTwfWm2nKgBjYcDT64CrZYgRO3nw=','2026-06-25 09:20:50',0,'2026-06-18 09:20:50'),(95,1,'VLOw+QM1O/F8OtNk/VP+lDWQGZIJpf8ElHJ00znyGbU=','2026-06-25 09:25:14',0,'2026-06-18 09:25:14'),(96,4,'MACiF3hnBeSwoA+zIkR7naC4sYgYKIfGAm9GKb06bew=','2026-06-25 09:38:24',0,'2026-06-18 09:38:24'),(97,1,'BcYXdfSMnI5OGq0+8gX1JhtrkmOM6CL2Tj1OGOl2MRk=','2026-06-25 09:39:18',0,'2026-06-18 09:39:18'),(98,3,'Xm/IbeNSoYHK0lLCdbCOPPf4cEntEIXTnpnJ0YiF1SM=','2026-06-26 02:05:57',0,'2026-06-19 02:05:57'),(99,3,'mCcsLpvK6ElO6nNiVlhZVrcnETaKp78oQdOtP6cKdG0=','2026-06-26 02:19:26',0,'2026-06-19 02:19:26'),(100,3,'sMAHbIy4/gldC1ZlsR0s/4Z4suUyHQf1edSTvGHOr8A=','2026-06-26 02:23:01',0,'2026-06-19 02:23:01'),(101,3,'8iUHdeXr/lepjCUTZl0iurlhdQGiVLMrv3veJygZdJc=','2026-06-26 02:27:38',0,'2026-06-19 02:27:38'),(102,3,'cvPBzyjjL0Bk5fYZJRqh4llf50IFTltBSwHUOGWhW1c=','2026-06-26 02:31:56',0,'2026-06-19 02:31:56'),(103,3,'jvStHY5m0aFVuvDm5v7M9rvezFhfm36YPl0a+xVu+2k=','2026-06-26 02:51:28',0,'2026-06-19 02:51:28'),(104,3,'BzhKkeocpFo21UjGzna2O47QFdqokmljgXlyTrj9g8k=','2026-06-26 03:04:27',0,'2026-06-19 03:04:27'),(105,3,'2iRyL0/s9tP/s+LV4sgmsadc+FwgBk1DYIo9bVJHmNQ=','2026-06-28 08:55:37',0,'2026-06-21 08:55:37'),(106,3,'wtXk4cObF+IcMedvFhCvOd0WSCTHhM9BeSflpnwWbJI=','2026-06-28 09:04:21',0,'2026-06-21 09:04:21'),(107,1,'xBbDcF4ooAzmFSvlFcvezGNbKl7L9PGbVpkLfxbQDZM=','2026-06-28 09:09:00',0,'2026-06-21 09:09:00'),(108,3,'WHwCsTGHEkvnWYaZAnANCUzJQH/j/YLwlL17IA13inc=','2026-06-28 09:10:43',0,'2026-06-21 09:10:43'),(109,3,'tZSJQlNHVF9MFk2KnsriE8A1r0C4346C6rduf71j6UU=','2026-06-28 09:25:48',0,'2026-06-21 09:25:48'),(110,3,'LX/ZuNPtLG5cEk45RyAyI5FDIl6g2qY/QiqF7C1vye4=','2026-06-28 10:21:51',0,'2026-06-21 10:21:51'),(111,3,'Jel5n9DYhLL2jUj34Sg5HuyyPP38B+W5DUa5J3tXtc4=','2026-06-28 10:25:51',0,'2026-06-21 10:25:51'),(112,3,'Vj8I8GUS3lcMLX39oe0N2EdAZ7UF8mUE34hJdmlOkGU=','2026-06-28 10:32:22',0,'2026-06-21 10:32:22'),(113,3,'B+uVLW5UvZNCpEwCM77aqdU0LZc9RGR6f94r+K2Cyy0=','2026-06-28 12:33:50',0,'2026-06-21 12:33:50'),(114,3,'bFwXlO8DiQpunwOT/q1bbbioHzp2Ff5qoeBI7qqIVq8=','2026-06-28 12:42:18',0,'2026-06-21 12:42:18'),(115,3,'1mgZRYoC03hDTiBJ813wbQZ5YGjELScgwi6QjhPn4WQ=','2026-06-28 12:48:20',0,'2026-06-21 12:48:20'),(116,3,'yPfq3Jx4NsITN58gIE//8WujJllqfOGor9daNtDT+8g=','2026-06-28 12:52:12',0,'2026-06-21 12:52:12');
/*!40000 ALTER TABLE `refresh_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `students`
--

DROP TABLE IF EXISTS `students`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `students` (
  `id` int NOT NULL AUTO_INCREMENT,
  `no` varchar(10) COLLATE utf8mb4_general_ci NOT NULL COMMENT '瀛﹀彿',
  `name` varchar(30) COLLATE utf8mb4_general_ci NOT NULL COMMENT '濮撳悕',
  `id_number` varchar(18) COLLATE utf8mb4_general_ci NOT NULL COMMENT '韬唤璇?,
  `gender` int NOT NULL COMMENT '鎬у埆',
  `birthday` datetime NOT NULL COMMENT '鍑虹敓鏃ユ湡',
  `weight` int NOT NULL COMMENT '浣撻噸(鍏枻)',
  `height` decimal(3,2) NOT NULL COMMENT '韬珮(绫?',
  `created_time` datetime NOT NULL,
  `ethnic_group` int NOT NULL COMMENT '姘戞棌',
  `native_place` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT '绫嶈疮鍦?,
  `Ver` bigint NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='瀛︾敓淇℃伅琛?;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `students`
--

LOCK TABLES `students` WRITE;
/*!40000 ALTER TABLE `students` DISABLE KEYS */;
/*!40000 ALTER TABLE `students` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_name` varchar(30) COLLATE utf8mb4_general_ci NOT NULL,
  `password` varchar(200) COLLATE utf8mb4_general_ci NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `nickname` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `avatar` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `signature` varchar(500) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `online_status` tinyint(1) NOT NULL,
  `last_login_time` datetime DEFAULT NULL,
  `created_time` datetime NOT NULL,
  `role` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'test','123456','test@example.com','娴嬭瘯鐢ㄦ埛',NULL,NULL,1,'2026-06-21 09:09:00','2026-06-11 04:34:33','user'),(2,'','',NULL,NULL,NULL,NULL,0,NULL,'2026-06-11 11:23:12','user'),(3,'admin','123456',NULL,'绠＄悊鍛?,NULL,NULL,1,'2026-06-21 12:52:12','2026-06-11 12:00:31','admin'),(4,'lds','123456',NULL,'lds',NULL,NULL,1,'2026-06-18 09:38:24','2026-06-17 11:16:48','user'),(5,'testuser123','123456',NULL,NULL,NULL,NULL,0,'2026-06-21 10:16:53','2026-06-21 10:16:53','user');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-22 13:46:24
