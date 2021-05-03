# AnomalyDetectionWebService

Flight Inspection Web Service, ex2, Advanced Programming 2, biu

# To add :

 * Installing guide
 * youtube link
 * About the csv and xml files
 * uml INCLUDING ASP SERVER MVC AND CLIENT

# Introduction
A service for computing correlation between features, and detect the anomalies according to the normal model that had been learnt.
The project includes indeed 2 parts,
The first part is http server which handle request about the anomaly detection algorithms.The server implements the approach of REST.
The second part is client-side(browser page) which uses the server and enable non-technical people to interact with the server via the browser.

# Folder structure links
(AnomalyDetectionWebService shortcut is ADWS)
 * [ADWS/](/)      Root folder
 * [ADWS/ADWS/](AnomalyDetectionWebService/)  Source files of the server side
* [ADWS/ADWS/Controllers/](AnomalyDetectionWebService/Controllers/)  Source files of controllers which handle http request from server side
* [ADWS/ADWS/NormalModelsDB/](AnomalyDetectionWebService/NormalModelsDB/)   Folder to store the trained data, the correlative feature according to normal flight
* [ADWS/ADWS/Properties/](AnomalyDetectionWebService/Properties/)  contains launchSettings.json to set if it's developing / production environment
* [ADWS/ADWS/wwwroot/](AnomalyDetectionWebService/wwwroot/)  The folder which contain the page the server sends to the client. Its static resource of the server, but it operates dynamicaly in the client browser 


# Pre requirements
* For establishing the server: asp .net core 5.0
* (Developing is recommended in visual studio 2019 with/without 'swagger' tool)
* For the client page : normal browser which support javascript and html

# Youtube link:

# Further words
* The project isn't suitable for private / secure communication, both from technical issues, and not taking security as most important thing while developing it.
* AnomalyDetectoion class can support trivial algorithms, while all you need in order to add your algorithm is to implements ThresholdMethods and CheckerMethods correctly and add them to the dictionaries in the class.
* I recommend use swagger, read comments at AnomalyDetectionController.cs, and if need, see the Request/response object that are resolved via json parse to get idea what each uri can give ,what possible return http status can be, and what is the way to use the uri correctly
* For developers for this program - server side - be careful about asyncronic programming, match-type from client(for example check fields aren't null, corrent range of num etc), json resolver (works for known + public proprties when make json to string)
* See also the Uml classes diagram for server side 
