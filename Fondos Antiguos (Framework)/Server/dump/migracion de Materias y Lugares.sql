START TRANSACTION;

-- Migrar Lugares

INSERT INTO `tblLugar` (`Nombre`)
SELECT nombre FROM
( SELECT SUBSTRING_INDEX(SUBSTRING_INDEX(c.Lugar, '/', numeros.n), '/', -1) nombre
FROM
  (SELECT 1 n 
   UNION ALL
   SELECT 2 
   UNION ALL 
   SELECT 3 
   UNION ALL
   SELECT 4 
   UNION ALL 
   SELECT 5) numeros
INNER JOIN tblCatalogo c
  ON CHAR_LENGTH(c.Lugar) - CHAR_LENGTH(REPLACE(c.Lugar, '/', '')) >= numeros.n-1
LEFT JOIN tblLugar l ON l.Nombre = SUBSTRING_INDEX(SUBSTRING_INDEX(c.Lugar, '/', numeros.n), '/', -1)
WHERE l.ID IS NULL ) ds
GROUP BY  `nombre`;

INSERT INTO `tblCatalogoLugar` (`Catalogo_ID`, `Lugar_ID`)
SELECT cx.ID, l.ID
FROM
	(
    SELECT * FROM (
		SELECT c.ID, TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(c.Lugar, '/', numeros.n), '/', -1)) nombre
		FROM
		  (SELECT 1 n 
		   UNION ALL
		   SELECT 2 
		   UNION ALL 
		   SELECT 3 
		   UNION ALL
		   SELECT 4 
		   UNION ALL 
		   SELECT 5) numeros
		INNER JOIN tblCatalogo c
		  ON CHAR_LENGTH(c.Lugar) - CHAR_LENGTH(REPLACE(c.Lugar, '/', '')) >= numeros.n-1 ) ds
	GROUP BY ID, `nombre`
    ) cx
INNER JOIN tblLugar l ON l.Nombre = cx.nombre
LEFT JOIN tblCatalogoLugar cl ON cl.Catalogo_ID = cx.ID AND cl.Lugar_ID = l.ID
WHERE cl.ID IS NULL;

-- Migrar Materias

INSERT INTO `tblMateria` (`Nombre`)
SELECT nombre FROM (
	SELECT SUBSTRING_INDEX(SUBSTRING_INDEX(c.Materias, ' / ', numeros.n), ' / ', -1) nombre
	FROM
	  (SELECT 1 n 
	   UNION ALL
	   SELECT 2 
	   UNION ALL 
	   SELECT 3 
	   UNION ALL
	   SELECT 4 
	   UNION ALL 
	   SELECT 5) numeros
	INNER JOIN tblCatalogo c
	  ON CHAR_LENGTH(c.Materias) - CHAR_LENGTH(REPLACE(c.Materias, ' / ', '')) >= numeros.n-1
	LEFT JOIN tblMateria m ON m.Nombre = SUBSTRING_INDEX(SUBSTRING_INDEX(c.Materias, ' / ', numeros.n), ' / ', -1)
	WHERE m.ID IS NULL) ds
GROUP BY  nombre;

INSERT INTO `tblCatalogoMateria` (`Catalogo_ID`, `Materia_ID`)
SELECT cx.ID, m.ID
FROM
	(
    SELECT * FROM (
		SELECT c.ID, TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(c.Materias, ' / ', numeros.n), ' / ', -1)) nombre
		FROM
		  (SELECT 1 n 
		   UNION ALL
		   SELECT 2 
		   UNION ALL 
		   SELECT 3 
		   UNION ALL
		   SELECT 4 
		   UNION ALL 
		   SELECT 5) numeros
		INNER JOIN tblCatalogo c
		  ON CHAR_LENGTH(c.Materias) - CHAR_LENGTH(REPLACE(c.Materias, ' / ', '')) >= numeros.n-1 ) ds
	GROUP BY ID, nombre
    ) cx
INNER JOIN tblMateria m ON m.Nombre = cx.nombre
LEFT JOIN tblCatalogoMateria cm ON cm.Catalogo_ID = cx.ID AND cm.Materia_ID = m.ID
WHERE cm.ID IS NULL;

COMMIT;