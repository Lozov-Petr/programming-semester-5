-- Номер вылета - Количество пилотов
SELECT Number, COUNT(*) FROM LP_Pilot
	JOIN LP_FlightPilot ON LP_Pilot.PilotID = LP_FlightPilot.PilotID
	JOIN LP_Flight ON LP_Flight.FlightID = LP_FlightPilot.FlightID
	Group BY Number
	
-- Название города - количество рейсов, вылетающих из этого города (строка есть, если рейсов более 2 и длина названия города > 5)
SELECT PlaceOfDeparture, COUNT(*) FROM LP_Order
	JOIN LP_Flight ON LP_Flight.FlightID = LP_Order.FlightID
	WHERE LEN(PlaceOfDeparture) > 5
	GROUP BY PlaceOfDeparture
	HAVING COUNT(*) > 2

-- Фамилия пассажира большими буквами - количество заказов - средняя стоимость купленных им билетов
SELECT UPPER(Surname) , COUNT(*), AVG(CAST (Price AS DECIMAL)) FROM LP_Client
	JOIN LP_Order ON LP_Order.ClientID = LP_Client.ClientID
	GROUP BY LP_Client.ClientID, Surname;
	
-- Номер рейса - количество заказов - день вылета - месяц вылета
SELECT LP_Flight.Number, COUNT(*), DAY(TimeOfDeparture), MONTH(TimeOfDeparture) FROM LP_Flight
	JOIN LP_Order ON LP_Order.FlightID = LP_Flight.FlightID
	GROUP BY LP_Flight.Number, LP_Flight.TimeOfDeparture;
	
-- День - Месяц - Количество заказов со стоимостью > 20000 (строка есть, есл таких заказов более 2)
SELECT DAY(TimeOfOrder), MONTH(TimeOfOrder), COUNT(*) FROM LP_Order
	WHERE Price > 20000
	GROUP BY DAY(TimeOfOrder), MONTH(TimeOfOrder)
	HAVING COUNT(*) > 2;