-- Фамилия пассажира - средняя стоимость купленных им билетов
SELECT Surname, (SELECT AVG(CAST (Price AS DECIMAL)) FROM LP_Order WHERE LP_Order.ClientID = LP_Client.ClientID) FROM LP_Client;

-- Фамилии тех пассажиров, которые купили более одного билета
SELECT Surname FROM LP_Client WHERE (SELECT COUNT(*) FROM LP_Order WHERE LP_Client.ClientID = LP_Order.ClientID) > 1;

-- Номера самолётов, которые будут вылетать из Москвы
SELECT Number FROM LP_Aircraft AS A WHERE EXISTS (SELECT * FROM LP_Flight AS F WHERE A.AircraftID = F.AircraftID AND F.PlaceOfDeparture = 'Moscow');

-- Отсортированные имена и фамилии пассажиров и пилотов в одной таблице
SELECT Surname, Name FROM LP_Client
UNION
SELECT Surname, Name FROM LP_Pilot
ORDER BY Surname, Name;

-- Номер самолёта - количество сободных мест
SELECT Number, CountOfVacantSeats - (SELECT COUNT(*) FROM LP_Order, LP_Flight 
		WHERE LP_Order.FlightID = LP_Flight.FlightID AND LP_Flight.AircraftID = LP_Aircraft.AircraftID) FROM LP_Aircraft;