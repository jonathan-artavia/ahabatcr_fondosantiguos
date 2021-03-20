CREATE TABLE `aha`.`tblHistCatalogo` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `Descripcion` MEDIUMTEXT NULL,
  `FechaOrig` TINYTEXT NULL,
  `Fecha` DATETIME NULL,
  `Signatura` TINYTEXT NULL,
  `Datos` LONGTEXT NULL,
  `Fichero` TINYTEXT NULL,
  `Lugar` TINYTEXT NULL,
  `Materias` MEDIUMTEXT NULL,
  `FechaCod` TINYTEXT NULL,
  `Año` SMALLINT(6) DEFAULT NULL,
  `Mes` TINYINT(4) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC));



UPDATE aha.tblHistCatalogo SET
Fecha = 
	(CASE WHEN FechaCod = 0 OR FechaCod = NULL OR length(FechaCod+'') < 6 
		OR substr(FechaCod, 5, 2) = '00'
        OR substr(FechaCod, 7, 2) = '00'
        OR TO_DAYS(str_to_date(FechaCod+'', '%Y%m%d')) IS NULL
		OR (MONTH(DATE(str_to_date(FechaCod+'', '%Y%m%d'))) = 2 
			AND DATE(DATE_ADD(str_to_date(FechaCod+'', '%Y%m%d'), INTERVAL 0 DAY)) != DATE(str_to_date(FechaCod+'', '%Y%m%d')))
		OR DATE_ADD(DATE(str_to_date(FechaCod+'', '%Y%m%d')), INTERVAL 0 DAY) = null
        OR MONTH(DATE(str_to_date(FechaCod+'', '%Y%m%d'))) = 0
        OR YEAR(DATE(str_to_date(FechaCod+'', '%Y%m%d'))) = 0
        
	THEN NULL
		ELSE str_to_date(FechaCod+'', '%Y%m%d')
	END)
WHERE ID > 0