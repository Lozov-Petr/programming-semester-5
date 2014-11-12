-- Список типов самолётов-BOEING
SELECT DISTINCT Type FROM LP_Aircraft WHERE Type LIKE 'BOEING%';

-- Средняя значение стоимостей, указанных в заказах
SELECT AVG(CAST (Price AS DECIMAL)) FROM LP_Order;

-- Список клиентов, у которых в фамилии первая буква 'S'
SELECT Surname, Name FROM LP_Client WHERE Surname LIKE 'S%' ORDER BY Surname;

-- Список клиентов, у которых указан номер кредитной карты
SELECT Surname, Name FROM LP_Client WHERE CreditCardNumber IS NOT NULL AND CreditCardNumber != '';

-- Список типов самолётов, вместимость которых больше 150 и меньше 200
SELECT DISTINCT Type FROM LP_Aircraft WHERE CountOfVacantSeats BETWEEN 200 AND 400;

-- Список клиентов с именами 'Иван' и 'Pavel'
SELECT * FROM LP_Client WHERE Name IN ('Ivan', 'Pavel');