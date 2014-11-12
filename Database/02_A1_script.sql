---------------------------------------------------------------
-- Создание таблиц и PK
---------------------------------------------------------------


CREATE TABLE LP_Client
(
    ClientID		 	INTEGER     	NOT NULL,
    Name	    	 	VARCHAR(20) 	NOT NULL,
    Surname     	 	VARCHAR(20)		NOT NULL,
    CreditCardNumber 	VARCHAR(20)			    ,
    PassportNumber   	VARCHAR(12)		NOT NULL,

	CONSTRAINT LP_ClientPK PRIMARY KEY (ClientID)
);


CREATE TABLE LP_Aircraft
(
	AircraftID			INTEGER			NOT NULL,
	Type				VARCHAR(20)		NOT NULL,
	CountOfVacantSeats  INTEGER 		NOT NULL,

	CONSTRAINT LP_AircraftPK PRIMARY KEY (AircraftID)
);


CREATE TABLE LP_Flight
(
	FlightID 			INTEGER 		NOT NULL,
	AircraftID 			INTEGER 		NOT NULL,
	Number 				VARCHAR(30) 	NOT NULL,
	PlaceOfDeparture    VARCHAR(30)		NOT NULL,
	PlaceOfArrival      VARCHAR(30) 	NOT NULL,
	TimeOfDeparture 	DATETIME 		NOT NULL,
	TimeOfArrival 		DATETIME 		NOT NULL,

	CONSTRAINT LP_FlightPK PRIMARY KEY (FlightID)
);


CREATE TABLE LP_Order
(
	FlightID		 	INTEGER			NOT NULL,
	ClientID 			INTEGER 		NOT NULL,
	Number 				VARCHAR(30)		NOT NULL,
	Price 				INTEGER 		NOT NULL,
	TimeOfOrder         DATETIME 		DEFAULT GETDATE() NOT NULL,
	NumberOfSeats       INTEGER 		NOT NULL
);


CREATE TABLE LP_Pilot
(
    PilotID 			INTEGER 		NOT NULL,
    Name	    	 	VARCHAR(20) 	NOT NULL,
    Surname     	 	VARCHAR(20)		NOT NULL,

	CONSTRAINT LP_PilotPK PRIMARY KEY (PilotID)
);


CREATE TABLE LP_FlightPilot
(
	FlightID			INTEGER 		NOT NULL,
	PilotID 			INTEGER 		NOT NULL
);

---------------------------------------------------------------
-- Создание FK 
---------------------------------------------------------------


ALTER TABLE LP_Flight ADD CONSTRAINT LP_FlightAircraftFK
	FOREIGN KEY (AircraftID)
	REFERENCES LP_Aircraft(AircraftID)
;

ALTER TABLE LP_Order ADD CONSTRAINT LP_OrderFlightFK
	FOREIGN KEY (FlightID)
	REFERENCES LP_Flight(FlightID)
;

ALTER TABLE LP_Order ADD CONSTRAINT LP_OrderClientFK
	FOREIGN KEY (ClientID)
	REFERENCES LP_Client(ClientID)
;

ALTER TABLE LP_FlightPilot ADD CONSTRAINT LP_FlPilFlightFK
	FOREIGN KEY (FlightID)
	REFERENCES LP_Flight(FlightID)
;

ALTER TABLE LP_FlightPilot ADD CONSTRAINT LP_FlPilPilotFK
	FOREIGN KEY (PilotID)
	REFERENCES LP_Pilot(PilotID)
;


---------------------------------------------------------------
-- Заполнение таблиц тестовыми данными
---------------------------------------------------------------


INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (1,  'Ivan',   'Petrov',  '1234-1234-1234-1234', '9876-123456');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (2,  'Anna',   'Ivanova', '6846-4865-3548-9999', '4895-321654');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (3,  'Sergey', 'Serov',   '6584-3214-3549-3542', '9845-323541');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (4,  'Pavel',  'Konev',   '8954-3795-3131-4646', '9797-585858');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (5,  'Egor',   'Sidorov', '9843-6874-6712-3787', '1324-987654');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (6,  'Elena',  'Panina',  '9875-6531-9875-6845', '4652-678912');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (7,  'Ivan',   'Karpov',  '7865-6542-6578-3131', '9999-999999');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (8,  'Olga',   'Somova',  '1111-2222-3333-4444', '5555-666666');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (9,  'Nina',   'Lizina',  '9632-8521-7413-9874', '6541-321000');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (10, 'Inna',   'Lapova',  '0000-0000-0000-0001', '0000-000002');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (11, 'Marina', 'Sinaya',  '9898-8787-6565-5454', '3232-212121');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (12, 'Pavel',  'Marov',   '3939-2828-1717-2828', '3939-000000');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (13, 'Igor',   'Tulev',    NULL                , '3939-000001');
INSERT INTO LP_Client(ClientID, Name, Surname, CreditCardNumber, PassportNumber) VALUES (14, 'Roman',  'Tasov',    NULL                , '3939-000002');


