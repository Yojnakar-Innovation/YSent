CREATE DATABASE  IF NOT EXISTS `ysent` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `ysent`;
-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: ysent
-- ------------------------------------------------------
-- Server version	8.0.37

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bounces`
--

DROP TABLE IF EXISTS `bounces`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bounces` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CampaignId` int DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `BounceDate` datetime DEFAULT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CampaignId` (`CampaignId`),
  CONSTRAINT `bounces_ibfk_1` FOREIGN KEY (`CampaignId`) REFERENCES `campaigns` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bounces`
--

LOCK TABLES `bounces` WRITE;
/*!40000 ALTER TABLE `bounces` DISABLE KEYS */;
/*!40000 ALTER TABLE `bounces` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `camp`
--

DROP TABLE IF EXISTS `camp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `camp` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `title` varchar(255) NOT NULL,
  `subject` varchar(255) NOT NULL,
  `body` text,
  `status` enum('draft','sent','scheduled') DEFAULT 'draft',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `camp_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `camp`
--

LOCK TABLES `camp` WRITE;
/*!40000 ALTER TABLE `camp` DISABLE KEYS */;
/*!40000 ALTER TABLE `camp` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `campaigns`
--

DROP TABLE IF EXISTS `campaigns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `campaigns` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Subject` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Body` longtext NOT NULL,
  `SentDate` datetime DEFAULT NULL,
  `FromAddress` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RecipientCount` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `campaigns`
--

LOCK TABLES `campaigns` WRITE;
/*!40000 ALTER TABLE `campaigns` DISABLE KEYS */;
/*!40000 ALTER TABLE `campaigns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contacts`
--

DROP TABLE IF EXISTS `contacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contacts` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `name` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `group_id` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  KEY `group_id` (`group_id`),
  CONSTRAINT `contacts_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
  CONSTRAINT `contacts_ibfk_2` FOREIGN KEY (`group_id`) REFERENCES `groups` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contacts`
--

LOCK TABLES `contacts` WRITE;
/*!40000 ALTER TABLE `contacts` DISABLE KEYS */;
/*!40000 ALTER TABLE `contacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `draftemails`
--

DROP TABLE IF EXISTS `draftemails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `draftemails` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Subject` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RecipientEmails` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Body` longtext NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `draftemails`
--

