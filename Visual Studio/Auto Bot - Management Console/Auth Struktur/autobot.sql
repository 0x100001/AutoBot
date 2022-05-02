-- phpMyAdmin SQL Dump
-- version 4.6.6
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Erstellungszeit: 02. Nov 2020 um 16:43
-- Server-Version: 5.5.60-0+deb7u1
-- PHP-Version: 5.6.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `ni4155434_1sql1`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `licences`
--

CREATE TABLE `licences` (
  `id` int(10) NOT NULL,
  `username` varchar(255) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  `banned_reason` varchar(255) DEFAULT NULL,
  `license_expire` date DEFAULT NULL,
  `license_type` varchar(3) DEFAULT NULL,
  `mysql_host` text NOT NULL,
  `mysql_database` text NOT NULL,
  `mysql_username` text NOT NULL,
  `mysql_password` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Daten für Tabelle `licences`
--

INSERT INTO `licences` (`id`, `username`, `password`, `banned_reason`, `license_expire`, `license_type`, `mysql_host`, `mysql_database`, `mysql_username`, `mysql_password`) VALUES
(1, 'admin', '101', NULL, '2020-12-17', 'rh', 'WmpGQ1oydHRjODUwbDRkSG8zNFRHSmsyUkVqWlc3bC96RnNwQlI3dzVNK3VTS1hZdTEzWlZYbXd6UWJua0hLYURLb2c3RHBsYU5wY0hkekxxMllTKzNjY2t6THRLbk1UenI3alJ3bWJXUEFFbDJDdkRmNk5BNEVPOVNXd1Y2Zy8=', 'Mkp2K1RMWklhYXBmVXdPazVLdEpDaW8rYjNiQitLd0hlTFd1ZFdwMzBNZGtwcitYblRMN0EyMEVTWmVEdmJNK3VjOTI5YXZTZHhjdUh0ekNJOHhab1ArdTBZUXJTWlBOay85RERZdWNjNlVXTVhBbGZ1Y2JYQ0ZQQzUzTmhFb0k=', 'a3ZMNjlVS0JRdUJ3UFR4bWU4a3BNOW0zTnlDT29xbEs3Tm9BNDM1UjlUWkE4S2tQTHhwbjVhN1NzT3hna243Y2hhWk5pMm9yRTcvNkhOZFVJM0dFbDdlSnJtQnZPdVVtZVBvOVVnOE5Xb3ZXZ25URllOYUh1amNFUkJkVmtYWjU=', 'SWpPU2luV1N5emt4ZXF4SXREZS9xdVFwSmQ0WVlKZktLTHhuWHpxN3JBUEVkaUF3WTdjWVdSMmVQSWxWQlIzUVJyaFpBeEhUemxaWHBvN2htUHgvUW5IT1l4Q0N5d25adHpWZWZJZHVmdjBFOXVaeGJOZFd3c3hDYVRwU0NEUHRkL1ZFRUtnQURZb09qbytNeTNuRWlITTJnSUxCM3Nsa1h1R3Z4MHNmSGt0Y2JoZzR5R2crUVRmdERxY0FveFFUb3lqWHNDeFM4d3ZlMTZJa0N6WDNWdz09');

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `licences`
--
ALTER TABLE `licences`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `licences`
--
ALTER TABLE `licences`
  MODIFY `id` int(10) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
