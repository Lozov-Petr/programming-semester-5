-- Оценки заказов студентов
SELECT Mark FROM A0_Order 
	WHERE Employee_ID IN (
		SELECT Employee_ID FROM A0_Employee 
			WHERE Spec = 'student');
	
-- Фамилии исполнителей без заказов
SELECT Last_Name FROM A0_Employee
	JOIN A0_Person ON A0_Employee.Person_ID = A0_Person.Person_ID
	WHERE Employee_ID NOT IN (
		SELECT Employee_ID FROM A0_Order);

-- Список заказов
SELECT A0_Service.Name AS Service , P_Emp.Last_Name AS Employee, Time_Order, Nick, P_Own.Last_Name AS Owner FROM A0_Order
	JOIN A0_Service ON A0_Order.Service_ID = A0_Service.Service_ID
	JOIN A0_Pet ON A0_Order.Pet_ID = A0_Pet.Pet_ID
	JOIN A0_Employee ON A0_Employee.Employee_ID = A0_Order.Employee_ID
	JOIN A0_Owner ON A0_Owner.Owner_ID = A0_Order.Owner_ID
	JOIN A0_Person AS P_Emp ON P_Emp.Person_ID = A0_Employee.Person_ID
	JOIN A0_Person As P_Own On P_Own.Person_ID = A0_Owner.Person_ID
	
-- Комментарии
SELECT Comments FROM A0_Order WHERE Comments <> ''
UNION
SELECT Description FROM A0_Pet WHERE Description <> ''
UNION
SELECT Description FROM A0_Owner WHERE Description <> ''
            
            