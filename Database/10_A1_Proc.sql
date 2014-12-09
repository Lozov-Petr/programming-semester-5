-- Подарить самому расточительному клиенту билет
CREATE PROCEDURE PresentTicketToTheBestClient
    @FlightNumber VARCHAR(30),   -- Дарим билет на этот рейс
    @NumberOfSeats INTEGER,      -- На это место
    @NewOrderNumber VARCHAR(30)  -- Номер,который получит подарочный билет
AS
    -- Находим ID самого расточительного клиента
    DECLARE @BestClientID INTEGER = (SELECT TOP(1) ClientID FROM
                                        (SELECT LP_Client.ClientID, SUM(Price) AS SumPrice FROM LP_Client
                                         JOIN LP_Order ON LP_Client.ClientID = LP_Order.ClientID
                                         GROUP BY LP_Client.ClientID) AS T
                                     ORDER BY SumPrice DESC);
    -- Находим ID рейса
    DECLARE @FlightID INTEGER = (SELECT FlightID FROM LP_Flight WHERE Number = @FlightNumber);

    -- Создаем билет (к созданию заказа должен быть прикреплён триггер, оповещающий клиента о билете) 
    INSERT INTO LP_Order(FlightID, ClientID, Number, Price,    NumberOfSeats)
        VALUES(@FlightID, @BestClientID, @NewOrderNumber, 0, @NumberOfSeats);

GO

-- EXECUTE PresentTicketToTheBestClient 'AF-1234', 30, 'qwerty';

/*
DROP PROCEDURE PresentTicketToTheBestClient;
*/