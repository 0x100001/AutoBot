/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Exportiere Datenbank Struktur für admin_autobot
CREATE DATABASE IF NOT EXISTS `admin_autobot` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `admin_autobot`;

-- Exportiere Struktur von Tabelle admin_autobot.account_generator
CREATE TABLE IF NOT EXISTS `account_generator` (
  `id` int NOT NULL AUTO_INCREMENT,
  `mail_domain` varchar(255) DEFAULT NULL,
  `gender` int DEFAULT NULL,
  `distributor_first_name` varchar(255) DEFAULT NULL,
  `distributor_last_name` varchar(255) DEFAULT NULL,
  `distributor_country` varchar(255) DEFAULT NULL,
  `distributor_phone_number` varchar(255) DEFAULT NULL,
  `distributor_hdykau` varchar(255) DEFAULT NULL,
  `distributor_street_number` varchar(255) DEFAULT NULL,
  `distributor_plz_city` varchar(255) DEFAULT NULL,
  `distributor_artist_name` varchar(255) DEFAULT NULL,
  `distributor_mail` varchar(255) DEFAULT NULL,
  `distributor_password` varchar(255) DEFAULT NULL,
  `cpanel_mail_password` varchar(255) DEFAULT NULL,
  `cpanel_mail_amount` int DEFAULT NULL,
  `roundcube_url` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.artist_manager
CREATE TABLE IF NOT EXISTS `artist_manager` (
  `id` int NOT NULL AUTO_INCREMENT,
  `distributor` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Recordjet',
  `release_status` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'Pending',
  `artist_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'encryptedcontent',
  `first_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'encryptedcontent',
  `last_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'encryptedcontent',
  `mail` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'encryptedcontent',
  `password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'encryptedcontent',
  `copyright` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'encryptedcontent',
  `country` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'encryptedcontent',
  `street_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'encryptedcontent',
  `plz_city` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'encryptedcontent',
  `phone_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'encryptedcontent',
  `notes` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `paypal_mail` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'encryptedcontent',
  `bank_iban` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'encryptedcontent',
  `bank_bic` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'encryptedcontent',
  `username` varchar(255) DEFAULT 'encryptedcontent',
  `hive_name` varchar(255) DEFAULT '-',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=89 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.artist_manager_payout_history
CREATE TABLE IF NOT EXISTS `artist_manager_payout_history` (
  `id` int NOT NULL AUTO_INCREMENT,
  `artist_name` varchar(255) NOT NULL DEFAULT '0',
  `datetime` datetime DEFAULT NULL,
  `type` varchar(255) DEFAULT NULL,
  `detail` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `value` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.clients
CREATE TABLE IF NOT EXISTS `clients` (
  `id` int NOT NULL AUTO_INCREMENT,
  `client_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '-',
  `last_access` datetime DEFAULT '2000-01-01 00:00:00',
  `spotify_enabled` int DEFAULT '0',
  `spotify_playlist_url` varchar(500) DEFAULT NULL,
  `spotify_player_account` varchar(500) DEFAULT NULL,
  `spotify_player_password` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `spotify_autoskip_enabled` int DEFAULT '1',
  `spotify_autoskip_forward_from` int DEFAULT '60000',
  `spotify_autoskip_forward_to` int DEFAULT '90000',
  `deezer_enabled` int DEFAULT '0',
  `deezer_playlist_url` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `deezer_player_account` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `deezer_player_password` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `deezer_autoskip_enabled` int DEFAULT '1',
  `deezer_autoskip_forward_from` int DEFAULT '60000',
  `deezer_autoskip_forward_to` int DEFAULT '90000',
  `auto_restart_bot` int DEFAULT '1',
  `auto_restart_bot_timer` int DEFAULT '14400000',
  `openvpn_enabled` int DEFAULT '1',
  `openvpn_randomize` int DEFAULT '1',
  `openvpn_health_check` int DEFAULT '1',
  `openvpn_profile` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '-',
  `custom_profile_enabled` int DEFAULT '0',
  `player_type` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'Playlist',
  `play_time_sheduler_monday_enabled` int DEFAULT '1',
  `play_time_sheduler_tuesday_enabled` int DEFAULT '1',
  `play_time_sheduler_wednesday_enabled` int DEFAULT '1',
  `play_time_sheduler_thursday_enabled` int DEFAULT '1',
  `play_time_sheduler_friday_enabled` int DEFAULT '1',
  `play_time_sheduler_saturday_enabled` int DEFAULT '1',
  `play_time_sheduler_sunday_enabled` int DEFAULT '1',
  `reinstall_chrome` int DEFAULT '0',
  `custom_useragent_enabled` int DEFAULT '0',
  `custom_useragent` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'None',
  `rotate_credentials_enabled` int DEFAULT '0',
  `emulate_device_enabled` int DEFAULT '0',
  `emulate_device_type` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'iPhone X',
  `renew_windows_trial_licence` int DEFAULT '0',
  `hive_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '-',
  `automation_mode` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'Selenium',
  `openvpn_bound_credentials_enabled` int DEFAULT '1',
  `openvpn_login_profile` mediumtext,
  `chrome_persistent_profiles_enabled` int DEFAULT '0',
  `cookies_file` mediumtext,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=121 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.deezer_credentials
CREATE TABLE IF NOT EXISTS `deezer_credentials` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(500) DEFAULT NULL,
  `password` varchar(500) DEFAULT NULL,
  `client_name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.deezer_playlists
CREATE TABLE IF NOT EXISTS `deezer_playlists` (
  `id` int NOT NULL AUTO_INCREMENT,
  `url` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.deezer_playlist_accounts
CREATE TABLE IF NOT EXISTS `deezer_playlist_accounts` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(255) NOT NULL DEFAULT '0',
  `password` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=118 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.deezer_song_urls
CREATE TABLE IF NOT EXISTS `deezer_song_urls` (
  `id` int NOT NULL AUTO_INCREMENT,
  `url` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=360 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.hives
CREATE TABLE IF NOT EXISTS `hives` (
  `id` int NOT NULL AUTO_INCREMENT,
  `hive_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '-',
  `playlist_poly_index` int DEFAULT '-1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.openvpn_profiles
CREATE TABLE IF NOT EXISTS `openvpn_profiles` (
  `id` int NOT NULL AUTO_INCREMENT,
  `profile_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `profile_data` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `login_conf` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `hive_name` varchar(255) DEFAULT '-',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.settings
CREATE TABLE IF NOT EXISTS `settings` (
  `id` int NOT NULL AUTO_INCREMENT,
  `telegram_active` int DEFAULT NULL,
  `telegram_api_token` varchar(500) DEFAULT NULL,
  `telegram_chat_id` varchar(500) DEFAULT NULL,
  `telegram_alert_chat_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `telegram_alert_creds_chat_id` varchar(255) DEFAULT NULL,
  `screenlock_enabled` int DEFAULT NULL,
  `screenlock_password` varchar(500) DEFAULT NULL,
  `namescheap_username` varchar(255) DEFAULT NULL,
  `namescheap_password` varchar(255) DEFAULT NULL,
  `artist_manager_roundcube_mail_password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `artist_manager_roundcube_mail_domain` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `distributor_password` varchar(255) DEFAULT NULL,
  `playlist_poly_index` int DEFAULT NULL,
  `cpanel_url` varchar(255) DEFAULT NULL,
  `cpanel_username` varchar(255) DEFAULT NULL,
  `cpanel_password` varchar(255) DEFAULT NULL,
  `client_mail_domain` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `client_mail_password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ssh_remote_hostname` varchar(255) DEFAULT NULL,
  `ssh_username` varchar(255) DEFAULT NULL,
  `ssh_password` varchar(255) DEFAULT NULL,
  `ssh_port` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.songs
CREATE TABLE IF NOT EXISTS `songs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `artist_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `song_name` varchar(255) DEFAULT NULL,
  `spotify_url` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `hive_name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=576 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.spotify_credentials
CREATE TABLE IF NOT EXISTS `spotify_credentials` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(500) DEFAULT NULL,
  `password` varchar(500) DEFAULT NULL,
  `client_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '-',
  `outdated` int DEFAULT '0',
  `premium` varchar(255) DEFAULT 'No.',
  `hive_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '-',
  `openvpn_profile` varchar(255) DEFAULT '-',
  `cookies_file` mediumtext,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1788 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.spotify_playlists
CREATE TABLE IF NOT EXISTS `spotify_playlists` (
  `id` int NOT NULL AUTO_INCREMENT,
  `url` varchar(500) DEFAULT NULL,
  `type` varchar(255) DEFAULT NULL,
  `hive_name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=236 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.spotify_playlist_accounts
CREATE TABLE IF NOT EXISTS `spotify_playlist_accounts` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(255) NOT NULL DEFAULT '0',
  `password` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=609 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle admin_autobot.useragents
CREATE TABLE IF NOT EXISTS `useragents` (
  `id` int NOT NULL AUTO_INCREMENT,
  `useragent_name` varchar(255) DEFAULT NULL,
  `useragent_string` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten Export vom Benutzer nicht ausgewählt

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
