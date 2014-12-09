-- Расписание рейсов, вылетающих из Москвы, для вывода на табло
CREATE VIEW LP_V_Schedule (FlightNumber, AircraftNumber, TimeOfDeparture, PlaceOfArrival) AS
	SELECT LP_Flight.Number, LP_Aircraft.Number, TimeOfDeparture, PlaceOfArrival FROM LP_Flight
	JOIN LP_Aircraft ON LP_Aircraft.AircraftID = LP_Flight.AircraftID
	WHERE PlaceOfDeparture = 'Moscow';

GO

-- Отсортированное расписание вылетов из Москвы	
SELECT * FROM LP_V_Schedule ORDER BY TimeOfDeparture;

GO

CREATE VIEW LP_V_ClientSumCosts (Surname, SumCosts) AS
	SELECT Surname, SUM(Price) FROM LP_Client
	JOIN LP_Order ON LP_Client.ClientID = LP_Order.ClientID
	GROUP BY LP_Client.ClientID, Surname;

GO

-- Фамилия пассажира - его рейтинг (чем больше потратил, тем выше)
SELECT Surname, 100 * SumCosts / (SELECT MAX(SumCosts) FROM LP_V_ClientSumCosts) 
       FROM LP_V_ClientSumCosts 
       ORDER BY SumCosts DESC;
       
GO

--
CREATE VIEW LP_V_LastOrderForAllClients (Surname, LastNumber) AS
	SELECT Surname, MAX(Number) FROM LP_Order  AS ord1
	JOIN LP_Client ON LP_Client.ClientID = ord1.ClientID
	GROUP BY LP_Client.ClientID, Surname;

GO

DELETE FROM LP_Order WHERE Number IN (SELECT LastNumber FROM LP_V_LastOrderForAllClients);


/*
DROP VIEW LP_V_Schedule
DROP VIEW LP_V_ClientSumCosts
DROP VIEW LP_V_LastOrderForAllClients
*/