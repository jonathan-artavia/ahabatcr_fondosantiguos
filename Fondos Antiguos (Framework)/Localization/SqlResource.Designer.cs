//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fondos_Antiguos.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class SqlResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SqlResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Fondos_Antiguos.Localization.SqlResource", typeof(SqlResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE `tblCatalogo` SET `Contenido` = @contenido, `Fecha` = @Fecha, `Signatura` = @Signatura, `Observaciones` = @Observaciones, `IdSerie` = @IdSerie, `Fichero` = @Fichero, `NumCaja` = @NumCaja, `NumTomo` = @NumTomo, `Folio` = @Folio, `Libro` = @Libro, `NumExpediente` = @NumExpediente, `NumCarpeta` = @NumCarpeta, `Año` = @Año,`Mes` = @Mes, `FechaIngreso` = @FechaIngreso WHERE `ID` = @id.
        /// </summary>
        public static string SqlCatalogoActualizarResource {
            get {
                return ResourceManager.GetString("SqlCatalogoActualizarResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(ID) 
        ///FROM (SELECT ID, Contenido, Signatura, Fecha, Materias, IdSerie, Fichero, Libro, NumCaja, NumTomo, Folio, NumExpediente, NumCarpeta, Lugar, `Año`, `Mes`, `Observaciones`, NULL as `FechaCod`, NULL as FechaOrig, 0 as `Hist`
        ///FROM `tblCatalogo`
        ///UNION
        ///SELECT ID, Descripcion as Contenido, Signatura, Fecha, Materias, NULL as IdSerie, Fichero, NULL as Libro, NULL as NumCaja, NULL as NumTomo, NULL as Folio, NULL as NumExpediente, NULL as NumCarpeta, Lugar, `Año`, `Mes`, Datos as `Observaciones` [rest of string was truncated]&quot;;.
        /// </summary>
        public static string SqlCatalogoAmbosCountResource {
            get {
                return ResourceManager.GetString("SqlCatalogoAmbosCountResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM
        ///(SELECT c.ID, Contenido, Signatura, Fecha, materias.Nombres as Materias, IdSerie, Fichero, Libro, NumCaja, NumTomo, Folio, NumExpediente, NumCarpeta, lugares.Nombres as Lugar, `Año`, `Mes`, `Observaciones`, NULL as `FechaCod`, NULL as FechaOrig, 0 as `Hist`
        ///FROM `tblCatalogo` c
        ///LEFT JOIN ( SELECT Catalogo_ID, GROUP_CONCAT(Nombre ORDER BY cl.ID SEPARATOR &apos; / &apos;) Nombres
        ///           FROM `tblCatalogoLugar` cl
        ///           LEFT JOIN `tblLugar` l ON l.ID = cl.Lugar_ID
        ///           GROUP BY Catalog [rest of string was truncated]&quot;;.
        /// </summary>
        public static string SqlCatalogoAmbosResource {
            get {
                return ResourceManager.GetString("SqlCatalogoAmbosResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(*) FROM `tblCatalogo` WHERE {0}.
        /// </summary>
        public static string SqlCatalogoCountResource {
            get {
                return ResourceManager.GetString("SqlCatalogoCountResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblCatalogo` WHERE ID = @id.
        /// </summary>
        public static string SqlCatalogoEliminar {
            get {
                return ResourceManager.GetString("SqlCatalogoEliminar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblCatalogo`(`Contenido`, `Fecha`, `Signatura`, `Observaciones`, `IdSerie`, `Fichero`, `NumCaja`, `NumTomo`, `Folio`,`Libro`,`NumExpediente`,`NumCarpeta`,`Lugar`,`Año`,`Mes`,`FechaIngreso`) VALUES (@contenido,@Fecha,@Signatura,@Observaciones,@IdSerie,@Fichero,@NumCaja,@NumTomo,@Folio,@Libro,@NumExpediente,@NumCarpeta,@Lugar,@Año,@Mes,@FechaIngreso);.
        /// </summary>
        public static string SqlCatalogoInsert {
            get {
                return ResourceManager.GetString("SqlCatalogoInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblCatalogoLugar` WHERE `Catalogo_ID` = @catalogoId AND `Lugar_ID` = @lugarId.
        /// </summary>
        public static string SqlCatalogoLugarEliminar {
            get {
                return ResourceManager.GetString("SqlCatalogoLugarEliminar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblCatalogoLugar` WHERE `Catalogo_ID` = @catalogoId.
        /// </summary>
        public static string SqlCatalogoLugarEliminarTodo {
            get {
                return ResourceManager.GetString("SqlCatalogoLugarEliminarTodo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblCatalogoLugar`
        ///(`Lugar_ID`,`Catalogo_ID`) VALUES(@lugarId,@catalogoId);.
        /// </summary>
        public static string SqlCatalogoLugarInsertar {
            get {
                return ResourceManager.GetString("SqlCatalogoLugarInsertar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT cm.*, m.`Nombre` 
        ///FROM `tblCatalogoLugar` as cm
        /// INNER JOIN `tblLugar` AS m ON m.ID = cm.`Lugar_ID`
        ///WHERE cm.Catalogo_ID = @catalogoId.
        /// </summary>
        public static string SqlCatalogoLugarResource {
            get {
                return ResourceManager.GetString("SqlCatalogoLugarResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblCatalogoMateria` WHERE `Catalogo_ID` = @catalogoId AND `Materia_ID` = @materiaId.
        /// </summary>
        public static string SqlCatalogoMateriasEliminar {
            get {
                return ResourceManager.GetString("SqlCatalogoMateriasEliminar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblCatalogoMateria` WHERE `Catalogo_ID` = @catalogoId.
        /// </summary>
        public static string SqlCatalogoMateriasEliminarTodo {
            get {
                return ResourceManager.GetString("SqlCatalogoMateriasEliminarTodo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblCatalogoMateria`
        ///(`Materia_ID`,`Catalogo_ID`) VALUES(@materiaId,@catalogoId);.
        /// </summary>
        public static string SqlCatalogoMateriasInsertar {
            get {
                return ResourceManager.GetString("SqlCatalogoMateriasInsertar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT cm.*, m.`Nombre` 
        ///FROM `tblCatalogoMateria` as cm
        /// INNER JOIN `tblMateria` AS m ON m.ID = cm.`Materia_ID`
        ///WHERE cm.Catalogo_ID = @catalogoId.
        /// </summary>
        public static string SqlCatalogoMateriasResource {
            get {
                return ResourceManager.GetString("SqlCatalogoMateriasResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM
        ///(SELECT c.`ID`, `Contenido`, `Fecha`, `Signatura`, `Observaciones`, `IdSerie`, `Fichero`, `NumCaja`, `NumTomo`, `Folio`, `Libro`, `NumExpediente`, `NumCarpeta`, lugares.Nombres as `Lugar`, `Año`, `Mes`, `FechaIngreso`, materias.Nombres as `Materias`, CAST(0 as SIGNED) as `Hist`
        ///FROM `tblCatalogo` c
        ///LEFT JOIN ( SELECT Catalogo_ID, GROUP_CONCAT(Nombre ORDER BY cl.ID SEPARATOR &apos; / &apos;) Nombres
        ///           FROM `tblCatalogoLugar` cl
        ///           LEFT JOIN `tblLugar` l ON l.ID = cl.Lugar_ID
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        public static string SqlCatalogoResource {
            get {
                return ResourceManager.GetString("SqlCatalogoResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblMensaje` (`Nombre`, `Mensaje`, `UltimaModificacion`) VALUES (@nombre, @mensaje, @ultimaModificacion).
        /// </summary>
        public static string SqlEditorMensajesInsert {
            get {
                return ResourceManager.GetString("SqlEditorMensajesInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT `ID`, `Nombre`, `Mensaje`, `UltimaModificacion` FROM `tblMensaje` WHERE {0}.
        /// </summary>
        public static string SqlEditorMensajesResource {
            get {
                return ResourceManager.GetString("SqlEditorMensajesResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE `tblMensaje` SET `Mensaje` = @mensaje, `UltimaModificacion` = @ultimaModificacion WHERE ID = @id.
        /// </summary>
        public static string SqlEditorMensajesUpdate {
            get {
                return ResourceManager.GetString("SqlEditorMensajesUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE `tblHistCatalogo`
        ///SET
        ///`Descripcion` = @descr,
        ///`Fecha` = @fecha,
        ///`Signatura` = @signatura,
        ///`Datos` = @datos,
        ///`Lugar` = @lugar,
        ///`Materias` = @materias,
        ///`Año` = @año,
        ///`Mes` = @mes
        ///WHERE `ID` = @id;.
        /// </summary>
        public static string SqlHistCatalogoActualizar {
            get {
                return ResourceManager.GetString("SqlHistCatalogoActualizar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(*) FROM `tblHistCatalogo` WHERE {0}.
        /// </summary>
        public static string SqlHistCatalogoCountResource {
            get {
                return ResourceManager.GetString("SqlHistCatalogoCountResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblHistCatalogo` WHERE ID = @id.
        /// </summary>
        public static string SqlHistCatalogoEliminar {
            get {
                return ResourceManager.GetString("SqlHistCatalogoEliminar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT *, CAST(1 as SIGNED) AS `Hist` FROM `tblHistCatalogo` WHERE {0}.
        /// </summary>
        public static string SqlHistCatalogoResource {
            get {
                return ResourceManager.GetString("SqlHistCatalogoResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT LAST_INSERT_ID();.
        /// </summary>
        public static string SqlLastInsertedId {
            get {
                return ResourceManager.GetString("SqlLastInsertedId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblLugar` WHERE Id = @id.
        /// </summary>
        public static string SqlLugarDelete {
            get {
                return ResourceManager.GetString("SqlLugarDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblLugar` (Nombre) VALUES (@nombre).
        /// </summary>
        public static string SqlLugarInsert {
            get {
                return ResourceManager.GetString("SqlLugarInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM `tblLugar` WHERE {0}.
        /// </summary>
        public static string SqlLugarResource {
            get {
                return ResourceManager.GetString("SqlLugarResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblMateria` WHERE Id = @id.
        /// </summary>
        public static string SqlMateriasDelete {
            get {
                return ResourceManager.GetString("SqlMateriasDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblMateria` (Nombre) VALUES (@nombre).
        /// </summary>
        public static string SqlMateriasInsert {
            get {
                return ResourceManager.GetString("SqlMateriasInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM `tblMateria` WHERE {0}.
        /// </summary>
        public static string SqlMateriasResource {
            get {
                return ResourceManager.GetString("SqlMateriasResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LIMIT {0}{1}.
        /// </summary>
        public static string SqlPagingFormat {
            get {
                return ResourceManager.GetString("SqlPagingFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT `IdRol` FROM `tblUsuarios` WHERE {0}.
        /// </summary>
        public static string SqlRoleResource {
            get {
                return ResourceManager.GetString("SqlRoleResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblRoles` WHERE `Id` = @id.
        /// </summary>
        public static string SqlRolesDelete {
            get {
                return ResourceManager.GetString("SqlRolesDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT IdRol FROM tblRoles WHERE `Nombre` = @name.
        /// </summary>
        public static string SqlRolesGetId {
            get {
                return ResourceManager.GetString("SqlRolesGetId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblRoles` (`IdRol`, `Name`) VALUES (@id, @name).
        /// </summary>
        public static string SqlRolesInsert {
            get {
                return ResourceManager.GetString("SqlRolesInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT `Nombre` FROM `tblRoles` WHERE `Id` = @id.
        /// </summary>
        public static string SqlRolesNombreById {
            get {
                return ResourceManager.GetString("SqlRolesNombreById", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT `Nombre` FROM `tblRoles` WHERE `IdRol` = @id.
        /// </summary>
        public static string SqlRolesNombreByIdRol {
            get {
                return ResourceManager.GetString("SqlRolesNombreByIdRol", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE `tblRoles` SET `Name` = @name WHERE `Id` = @id.
        /// </summary>
        public static string SqlRolesUpdate {
            get {
                return ResourceManager.GetString("SqlRolesUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM `tblRolView` WHERE {0}.
        /// </summary>
        public static string SqlRolesVistasPermitidasByRol {
            get {
                return ResourceManager.GetString("SqlRolesVistasPermitidasByRol", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblSerie` WHERE Id = @id.
        /// </summary>
        public static string SqlSeriesDelete {
            get {
                return ResourceManager.GetString("SqlSeriesDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblSerie` (Nombre) VALUES (@series).
        /// </summary>
        public static string SqlSeriesInsert {
            get {
                return ResourceManager.GetString("SqlSeriesInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM tblSerie WHERE {0}.
        /// </summary>
        public static string SqlSeriesResource {
            get {
                return ResourceManager.GetString("SqlSeriesResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblUsuarioClaims` WHERE `UserId` = @userId.
        /// </summary>
        public static string SqlUsuarioClaimDelete {
            get {
                return ResourceManager.GetString("SqlUsuarioClaimDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblUsuarioClaims` WHERE `IdUsuario` = @userId and @ClaimValue = @value and `ClaimType` = @type.
        /// </summary>
        public static string SqlUsuarioClaimDeleteSingle {
            get {
                return ResourceManager.GetString("SqlUsuarioClaimDeleteSingle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblUsuarioClaims` (ClaimValue, ClaimType, IdUsuario) values (@value, @type, @userId).
        /// </summary>
        public static string SqlUsuarioClaimInsert {
            get {
                return ResourceManager.GetString("SqlUsuarioClaimInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM `tblUsuarioClaims` WHERE `UserId` = @userId.
        /// </summary>
        public static string SqlUsuarioClaimSelect {
            get {
                return ResourceManager.GetString("SqlUsuarioClaimSelect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT tblRoles.Nombre FROM tblUsuarios, tblRoles WHERE tblUsuarios.IdUsuario = @userId and tblRoles.IdRol = tblUsuarios.IdRol.
        /// </summary>
        public static string SqlUsuarioRolesByUserId {
            get {
                return ResourceManager.GetString("SqlUsuarioRolesByUserId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE tblUsuarios SET IdRol = 0 WHERE IdUsuario = @userId.
        /// </summary>
        public static string SqlUsuarioRolesDelete {
            get {
                return ResourceManager.GetString("SqlUsuarioRolesDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE `tblUsuarios` SET `IdRol` = @rolId WHERE `IdUsuario` = @userId.
        /// </summary>
        public static string SqlUsuarioRolesInsert {
            get {
                return ResourceManager.GetString("SqlUsuarioRolesInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM `tblUsuarios` WHERE `IdUsuario` = @id.
        /// </summary>
        public static string SqlUsuariosByIdUsuario {
            get {
                return ResourceManager.GetString("SqlUsuariosByIdUsuario", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM `tblUsuarios` where `Usuario` = @name.
        /// </summary>
        public static string SqlUsuariosByUsuario {
            get {
                return ResourceManager.GetString("SqlUsuariosByUsuario", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM `tblUsuarios` WHERE `IdUsuario` = @userId.
        /// </summary>
        public static string SqlUsuariosDeleteByIdUsuario {
            get {
                return ResourceManager.GetString("SqlUsuariosDeleteByIdUsuario", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select `IdUsuario` from `tblUsuarios` where `Usuario` = @name.
        /// </summary>
        public static string SqlUsuariosIdUsuarioByUsuario {
            get {
                return ResourceManager.GetString("SqlUsuariosIdUsuarioByUsuario", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO `tblUsuarios` (`Usuario`, `IdUsuario`, `Cntrña`. `FechaIngreso`) VALUES (@name, @id, @pwdHash, @fechaIngreso).
        /// </summary>
        public static string SqlUsuariosInsert {
            get {
                return ResourceManager.GetString("SqlUsuariosInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM `tblUsuarios` WHERE {0}.
        /// </summary>
        public static string SqlUsuariosResource {
            get {
                return ResourceManager.GetString("SqlUsuariosResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(*) FROM `tblUsuarios` WHERE {0}.
        /// </summary>
        public static string SqlUsuariosResourceCount {
            get {
                return ResourceManager.GetString("SqlUsuariosResourceCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE `tblUsuarios` SET `Usuario` = @userName, `IntentosFallidos` = @intentos, `EstaBloqueado` = @bloqueado, `FechaDesbloqueo` = @fechaDesb  WHERE `IdUsuario` = @userId.
        /// </summary>
        public static string SqlUsuariosUpdateByIdUsuario {
            get {
                return ResourceManager.GetString("SqlUsuariosUpdateByIdUsuario", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE `tblUsuarios` SET `Cntrña` = @pwdHash, `ReqCambioCntrña` = @req WHERE `IdUsuario` = @id.
        /// </summary>
        public static string SqlUsuariosUpdatePasswordHash {
            get {
                return ResourceManager.GetString("SqlUsuariosUpdatePasswordHash", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT `Usuario` FROM `tblUsuarios` WHERE `IdUsuario` = @id .
        /// </summary>
        public static string SqlUsuariosUsuarioByIdUsuario {
            get {
                return ResourceManager.GetString("SqlUsuariosUsuarioByIdUsuario", resourceCulture);
            }
        }
    }
}
