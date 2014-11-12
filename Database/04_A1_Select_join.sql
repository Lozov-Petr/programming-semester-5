-- Список: Номер рейса - Тип самолёта (без JOIN)
SELECT Number, Type FROM LP_Flight, LP_Aircraft WHERE LP_Aircraft.AircraftID = LP_Flight.AircraftID;


-- Список: номер заказа - фамилия пассажира (если у пассажира нет заказов, то есть в таблице со значением NULL)
SELECT Number, Surname FROM LP_Order 
	RIGHT JOIN LP_Client ON LP_Client.ClientID = LP_Order.ClientID;

-- Список: Фамилия пилота - Номер рейса - Время вылета, отортированный по времени вылета
SELECT Surname, Number, TimeOfArrival FROM LP_FlightPilot 
	JOIN LP_Pilot  ON LP_FlightPilot.PilotID  = LP_Pilot.PilotID
    JOIN LP_Flight ON LP_FlightPilot.FlightID = LP_Flight.FlightID
    ORDER BY TimeOfArrival;

-- Список: Фамилия пассажира - Номер рейса - Стоимость билета, для тех заказов, стоимость которых больше 20000 и меньше 30000, sort по стоимости
SELECT Surname, LP_FLIGHT.Number, Price FROM LP_Client
	JOIN LP_Order ON LP_Client.ClientID = LP_Order.ClientID
	JOIN LP_Flight ON LP_Flight.FlightID = LP_Order.FlightID
	WHERE Price BETWEEN 20000 AND 30000
	ORDER BY Price;
    
-- Cписок: Фамилия пилота - Фамилия пассажира (который летит на самолёте, управляемым этим пилотом)
SELECT LP_Pilot.Surname, LP_Client.Surname FROM LP_Pilot
	LEFT JOIN LP_FlightPilot ON LP_Pilot.PilotID = LP_FlightPilot.PilotID
	LEFT JOIN LP_Flight ON LP_Flight.FlightID = LP_FlightPilot.FlightID
	LEFT JOIN LP_Order ON LP_Order.FlightID = LP_Flight.FlightID
	FULL JOIN LP_Client ON LP_Client.ClientID = LP_Order.ClientID

