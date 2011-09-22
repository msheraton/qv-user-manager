qv-user-manager
===============

This tool allows you to retrieve information about assigned Named and Document CAL's as well as DMS user authorization in QlikView.

It also allows you to populate DMS users in various ways.

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
