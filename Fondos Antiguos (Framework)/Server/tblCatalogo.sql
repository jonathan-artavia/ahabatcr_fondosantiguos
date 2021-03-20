CREATE TABLE `aha`.`tblCatalogo` (
  `ID` BIGINT NOT NULL AUTO_INCREMENT,
  `Contenido` MEDIUMTEXT, -- Acc:Descr y Ex:Contenido
  `Fecha` DATE DEFAULT NULL,
  `Signatura` TINYTEXT, -- Acc
  `Observaciones` LONGTEXT, -- Acc:Nota y Ex:Rubro
  `IdSerie` BIGINT DEFAULT NULL, -- Ex, era Texto, sera su propio mant
  `Fichero` tinytext NULL, -- Nuevo. Para poder armar la signatura
  `NumCaja` int, -- Ex y (Acc:Fichero formateado o de Signatura)
  `NumTomo` INT DEFAULT NULL, -- Ex
  `Folio` TINYTEXT DEFAULT NULL, -- Ex
  `Libro` INT DEFAULT NULL, -- Ex
  `NumExpediente` INT DEFAULT NULL, -- Ex
  `NumCarpeta` INT DEFAULT NULL, -- Ex
  `Lugar` TINYTEXT, -- Acc y Ex
  `Materia` mediumtext, --, FK to tblMateria
  -- `FechaCod` removed
  `Año` SMALLINT(6) DEFAULT NULL,
  `Mes` TINYINT(4) DEFAULT NULL,
  `FechaIngreso` datetime NOT NULL,
  `Materias` MEDIUMTEXT NULL,-- Acc
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=19641 DEFAULT CHARSET=latin1