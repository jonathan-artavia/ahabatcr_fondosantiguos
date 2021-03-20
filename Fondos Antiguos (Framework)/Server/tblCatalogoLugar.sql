CREATE TABLE `tblCatalogoLugar` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Lugar_ID` BIGINT(20) NOT NULL,
  `Catalogo_ID` BIGINT(20) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `UNIQUE` (`Catalogo_ID` ASC, `Lugar_ID` ASC),
  INDEX `Lugar_idx` (`Lugar_ID` ASC),
  CONSTRAINT `Catalogo`
    FOREIGN KEY (`Catalogo_ID`)
    REFERENCES `tblCatalogo` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `Lugar`
    FOREIGN KEY (`Lugar_ID`)
    REFERENCES `tblLugar` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
COMMENT = 'Lugar x Catalogo';
