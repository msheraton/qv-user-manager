qv-user-manager
===============

This tool allows you to retrieve information about assigned Named CAL's and Document CAL's as well as DMS user authorization for your QlikView Servers.

It also allows you to populate DMS with users in various ways.

Configuration
-------------

Add the user that is executing the tool to the "QlikView Management API" Windows group. If the group does not exist, it must be created.

Change the line below in qv-user-manager.exe.config file to reflect the servername of your QlikView Management Service (QMS).

 	<endpoint address="http://sesth-rfn1:4799/QMS/Service" binding="basicHttpBinding"
 	bindingConfiguration="BasicHttpBinding_IQMSBackend" contract="QMSBackendService.IQMSBackend"
 	name="BasicHttpBinding_IQMSBackend" behaviorConfiguration="ServiceKeyEndpointBehavior" />

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


Example for listing CAL's:

	:: List Named CALs and DocumentCAL's on all available QVS to a semicolon separated file
	qv-user-manager.exe --list cal > cals.csv

Meta
----

* Code: `git clone git://github.com/braathen/qv-user-manager.git`
* Home: <https://github.com/braathen/qv-user-manager>
* Bugs: <https://github.com/braathen/qv-user-manager/issues>