INSERT INTO LP_Aircraft(AircraftID, Type, CountOfVacantSeats) VALUES (1, 'BOEING 777-300', 373);
INSERT INTO LP_Aircraft(AircraftID, Type, CountOfVacantSeats) VALUES (2, 'BOEING 777-300', 373);
INSERT INTO LP_Aircraft(AircraftID, Type, CountOfVacantSeats) VALUES (3, 'TU-214',         230);
INSERT INTO LP_Aircraft(AircraftID, Type, CountOfVacantSeats) VALUES (4, 'TU-214',         230);
INSERT INTO LP_Aircraft(AircraftID, Type, CountOfVacantSeats) VALUES (5, 'TU-214',         230);
INSERT INTO LP_Aircraft(AircraftID, Type, CountOfVacantSeats) VALUES (6, 'BOEING 737-500', 101);
INSERT INTO LP_Aircraft(AircraftID, Type, CountOfVacantSeats) VALUES (7, 'BOEING 747-400', 522);
INSERT INTO LP_Aircraft(AircraftID, Type, CountOfVacantSeats) VALUES (8, 'BOEING 737-500', 101);


INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (1, 2, 'AF-1234', 'Moscow',   'New-York',         '2014-10-08 10:15', '2014-10-08 12:25');
INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (2, 1, 'AF-2345', 'London',   'Saint-Petersburg', '2014-10-08 09:20', '2014-10-08 15:35');
INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (3, 3, 'AF-3456', 'Moscow',   'Saint-Petersburg', '2014-10-08 01:45', '2014-10-08 03:05');
INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (4, 7, 'AF-4567', 'New-York', 'Sydney',           '2014-10-09 12:00', '2014-10-10 09:44');
INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (5, 4, 'AF-5678', 'Moscow',   'Paris',            '2014-10-08 18:50', '2014-10-08 20:50');
INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (6, 6, 'AF-6789', 'New-York', 'Paris',            '2014-10-08 21:35', '2014-10-09 05:50');
INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (7, 5, 'AF-7890', 'Moscow',   'Tokyo',            '2014-10-08 18:15', '2014-10-09 08:35');
INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (8, 2, 'AF-8901', 'New-York', 'Las-Vegas',        '2014-10-09 06:30', '2014-10-09 08:59');
INSERT INTO LP_Flight(FlightID, AircraftID, Number, PlaceOfDeparture, PlaceOfArrival, TimeOfDeparture, TimeOfArrival)
	VALUES (9, 4, 'AF-8901', 'Paris',    'Tokyo',            '2014-10-09 10:50', '2014-10-10 06:00');


INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (1, 1,  'T0001', 30000, '2014-10-01 10:14', 31);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (4, 1,  'T0004', 70000, '2014-10-01 10:14', 1 );
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (2, 11, 'T0005', 20000, '2014-10-01 18:01', 60);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (1, 3,  'T0009', 50000, '2014-10-02 14:58', 07);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (8, 3,  'T0030', 70000, '2014-10-02 15:19', 1 );
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (1, 12, 'T0018', 12000, '2014-10-02 15:55', 30);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (3, 2,  'T0021', 5000,  '2014-10-02 18:15', 47);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (7, 4,  'T0023', 18000, '2014-10-02 19:05', 17);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (1, 5,  'T0026', 10000, '2014-10-02 20:00', 48);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (6, 7,  'T0030', 20500, '2014-10-02 20:05', 56);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (9, 7,  'T0031', 17000, '2014-10-02 20:53', 34);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (5, 6,  'T0037', 11000, '2014-10-03 09:09', 20);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (5, 8,  'T0042', 11000, '2014-10-03 11:00', 43);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (5, 9,  'T0043', 11000, '2014-10-03 15:08', 98);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (5, 10, 'T0044', 30000, '2014-10-03 17:09', 02);
INSERT INTO LP_Order(FlightID, ClientID, Number, Price, TimeOfOrder, NumberOfSeats) VALUES (5, 13, 'T0048', 30000, '2014-10-03 17:10', 05);


INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (1, 'Semen',  'Lazarev');
INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (2, 'Matvey', 'Pologov');
INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (3, 'Alexey', 'Pirogov');
INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (4, 'Maxim',  'Baranov');
INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (5, 'Timur',  'Lepesov');
INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (6, 'Ruslan', 'Povarov');
INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (7, 'Lidiya', 'Pravaya');
INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (8, 'Ivan',   'Terehov');
INSERT INTO LP_Pilot(PilotID, Name, Surname) VALUES (9, 'Peter',  'Navarov');


INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (1, 5);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (1, 3);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (6, 5);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (8, 3);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (5, 1);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (9, 1);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (2, 2);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (3, 4);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (4, 6);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (7, 8);
INSERT INTO LP_FlightPilot(FlightID, PilotID) VALUES (3, 7);



---------------------------------------------------------------
-- Удаление таблиц 
---------------------------------------------------------------

/*
DROP TABLE LP_FlightPilot;
DROP TABLE LP_Pilot;
DROP TABLE LP_Order;
DROP TABLE LP_Flight;
DROP TABLE LP_Aircraft;
DROP TABLE LP_Client;
*/