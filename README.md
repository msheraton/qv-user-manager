qv-user-manager
===============

qv-user-manager for QlikView 10 allows you to retrieve information about assigned Named CALs and Document CALs as well as DMS user authorization for your QlikView Servers.

It also allows you to populate DMS with users in various ways, which can be useful in some DMZ solutions where for example QlikView Publisher is not allowed to handle the distribution.

This project aims to both demonstrate and inspire how to work with the QlikView Management Service (QMS) API while at the same time being useful and a fully working example of what can be accomplished.

Help screen
-----------

	Usage: qv-user-manager [options]
	Handles QlikView CALs and DMS user authorizations.

	Options:
	  -l, --list=CAL|DMS         List CALs or usernames to console [CAL|DMS]
	  -a, --add=CAL|DMS          Add users or assign CALs from --input [CAL|DMS]
	  -r, --remove=CAL|DMS       Remove specified users or inactive CALs [CAL|DMS]
	  -d, --document=VALUE       QlikView document(s) to perform actions on
	  -p, --prefix=VALUE         Use specified prefix for all users and CALs
	  -V, --version              Show version information
	  -?, -h, --help             Show usage information

	Options can be in the form -option, /option or --long-option

Configuration
-------------

Add the user that is executing the tool to the "QlikView Management API" Windows group. This group does not exist by default and must therefore be created.

Change the line below in qv-user-manager.exe.config file to reflect the server address of your QlikView Management Service.

 	<endpoint address="http://sesth-rfn1:4799/QMS/Service" binding="basicHttpBinding"
 	bindingConfiguration="BasicHttpBinding_IQMSBackend" contract="QMSBackendService.IQMSBackend"
 	name="BasicHttpBinding_IQMSBackend" behaviorConfiguration="ServiceKeyEndpointBehavior" />

It's recommended to schedule and run the tool from a batch file, see below for examples. 

Examples
--------

Examples for populating DMS users:

	:: Add DMS users to Films.qvw from a textfile containing users
	qv-user-manager.exe --add dms --document Films.qvw < C:\Temp\users.txt

	:: Add DMS users to Films.qvw AND Presidents.qvw from a textfile containing users
	qv-user-manager.exe --add dms --document Films.qvw --document Presidents.qvw < C:\Temp\users.txt

	:: Add DMS users to all available documents from a textfile containing users
	qv-user-manager.exe --add dms < C:\Temp\users.txt

	:: Add DMS users to all available documents where a user is specified on commandline and prefixed
	echo rfn | qv-user-manager.exe --add dms --prefix QTSEL\

Examples for listing DMS users:

	:: List DMS users for all documents on all available QVS to a semicolon separated file
	qv-user-manager.exe --list dms > dmsusers.csv

	:: List DMS users for Films.qvw on all available QVS to a semicolon separated file
	qv-user-manager.exe --list dms --document Films.qvw > dmsusers.csv

Examples for removing DMS users:

	:: Remove DMS users for all documents on all available QVS matching usernames in a textfile
	qv-user-manager.exe --remove dms < C:\Temp\Users.txt

	:: Remove ALL DMS users for all documents on all available QVS
	qv-user-manager.exe --remove dms

	:: Remove DMS users for Films.qvw on all available QVS where a user is specified as parameter
	echo rfn | qv-user-manager.exe --remove dms --document Films.qvw

Example for listing CALs:

	:: List Named CALs and DocumentCALs on all available QVS to a semicolon separated file
	qv-user-manager.exe --list cal > cals.csv

Example for removing CALs:

	:: Remove all inactive CALs (inactive > 30 days)
	qv-user-manager.exe --remove cal

PowerShell examples:

	:: Add CALs from Active Directory (edit the filter in the script file if necessary)
	.\extras\get-adusers.ps1 "LDAP://OU=Stockholm,DC=qliktech,DC=com" | .\qv-user-manager.exe --add cal --prefix QTSEL\

	:: Add CALs from an SQL Server (edit connection string in the script file)
	.\extras\get-sqlusers.ps1 "SELECT users FROM table" | .\qv-user-manager.exe --add cal --document Films.qvw --prefix QTSEL\

Note: By default PowerShell has scripting support disabled. To change the script execution policy, use the Set-ExecutionPolicy cmdlet and change it to 'unrestricted'. See the link below for more detailed instructions.

<http://www.tech-recipes.com/rx/2513/powershell_enable_script_support/>

Example how to run from a QliKView script:

	EXECUTE cmd.exe /C "qv-user-manager.exe --list cal > cals.csv"; 

	Directory;
	LOAD UserName, 
     	LastUsed, 
     	QuarantinedUntil,
     	Document,
     	Server
	FROM
	cals.csv
	(txt, codepage is 1252, embedded labels, delimiter is ';', msq);

Meta
----

* Code: `git clone git://github.com/braathen/qv-user-manager.git`
* Home: <https://github.com/braathen/qv-user-manager>
* Bugs: <https://github.com/braathen/qv-user-manager/issues>
