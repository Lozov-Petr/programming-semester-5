-- Добавление клиента
INSERT INTO LP_Client SELECT MAX(ClientID) + 1, 'peter', 'surikov', NULL, '1234567890' FROM LP_Client;

-- Приведение полей Name и Surname у клиентов в надлежащий вид
UPDATE LP_Client SET Name    = UPPER(LEFT(Name,    1)) + LOWER(RIGHT(Name,    LEN(Name)    - 1)), 
                     Surname = UPPER(LEFT(Surname, 1)) + LOWER(RIGHT(Surname, LEN(Surname) - 1)); 
                     
-- Обновление поля CreditCardNumber у клиента Surikov Peter
UPDATE LP_Client SET CreditCardNumber = '1234-9876-4563-0000' WHERE Name = 'Peter' AND Surname = 'Surikov';

-- Добавление заказов для клиента Surikov Peter на каждый рейс
INSERT INTO LP_Order 
	SELECT FlightID, (SELECT ClientID FROM LP_Client WHERE Name = 'Peter' AND Surname = 'Surikov'), 
	       'T000', 10000, GETDATE(), 1 FROM LP_Flight;
	       
-- Удаление заказов на рейсы из Москвы для клиента Surikov Peter

DELETE FROM LP_Order WHERE ClientID IN (SELECT ClientID FROM LP_Client WHERE Name = 'Peter' AND Surname = 'Surikov') AND
                           FlightID IN (SELECT FlightID FROM LP_Flight WHERE PlaceOfDeparture = 'Moscow')
 
 
-- Замена CONSTRAINT
ALTER TABLE LP_Order DROP CONSTRAINT LP_OrderClientFK;                       
ALTER TABLE LP_Order ADD CONSTRAINT LP_OrderClientFK
	FOREIGN KEY (ClientID)
	REFERENCES LP_Client(ClientID)
	ON DELETE CASCADE
;

-- Удаление клиента Surikov Peter со всеми его заказами
DELETE FROM LP_Client WHERE Name = 'Peter' AND Surname = 'Surikov';