LOCK TABLES `draftemails` WRITE;
/*!40000 ALTER TABLE `draftemails` DISABLE KEYS */;
INSERT INTO `draftemails` VALUES (2,'Testing067','oodurito@gmail.com','hh'),(9,'Testing01','oodurito@gmail.com, sodepak691@nike4s.com','<h1><a href=\"www.google.com\" target=\"_blank\"><strong>dadf<span class=\"ql-cursor\">﻿﻿﻿﻿﻿</span></strong></a></h1>'),(10,'0121testing','oodurito@gmail.com, sodepak691@nike4s.com','<h1><a href=\"www.google.com\" target=\"_blank\"><strong>dadf<span class=\"ql-cursor\">﻿﻿﻿</span></strong></a></h1>'),(11,'kio','oodurito@gmail.com, sodepak691@nike4s.com','<h1><a href=\"www.google.com\" target=\"_blank\"><strong>dadf<span class=\"ql-cursor\">﻿﻿</span></strong></a></h1>'),(12,'nknm','qwe@gmail.com, hjs@gmail.com','<h1><a href=\"www.google.com\" target=\"_blank\"><strong>dadf<span class=\"ql-cursor\">﻿﻿</span></strong></a></h1>');
/*!40000 ALTER TABLE `draftemails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `emailopens`
--

DROP TABLE IF EXISTS `emailopens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `emailopens` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CampaignId` int DEFAULT NULL,
  `SubscriberId` int DEFAULT NULL,
  `OpenDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CampaignId` (`CampaignId`),
  CONSTRAINT `emailopens_ibfk_1` FOREIGN KEY (`CampaignId`) REFERENCES `campaigns` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `emailopens`
--

LOCK TABLES `emailopens` WRITE;
/*!40000 ALTER TABLE `emailopens` DISABLE KEYS */;
/*!40000 ALTER TABLE `emailopens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `groups`
--

DROP TABLE IF EXISTS `groups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `groups` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `name` varchar(255) NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `groups_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `groups`
--

LOCK TABLES `groups` WRITE;
/*!40000 ALTER TABLE `groups` DISABLE KEYS */;
/*!40000 ALTER TABLE `groups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `linkactivities`
--

DROP TABLE IF EXISTS `linkactivities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `linkactivities` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CampaignId` int DEFAULT NULL,
  `OriginalUrl` varchar(2048) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `TrackedUrl` varchar(2048) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `UniqueClicks` int DEFAULT '0',
  `TotalClicks` int DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `CampaignId` (`CampaignId`),
  CONSTRAINT `linkactivities_ibfk_1` FOREIGN KEY (`CampaignId`) REFERENCES `campaigns` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `linkactivities`
--

LOCK TABLES `linkactivities` WRITE;
/*!40000 ALTER TABLE `linkactivities` DISABLE KEYS */;
/*!40000 ALTER TABLE `linkactivities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `list`
--

DROP TABLE IF EXISTS `list`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `list` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `list_name` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `list`
--

LOCK TABLES `list` WRITE;
/*!40000 ALTER TABLE `list` DISABLE KEYS */;
INSERT INTO `list` VALUES (1,'raj','ev','qwe@gmail.com','2025-01-30 12:05:01'),(2,'rajansi','sion','lx@gmail.com','2025-01-30 12:05:10'),(4,'rajiiiig','gh','goat@gmail.com','2025-01-30 12:58:37'),(5,'raj','ev','hjs@gmail.com','2025-01-30 13:16:59'),(6,'rajo','siond','nav@gmail.com','2025-01-30 13:26:18'),(7,'fira','otg','fir@gmail.com','2025-01-31 04:29:25'),(8,'dv','dk1','oodurito@gmail.com','2025-02-06 10:25:41'),(9,'kj','dk1','sodepak691@nike4s.com','2025-02-06 19:39:45');
/*!40000 ALTER TABLE `list` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `logs`
--

DROP TABLE IF EXISTS `logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `logs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `campaign_id` int NOT NULL,
  `contact_id` int NOT NULL,
  `status` enum('pending','delivered','failed') DEFAULT 'pending',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `campaign_id` (`campaign_id`),
  KEY `contact_id` (`contact_id`),
  CONSTRAINT `logs_ibfk_1` FOREIGN KEY (`campaign_id`) REFERENCES `camp` (`id`) ON DELETE CASCADE,
  CONSTRAINT `logs_ibfk_2` FOREIGN KEY (`contact_id`) REFERENCES `contacts` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `logs`
--

LOCK TABLES `logs` WRITE;
/*!40000 ALTER TABLE `logs` DISABLE KEYS */;
/*!40000 ALTER TABLE `logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `password_reset_tokens`
--

DROP TABLE IF EXISTS `password_reset_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `password_reset_tokens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `token` varchar(255) NOT NULL,
  `expires_at` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `password_reset_tokens_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `password_reset_tokens`
--

LOCK TABLES `password_reset_tokens` WRITE;
/*!40000 ALTER TABLE `password_reset_tokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `password_reset_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sentemails`
--

DROP TABLE IF EXISTS `sentemails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sentemails` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Subject` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RecipientEmails` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Body` longtext NOT NULL,
  `SentTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `opened` varchar(50) DEFAULT 'no',
  `OpenDate` datetime DEFAULT NULL,
  `UserAgent` text,
  `IPAddress` varchar(45) DEFAULT NULL,
  `TrackingId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sentemails`
--

LOCK TABLES `sentemails` WRITE;
/*!40000 ALTER TABLE `sentemails` DISABLE KEYS */;
INSERT INTO `sentemails` VALUES (1,'testing','oodurito@gmail.com','testing101','2025-02-02 00:24:56','no',NULL,NULL,NULL,NULL),(3,'uit','oodurito@gmail.com','mx','2025-02-06 23:41:43','no',NULL,NULL,NULL,NULL),(5,'wbc conference','oodurito@gmail.com,sodepak691@nike4s.com,sodepak691+abc@nike4s.com,oodurito+1@gmail.com','ok','2025-02-07 16:15:01','no',NULL,NULL,NULL,NULL),(6,'Divine Blessing','oodurito@gmail.com,sodepak691@nike4s.com','<h1 class=\"ql-indent-2\"><a href=\"www.google.com\" target=\"_blank\"><strong><img src=\"/imageupload/uploads/4fc6349b-b60c-4487-96ce-0d722f2d7fec_2.png\"></strong></a></h1><p class=\"ql-indent-6\"><br></p><p class=\"ql-indent-6\"><br></p><h1 class=\"ql-indent-6\"><a href=\"www.google.com\" target=\"_blank\"><strong>click to confirm</strong></a></h1>','2025-02-12 10:47:37','no',NULL,NULL,NULL,NULL),(10,'route121','qwe@gmail.com,hjs@gmail.com','<h1><a href=\"www.google.com\" target=\"_blank\"><strong>dadf<span class=\"ql-cursor\">﻿﻿</span></strong></a></h1>','2025-02-13 13:12:31','no',NULL,NULL,NULL,NULL),(14,'final testing','oodurito@gmail.com,sodepak691@nike4s.com','<h1 class=\"ql-indent-6\"><a href=\"www.google.com\" target=\"_blank\"><strong><img src=\"/imageupload/uploads/4fc6349b-b60c-4487-96ce-0d722f2d7fec_2.png\"></strong></a></h1><p class=\"ql-indent-6\"><br></p><p class=\"ql-indent-6\"><br></p><h1 class=\"ql-indent-6\"><a href=\"www.google.com\" target=\"_blank\"><strong>click to confirm</strong></a></h1>','2025-02-16 18:02:02','no',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `sentemails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `sign_in`
--

DROP TABLE IF EXISTS `sign_in`;
/*!50001 DROP VIEW IF EXISTS `sign_in`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `sign_in` AS SELECT 
 1 AS `username`,
 1 AS `password`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `spamreports`
--

DROP TABLE IF EXISTS `spamreports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `spamreports` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CampaignId` int DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `ReportDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CampaignId` (`CampaignId`),
  CONSTRAINT `spamreports_ibfk_1` FOREIGN KEY (`CampaignId`) REFERENCES `campaigns` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `spamreports`
--

LOCK TABLES `spamreports` WRITE;
/*!40000 ALTER TABLE `spamreports` DISABLE KEYS */;
/*!40000 ALTER TABLE `spamreports` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `subscriber`
--

DROP TABLE IF EXISTS `subscriber`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `subscriber` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ListName` varchar(255) DEFAULT NULL,
  `SubscribedDate` datetime NOT NULL,
  `Status` enum('Active','Unsubscribed','Bounced','spam') NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `subscriber`
--

LOCK TABLES `subscriber` WRITE;
/*!40000 ALTER TABLE `subscriber` DISABLE KEYS */;
/*!40000 ALTER TABLE `subscriber` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `templates`
--

DROP TABLE IF EXISTS `templates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `templates` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT '0',
  `template_name` varchar(255) NOT NULL,
  `plain_text` text,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `html_content` varchar(16000) DEFAULT NULL,
  `image_url` longtext,
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `templates_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `templates`
--

LOCK TABLES `templates` WRITE;
/*!40000 ALTER TABLE `templates` DISABLE KEYS */;
INSERT INTO `templates` VALUES (26,NULL,'Nirvaana Subscription','confirm your subscription','2025-02-11 23:22:17','<h1 class=\"ql-indent-6\"><a href=\"www.google.com\" target=\"_blank\"><strong><img src=\"/imageupload/uploads/4fc6349b-b60c-4487-96ce-0d722f2d7fec_2.png\"></strong></a></h1><p class=\"ql-indent-6\"><br></p><p class=\"ql-indent-6\"><br></p><h1 class=\"ql-indent-6\"><a href=\"www.google.com\" target=\"_blank\"><strong>click to confirm</strong></a></h1>','/imageupload/uploads/4fc6349b-b60c-4487-96ce-0d722f2d7fec_2.png'),(27,NULL,'Birthday Invitation','Birthday invitation confirmation','2025-02-12 00:23:07','<h1 class=\"ql-indent-3\"><img src=\"/imageupload/uploads/cb1cd9d6-f7d8-4ac9-ace6-daf2717bcdcd_Screenshot (545).png\"></h1><p class=\"ql-indent-3\"><br></p><p class=\"ql-indent-3\"><br></p><h2 class=\"ql-indent-3\">                                    <a href=\"WWW.GOOGLE.COM\" target=\"_blank\"><strong>   CLICK TO CONFIRM YOUR INVITATION</strong></a></h2>','/imageupload/uploads/cb1cd9d6-f7d8-4ac9-ace6-daf2717bcdcd_Screenshot (545).png');
/*!40000 ALTER TABLE `templates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `unsubscribes`
--

DROP TABLE IF EXISTS `unsubscribes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `unsubscribes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CampaignId` int DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `UnsubscribeDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CampaignId` (`CampaignId`),
  CONSTRAINT `unsubscribes_ibfk_1` FOREIGN KEY (`CampaignId`) REFERENCES `campaigns` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unsubscribes`
--

LOCK TABLES `unsubscribes` WRITE;
/*!40000 ALTER TABLE `unsubscribes` DISABLE KEYS */;
/*!40000 ALTER TABLE `unsubscribes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `email` varchar(255) DEFAULT NULL,
  `is_admin` tinyint(1) NOT NULL DEFAULT '0',
  `reset_token` varchar(255) DEFAULT NULL,
  `reset_token_expiry` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (5,'shivraj','Di1137','9090909090','2025-02-15 06:36:18','oodurito@gmail.com',0,NULL,NULL),(6,'demo','Demo1','aaaassss','2025-02-16 10:53:38','sodepak691@nike4s.com',0,NULL,NULL);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Final view structure for view `sign_in`
--

/*!50001 DROP VIEW IF EXISTS `sign_in`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `sign_in` AS select `users`.`username` AS `username`,`users`.`password` AS `password` from `users` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-02-18  0:36:57
