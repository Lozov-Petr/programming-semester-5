
-- Проверка корректности рейса (взлетает раньше, чем садися и не летит туда, откуза вылетел)
CREATE TRIGGER ValidationFlight ON LP_Flight INSTEAD OF INSERT
AS 
    IF EXISTS(SELECT * FROM INSERTED WHERE TimeOfDeparture > TimeOfArrival OR PlaceOfDeparture = PlaceOfArrival)
        ROLLBACK;

GO

-- Проверка корректности места пассажира (не дублируется и сущетвует в данном самолёте)
CREATE TRIGGER ValidationSeats ON LP_Order FOR INSERT
AS

    IF EXISTS(SELECT *
              FROM INSERTED 
              JOIN LP_Flight ON INSERTED.FlightID = LP_Flight.FlightID
              JOIN LP_Aircraft ON LP_Flight.AircraftID = LP_Aircraft.AircraftID
              WHERE NOT NumberOfSeats BETWEEN 1 AND CountOfVacantSeats) 
       OR
       EXISTS(SELECT *
              FROM INSERTED
              JOIN LP_Order ON INSERTED.FlightID = LP_Order.FlightID 
                           AND INSERTED.NumberOfSeats = LP_Order.NumberOfSeats
                           AND INSERTED.OrderID != LP_Order.OrderID)
    ROLLBACK;

GO

-- Необходимая информация о клиенте
CREATE INDEX FullName ON LP_Client(Name, Surname);

-- Занятые места
CREATE INDEX Seats ON LP_Order(FlightID, NumberOfSeats);

-- Не может быть нескольких людей с одинаковым номером паспорта
CREATE UNIQUE INDEX PassportNumber ON LP_Client(PassportNumber);

/*
DROP INDEX FullName ON LP_Client;
DROP INDEX Seats ON LP_Order;
DROP INDEX PassportNumber ON LP_Client;
DROP TRIGGER ValidationFlight;
DROP TRIGGER ValidationSeats;
*/