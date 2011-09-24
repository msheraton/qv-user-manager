qv-user-manager
===============

qv-user-manager for QlikView 10 allows you to retrieve information about assigned Named CAL's and Document CAL's as well as DMS user authorization for your QlikView Servers.

It also allows you to populate DMS with users in various ways, which can be useful in some DMZ solutions where for example QlikView Publisher is not allowed to handle the distribution.

This project aims to both demonstrate and inspire how to work with the QlikView QMS API while at the same time being useful and a fully working example of what can be accomplished.

Configuration
-------------

Add the user that is executing the tool to the "QlikView Management API" Windows group. If the group does not exist, it must be created.

Change the line below in qv-user-manager.exe.config file to reflect the server address of your QlikView Management Service (QMS).

 	<endpoint address="http://sesth-rfn1:4799/QMS/Service" binding="basicHttpBinding"
 	bindingConfiguration="BasicHttpBinding_IQMSBackend" contract="QMSBackendService.IQMSBackend"
 	name="BasicHttpBinding_IQMSBackend" behaviorConfiguration="ServiceKeyEndpointBehavior" />

It's recommended to schedule and run the tool from a batch file, see below for examples. 

Examples
--------

Examples for populating DMS users:

	:: Add DMS users to Films.qvw from a textfile containing users
	qv-user-manager.exe --add dms --document Films.qvw --input C:\Temp\Users.txt

	:: Add DMS users to Films.qvw AND Presidents.qvw from a textfile containing users
	qv-user-manager.exe --add dms --document Films.qvw --document Presidents.qvw --input C:\Temp\Users.txt

	:: Add DMS users to all available documents from a textfile containing users
	qv-user-manager.exe --add dms --input C:\Temp\Users.txt

	:: Add DMS users to all available documents where users are specified as parameter(s)
	qv-user-manager.exe --add dms --user rikard --user magnus --user mats

Examples for listing DMS users:

	:: List DMS users for all documents on all available QVS to a semicolon separated file
	qv-user-manager.exe --list dms > dmsusers.csv

	:: List DMS users for Films.qvw on all available QVS to a semicolon separated file
	qv-user-manager.exe --list dms --document Films.qvw > dmsusers.csv

Examples for removing DMS users:

	:: Remove DMS users for all documents on all available QVS matching usernames in a textfile
	qv-user-manager.exe --remove dms --input C:\Temp\Users.txt

	:: Remove DMS users for Films.qvw on all available QVS where users are specified as parameters
	qv-user-manager.exe --remove dms --document Films.qvw --user rikard --user magnus

	:: Remove ALL DMS users for all documents on all available QVS
	qv-user-manager.exe --remove dms

Example for listing CAL's:

	:: List Named CALs and DocumentCAL's on all available QVS to a semicolon separated file
	qv-user-manager.exe --list cal > cals.csv

Example for removing CAL's:

	:: Remove all inactive CAL's (inactive > 30 days)
	qv-user-manager.exe --remove cal

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
