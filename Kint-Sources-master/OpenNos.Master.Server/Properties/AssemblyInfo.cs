﻿using log4net.Config;
using System.Reflection;
using System.Runtime.InteropServices;

// Allgemeine Informationen über eine Assembly werden über die folgenden Attribute gesteuert. Ändern
// Sie diese Attributwerte, um die Informationen zu ändern, die einer Assembly zugeordnet sind.
[assembly: AssemblyTitle("OpenNos Master Server")]
[assembly: AssemblyDescription("Source# Master")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Source# Inc.")]
[assembly: AssemblyProduct("OpenNos.Master.Server")]
[assembly: AssemblyCopyright("Source# Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf FALSE werden die Typen in dieser Assembly für COM-Komponenten
// unsichtbar. Wenn Sie auf einen Typ in dieser Assembly von COM aus zugreifen müssen, sollten Sie
// das ComVisible-Attribut für diesen Typ auf "True" festlegen.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("9aa91bf5-88e7-4130-9f42-73ae206e2916")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
// Hauptversion Nebenversion Buildnummer Revision
//
// Sie können alle Werte angeben oder Standardwerte für die Build- und Revisionsnummern verwenden,
// übernehmen, indem Sie "*" eingeben: [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.9.0.*")]
[assembly: XmlConfigurator(Watch = true)]
