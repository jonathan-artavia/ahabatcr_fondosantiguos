CREATE TABLE `tblCatalogoMateria` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Materia_ID` BIGINT(20) NOT NULL,
  `Catalogo_ID` BIGINT(20) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `UNIQUE` (`Catalogo_ID` ASC, `Materia_ID` ASC),
  INDEX `Materia_idx` (`Materia_ID` ASC),
  CONSTRAINT `tblCatalogoMateria_Catalogo`
    FOREIGN KEY (`Catalogo_ID`)
    REFERENCES `tblCatalogo` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `tblCatalogoMateria_Materia`
    FOREIGN KEY (`Materia_ID`)
    REFERENCES `tblMateria` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
COMMENT = 'Materias x Catalogo';
