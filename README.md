About
=====

qv-user-manager for QlikView 10 is able to automate the process of assigning and removing CALs and retrieve information such as when the CALs were last used in a CSV format, which is simple to read for QlikView. It can also populate DMS users and retrieve information in a similar CSV format. It automatically recognizes all available QlikView Servers and can work against the entire server(s) or specific document(s).

Don't fear the command line! With console redirection and piping support this little tool can with minimal effort be used to for example assign CALs from an Active Directory group or populate DMS users from an SQL Server among other things. See examples below. It's all dynamic and extendable, create your own modules if you whish.

This project aim to both demonstrate and inspire how to work with the QlikView Management Service (QMS) API while at the same time being a useful and fully working example of what can be done.

Help screen
-----------

	Usage: qv-user-manager [options]
	Handles QlikView CALs and DMS user authorizations.

	Options:
	  -l, --list=CAL|DMS         List CALs or usernames to console [CAL|DMS]
	  -a, --add=CAL|DMS          Add users or assign CALs [CAL|DMS]
	  -r, --remove=CAL|DMS       Remove specified users or inactive CALs [CAL|DMS]
	  -d, --document=VALUE       QlikView document(s) to perform actions on
	  -p, --prefix=VALUE         Use specified prefix for all users and CALs
	  -V, --version              Show version information
	  -?, -h, --help             Show usage information

	Options can be in the form -option, /option or --long-option

Configuration
-------------

Add the user that is executing the tool to the "QlikView Management API" Windows group. This group does not exist by default and must be created.

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
	qv-user-manager.exe --add dms --document Films.qvw;Presidents.qvw < C:\Temp\users.txt

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

	:: Add Document CALs from an SQL Server (edit connection string in the script file)
	.\extras\get-sqlusers.ps1 "SELECT users FROM table" | .\qv-user-manager.exe --add cal --document Films.qvw --prefix QTSEL\

	:: Populate DMS users from an XML file (edit the script to match XML format)
	.\extras\get-xmlusers.ps1 .\examples\users-example.xml | .\qv-user-manager.exe --add dms

Note: By default PowerShell has scripting support disabled. To change the script execution policy, use the Set-ExecutionPolicy cmdlet and change it to 'unrestricted'. See the link below for more detailed instructions.

<http://www.tech-recipes.com/rx/2513/powershell_enable_script_support/>

Example how to run from a QlikView script:

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

License
-------

This software is made available "AS IS" without warranty of any kind under The Mit License (MIT). QlikTech support agreement does not cover support for this software.

Meta
----

* Code: `git clone git://github.com/braathen/qv-user-manager.git`
* Home: <https://github.com/braathen/qv-user-manager>
* Bugs: <https://github.com/braathen/qv-user-manager/issues>
